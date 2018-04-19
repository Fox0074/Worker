﻿#define USE_COMPRESSION

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Collections.Concurrent;
using Interfaces;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace ServerWorker
{

    public enum UserType
    {
        User,
        Admin,
        System,
    }

    public class ServerNet
    {
        public const int PING_TIME = 7000;
        public System.Net.Sockets.TcpListener SERV;
        static readonly SyncAccess ConnectedUsers = new SyncAccess();

        #region Синхронный доступ к юзерам

        private class SyncAccess
        {
            private List<User> userList = new List<User>();
            private readonly object listLock = new object();

            public void Add(User item)
            {
                lock (listLock)
                {
                    userList.Add(item);
                }
            }

            public bool Remove(User up)
            {
                lock (listLock)
                {
                    up.Dispose();
                    return userList.Remove(up);
                }
            }

            public User[] ToArray()
            {
                lock (listLock)
                {
                    return userList.ToArray();
                }
            }
        }
        #endregion

        #region Юзер

        #region FIFO

        public class ConqurentNetworkStream : IDisposable
        {
            private readonly BlockingCollection<byte[]> _fifo = new BlockingCollection<byte[]>();
            private readonly NetworkStream _nstream;
            private readonly CancellationTokenSource _token = new CancellationTokenSource();
            private readonly ManualResetEventSlim _disposeEvent = new ManualResetEventSlim(false);

            public ConqurentNetworkStream(NetworkStream nstream)
            {
                this._nstream = nstream;
                ThreadPool.QueueUserWorkItem(_thread);
            }

            private void _thread(object state)
            {
                try
                {
                    while (true)
                    {
                        try
                        {
                            byte[] data = _fifo.Take(_token.Token);
                            _nstream.BeginWrite(data, 0, data.Length, null, null);
                        }
                        catch (InvalidOperationException)
                        {
                            continue;
                        }
                    }
                }
                catch (Exception)
                {
                    _disposeEvent.Set();
                    return;
                }
            }

            public void Add(byte[] data)
            {
                _fifo.Add(data);
            }

            public int Read(byte[] data)
            {
                return _nstream.Read(data, 0, data.Length);
            }

            public int EndRead(IAsyncResult asyncResult)
            {
                return _nstream.EndRead(asyncResult);
            }

            public IAsyncResult BeginRead(byte[] data, AsyncCallback callback, object state)
            {
                return _nstream.BeginRead(data, 0, data.Length, callback, state);
            }

            private readonly object _disposeLock = new object();
            private bool _IsDisposed;
            public void Dispose()
            {
                lock (_disposeLock)
                {
                    if (!_IsDisposed)
                    {
                        _IsDisposed = true;
                        _token.Cancel();
                        _disposeEvent.Wait();
                        _token.Dispose();
                        _disposeEvent.Dispose();
                        _nstream.Dispose();
                    }
                }
            }
        }
        #endregion

        public class User : IDisposable
        {
            private readonly Timer _pingTimer;
            public Type RingType { get; private set; }
            private Ring _ClassInstance;
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
            }

            private void OnPing(object state)
            {
                SendMessage(nStream, new Unit("OnPing", null));
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
        }
        #endregion

        public ServerNet(int Port)
        {
            SERV = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, Port);
            //Console.Title = string.Concat("Порт: ", Port);
        }

        public void Start()
        {
            SERV.Start();
            SERV.BeginAcceptTcpClient(OnAcceptClient, null);
        }

        private void OnAcceptClient(IAsyncResult asyncResult)
        {
            var client = SERV.EndAcceptTcpClient(asyncResult);
            SERV.BeginAcceptTcpClient(OnAcceptClient, null);

            User up = new User(client);
            ConnectedUsers.Add(up);

            try
            {
                up.nStream.BeginRead(up.HeaderLength, OnDataReadCallback, up);
                SendMessage(up.nStream, new Unit("TestFunc", null));
            }
            catch (IOException)
            {
                ConnectedUsers.Remove(up);
            }
        }

        private void OnDataReadCallback(IAsyncResult asyncResult)
        {
            User up = (User)asyncResult.AsyncState;
            byte[] data;

            try
            {
                up.nStream.EndRead(asyncResult);
                int dataLength = BitConverter.ToInt32(up.HeaderLength, 0);
                data = new byte[dataLength];
                up.nStream.Read(data);

                Unit msg = MessageFromBinary<Unit>(data);
                ProcessMessage(msg, up);

                up.nStream.BeginRead(up.HeaderLength, OnDataReadCallback, up);
            }
            catch (Exception)
            {
                ConnectedUsers.Remove(up);
                GC.Collect(2, GCCollectionMode.Optimized);
                return;
            }
        }

        private void ProcessMessage(Unit msg, User u)
        {
            string MethodName = msg.Command;
            if (MethodName == "OnPing") return;

            // ищем запрошенный метод в кольце текущего уровня
            MethodInfo method = u.RingType.GetMethod(MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            try
            {
                if (method == null)
                {
                    Console.WriteLine(string.Concat(u.UserType.ToString(), " -> ", MethodName, "(", string.Join(", ", msg.prms), ")"));
                    throw new Exception(string.Concat("Метод \"", MethodName, "\" недоступен"));
                }

                try
                {
                    // выполняем метод интерфейса
                    msg.ReturnValue = method.Invoke(u.ClassInstance, msg.prms);
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }

                Console.WriteLine(string.Concat(u.UserType.ToString(), " -> ", MethodName, "(", string.Join(", ", msg.prms), ")"));

                // возвращаем ref и out параметры
                msg.prms = method.GetParameters().Select(x => x.ParameterType.IsByRef ? msg.prms[x.Position] : null).ToArray();
            }
            catch (Exception ex)
            {
                msg.Exception = ex;
            }
            finally
            {
                // возвращаем результат выполнения запроса
                SendMessage(u.nStream, msg);
            }
        }

        #region Кольца

        public class Cat_Ring0 : Ring2, ICat
        {
            public Cat_Ring0(User u) : base(u)
            {
                up.UserType = UserType.System;
            }

            public void CutTheText(ref string Text)
            {
                Text = Text.Remove(Text.Length - 1);
            }
        }

        public class Dog_Ring0 : Dog_Ring1, IDog
        {
            public Dog_Ring0(User u) : base(u)
            {
                up.UserType = UserType.Admin;
            }

            public int Bark(int nTimes)
            {
                var ConnectedDogs = ConnectedUsers.ToArray().Where(x => x.UserType == UserType.Admin).Select(x => x.nStream);
                ConnectedDogs.AsParallel().ForAll(nStream =>
                {
                    // инициировать событие у клиента
                    SendMessage(nStream, new Unit("OnBark", new object[] { nTimes }));
                });

                return ConnectedDogs.Count();
            }
        }

        public class Dog_Ring1 : Ring2
        {
            public Dog_Ring1(User u) : base(u)
            {
                up.UserType = UserType.Admin;
            }

            public bool TryFindObject(out object obj)
            {
                obj = "TheBall";
                return true;
            }
        }

        public class Ring2 : Ring, ICommon
        {
            public Ring2(User u) : base(u)
            {

            }

            public string[] GetAvailableUsers()
            {
                return new string[] { "Dog0", "Dog1", "Tom" };
            }

            public void ChangePrivileges(string Animal, string password)
            {
                switch (Animal)
                {
                    case "Dog0":
                        if (password != "groovy!") throw new Exception("Не верный пароль");
                        up.ClassInstance = new Dog_Ring0(up);
                        break;
                    case "Dog1":
                        if (password != "_password") throw new Exception("Не верный пароль");
                        up.ClassInstance = new Dog_Ring1(up);
                        break;
                    case "Tom":
                        if (password != "TheCat") throw new Exception("Не верный пароль");
                        up.ClassInstance = new Cat_Ring0(up);
                        break;
                    default:
                        throw new Exception("Такого пользователя не существует");
                }
            }
        }

        public abstract class Ring
        {
            public readonly User up;

            public Ring(User up)
            {
                this.up = up;
            }
        }
        #endregion

        #region Send/Receive

        private T MessageFromBinary<T>(byte[] BinaryData) where T : class
        {
#if USE_COMPRESSION

            using (MemoryStream memory = new MemoryStream(BinaryData))
            {
                using (var gZipStream = new GZipStream(memory, CompressionMode.Decompress, false))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    return (T)binaryFormatter.Deserialize(gZipStream);
                }
            }
#else
            using (MemoryStream memory = new MemoryStream(BinaryData))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(memory);
            }
#endif
        }

        private static void SendMessage(ConqurentNetworkStream nStream, Unit msg)
        {
#if USE_COMPRESSION

            using (MemoryStream memory = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(memory, CompressionMode.Compress, false))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(gZipStream, msg);
                }

                byte[] BinaryData = memory.ToArray();
                byte[] DataLength = BitConverter.GetBytes(BinaryData.Length);
                byte[] DataWithHeader = DataLength.Concat(BinaryData).ToArray();

                nStream.Add(DataWithHeader);
            }

