using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerWorker.Server
{
    public class User : IDisposable
    {
        private readonly Timer _pingTimer;
        public Type RingType { get; private set; }
        private Ring _ClassInstance;
        private readonly object syncLock = new object();
        private Unit _syncResult;
        private readonly ManualResetEventSlim _OnResponce = new ManualResetEventSlim(false);

        private readonly Proxy<IDog> DogProxy;
        private readonly Proxy<ICat> CatProxy;
        private readonly Proxy<IUser> UsersComProxy;

        public void SyncResult(Unit msg)
        {  // получен результат выполнения процедуры

            _syncResult = msg;
            _syncResult.IsEmpty = false;

            _OnResponce.Set();  // разблокируем поток
        }

        public IUser UsersCom { get; private set; }
        public IDog Dog { get; private set; }
        public ICat Cat { get; private set; }



        public Ring ClassInstance
        {
            get { return _ClassInstance; }
            set
            {
                _ClassInstance = value;
                RingType = _ClassInstance.GetType();
            }
        }

        public UserType UserType = UserType.User;

        public byte[] HeaderLength = BitConverter.GetBytes((int)0);

        private readonly TcpClient _socket;
        public readonly ConqurentNetworkStream nStream;

        private readonly object _disposeLock = new object();
        private bool _IsDisposed = false;

        public User(TcpClient Socket)
        {
            this._socket = Socket;
            Socket.ReceiveTimeout = PING_TIME * 4;
            Socket.SendTimeout = PING_TIME * 4;
            nStream = new ConqurentNetworkStream(Socket.GetStream());
            _pingTimer = new Timer(OnPing, null, PING_TIME, PING_TIME);
            ClassInstance = new Ring2(this);

            UsersComProxy = new Proxy<IUser>(this);
            DogProxy = new Proxy<IDog>(this);
            CatProxy = new Proxy<ICat>(this);

            UsersCom = (IUser)UsersComProxy.GetTransparentProxy();
            Dog = (IDog)DogProxy.GetTransparentProxy();
            Cat = (ICat)CatProxy.GetTransparentProxy();

        }

        private void OnPing(object state)
        {
            ServerNet.SendMessage(nStream, new Unit("OnPing", null));
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

        public Unit Execute(string MethodName, object[] parameters)
        {
            lock (syncLock)
            {
                _syncResult = new Unit(MethodName, parameters);
                _syncResult.IsSync = true;

                _OnResponce.Reset();
                ServerNet.SendMessage(nStream, _syncResult);
                _OnResponce.Wait();  // ожидаем ответ сервера

                if (_syncResult.IsEmpty)
                {// произошел дисконект, результат не получен
                    throw new Exception(string.Concat("Ошибка при получении результата на команду \"", MethodName, "\""));
                }

                if (_syncResult.Exception != null) throw _syncResult.Exception;  // исключение переданное сервером
                return _syncResult;
            }
        }
    }
}
