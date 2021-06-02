using Interfaces;
using Interfaces.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerWorker.UsersRank;
using System.Net;
using System.Diagnostics;

namespace ServerWorker.Server
{
    [Serializable]
    public class UserInfo
    {
        public UserCard.UserData userData;
        public UserType UserType;
        public EndPoint EndPoint;
    }

    public class User : IDisposable
    {
        public Action<string> OnSendChatMessage = delegate { };
        public event Action<User> OnPingUpdate = delegate { };

        public long Ping;
        public const int PING_TIME = 7000;
        public UserCard.UserData userData;
        public bool _isProxyUser = false;
        private readonly Timer _pingTimer;
        public Type RingType { get; private set; }
        public EndPoint EndPoint;
        private AbstractRing _ClassInstance;
        private readonly object syncLock = new object();
        private Unit _syncResult;
        private readonly ManualResetEventSlim _OnResponce = new ManualResetEventSlim(false);
        private Stopwatch stopWatch = new Stopwatch();

        public UserType UserType = UserType.UnAuthorized;

        public byte[] HeaderLength = BitConverter.GetBytes((int)0);
        public readonly TcpClient _socket;
        public readonly ConqurentNetworkStream nStream;
        private readonly object _disposeLock = new object();
        private bool _IsDisposed = false;

        private readonly Proxy<IAdmin> AdminComProxy;
        private readonly Proxy<ISystem> SystemComProxy;
        private readonly Proxy<IUser> UsersComProxy;

        public void SyncResult(Unit msg)
        {  // получен результат выполнения процедуры

            _syncResult = msg;
            _syncResult.IsEmpty = false;

            _OnResponce.Set();  // разблокируем поток
        }

        public IUser UsersCom { get; private set; }
        public IAdmin AdminCom { get; private set; }
        public ISystem SystemCom { get; private set; }

        public AbstractRing ClassInstance
        {
            get { return _ClassInstance; }
            set
            {
                _ClassInstance = value;
                RingType = _ClassInstance.GetType();
            }
        }

        public User(TcpClient Socket)
        {
            this._socket = Socket;
            Socket.ReceiveTimeout = PING_TIME * 4;
            Socket.SendTimeout = PING_TIME * 4;
            Socket.ReceiveBufferSize = 9999999;
            Socket.ReceiveBufferSize = 9999999;
            nStream = new ConqurentNetworkStream(Socket.GetStream());
            _pingTimer = new Timer(OnPing, null, PING_TIME, PING_TIME);
            ClassInstance = new ClientRing(this);
            EndPoint = Socket.Client.RemoteEndPoint;

            UsersComProxy = new Proxy<IUser>(this);
            AdminComProxy = new Proxy<IAdmin>(this);
            SystemComProxy = new Proxy<ISystem>(this);

            UsersCom = (IUser)UsersComProxy.GetTransparentProxy();
            AdminCom = (IAdmin)AdminComProxy.GetTransparentProxy();
            SystemCom = (ISystem)SystemComProxy.GetTransparentProxy();

        }

        public User(UserInfo proxyUser)
        {
            _isProxyUser = true;
            
            EndPoint = proxyUser.EndPoint;
            userData = proxyUser.userData;
            UserType = proxyUser.UserType;

            switch(UserType)
            {
                case UserType.User:
                    ClassInstance = new UserRing(this);
                    break;
                case UserType.Admin:
                    ClassInstance = new AdminRing(this);
                    break;
                case UserType.System:
                    ClassInstance = new SystemRing(this);
                    break;
                default:
                    ClassInstance = new ClientRing(this);
                    break;
            }

            UsersComProxy = new Proxy<IUser>(this);
            AdminComProxy = new Proxy<IAdmin>(this);
            SystemComProxy = new Proxy<ISystem>(this);

            UsersCom = (IUser)UsersComProxy.GetTransparentProxy();
            AdminCom = (IAdmin)AdminComProxy.GetTransparentProxy();
            SystemCom = (ISystem)SystemComProxy.GetTransparentProxy();

        }



        private void OnPing(object state)
        {
            ServerNet.SendMessage(nStream, new Unit("OnPing", null));
            stopWatch.Restart();
        }

        public void PingResponce()
        {
            stopWatch.Stop();
            if (Ping != stopWatch.ElapsedMilliseconds)
            {
                Ping = stopWatch.ElapsedMilliseconds;
                OnPingUpdate(this);
            }
        }

        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (!_IsDisposed)
                {
                    _IsDisposed = true;
                    nStream.Dispose();
                    _pingTimer.Dispose();
                    _socket.Close();
                }
            }
        }

        public Unit Execute(string MethodName, object[] parameters, bool IsWaitAnswer)
        {
            lock (syncLock)
            {
                _syncResult = new Unit(MethodName, parameters);
                _syncResult.IsSync = IsWaitAnswer;
                _syncResult.EndPoint = _isProxyUser ? EndPoint : null;
                if (IsWaitAnswer)
                {
                    _OnResponce.Reset();

                    if (!_isProxyUser)
                        ServerNet.SendMessage(nStream, _syncResult);
                    else
                        SubServer.SendMessage(_syncResult);

                    _OnResponce.Wait();  // ожидаем ответ сервера
                }
                else
                {
                    if (!_isProxyUser)
                        ServerNet.SendMessage(nStream, _syncResult);
                    else
                        SubServer.SendMessage(_syncResult);

                    Log.Send(UserType.ToString() +" "+ EndPoint + " -> " + MethodName);
                }

                if (_syncResult.IsEmpty && IsWaitAnswer)
                {// произошел дисконект, результат не получен
                    throw new Exception(string.Concat("Ошибка при получении результата на команду \"", MethodName, "\""));
                }

                if (_syncResult.Exception != null)
                    throw _syncResult.Exception;  // исключение переданное клиентом          
                return _syncResult;
            }
        }

        public static string GetAllNestedMessages(Exception ex)
        {
            string s = ex.Message;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                s += string.Concat(Environment.NewLine, ex.Message);
            }
            return s;
        }
    }
}