#else
            using (MemoryStream memory = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memory, msg);
                nStream.Add(memory.ToArray());
            }
#endif
        }
        #endregion
    }
    #region OLD
    //    public class ServerNet : ServerBehaviour
    //    {
    //        public static Action AddClient = delegate { };

    //        //Сервер
    //        const int port = 7777;
    //        public static TcpListener listener;
    //        public static IPAddress localIp = IPAddress.Parse("127.0.0.1");
    //        //public static IPAddress localIp = null;
    //        public List<EndPoint> listIp = new List<EndPoint>();      

    //        //Клиент
    //        public TcpClient client;
    //        Thread clientThread;

    //        #region Конструкторы

    //        public ServerNet(TcpClient tcpClient)
    //        {
    //            client = tcpClient;
    //        }
    //        public ServerNet() { }
    //        #endregion

    //        public override void Start()
    //        {
    //            AvailableLocalIp.CheckAviableNetworkConnections();
    //        }

    //        #region Server
    //        //Запуск сервера
    //        public void StartServer()
    //        {
    //            Program.form1.Interfaces = UserInterface.mainServer;
    //            try
    //            {
    //                //Костыль!!!
    //                if (localIp.ToString() == IPAddress.Parse("127.0.0.1").ToString())
    //                {
    //                    localIp = AvailableLocalIp.GetFirstAviableIp();
    //                }

    //                listener = new TcpListener(IPAddress.Any, port);
    //                listener.Start();

    //                Log.Send("Твой LocalIp " + localIp);
    //                Log.Send("Ожидание подключений...");

    //                while (true)
    //                {
    //                    TcpClient client = listener.AcceptTcpClient();
    //                    Messenger clientObject = new Messenger();
    //                    // создаем новый поток для обслуживания нового клиента
    //                    clientThread = new Thread(new ThreadStart(() => clientObject.Process(client)));
    //                    clientThread.Start();

    //                    AddClient.Invoke();
    //                    Log.Send("Подключился клиент : " + client.Client.RemoteEndPoint);
    //                    listIp.Add(client.Client.RemoteEndPoint);                    
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                Log.Send("ServerNet.StartServer " + ex.Message);
    //            }
    //            finally
    //            {
    //                if (listener != null)
    //                    listener.Stop();               
    //            }
    //        }

    //        //Остановка сервера
    //        public void StopServer()
    //        {
    //            try
    //            {
    //                foreach (Messenger sender in Messenger.messangers)
    //                {
    //                    sender.StopClientStream();
    //                }
    //                Messenger.messangers.Clear();
    //                listener.Stop();
    //                clientThread.Abort();
    //            }
    //            catch (Exception ex)
    //            {
    //                Log.Send("Ошибка при остановке сервера " + ex.Message);
    //            }
    //        }
    //        #endregion

    //        #region AddingFunctions
    //        private IPAddress GetLocalIp()
    //        {
    //            IPHostEntry host;
    //            string hostName = Dns.GetHostName();

    //            host = Dns.GetHostEntry(hostName);
    //            foreach (IPAddress ip in host.AddressList)
    //            {
    //                if (ip.AddressFamily == AddressFamily.InterNetwork)
    //                {
    //                    return ip;
    //                }
    //            }

    //            return IPAddress.Parse("192.168.1.10");
    //        }
    //#endregion
    //    }
    #endregion
}
