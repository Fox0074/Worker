#define USE_COMPRESSION

using Interfaces;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using ClientWorker;
using System.Collections.Generic;

namespace ClientWorker
{
    public interface IEvents
    {
        Action OnStartConnect { get; set; }
        Action OnConnected { get; set; }
        Action OnDisconnect { get; set; }
        Action OnPing { get; set; }
        Action<Exception> OnError { get; set; }

        Action<int> OnBark { get; set; }
    }

    public class Client : IEvents
    {
        public const int TCP_SIZE = 8192;
        public const int TIMEOUT = 30000;
        public int Port;
        public string Host;
        private TcpClient ServerSocket;
        private readonly object tcpSendLock = new object();
        private readonly object syncLock = new object();
        //private Unit _syncResult;
        private Functions functions = new Functions();

        private Task ListenerTask;
        private CancellationTokenSource ListenerToken;
        private readonly ManualResetEventSlim _OnResponce = new ManualResetEventSlim(false);
        private readonly object _IsConnectedLock = new object();
        private bool _IsConnected = false;
        private bool _IsAuthorized = false;
        private bool IsConnected
        {
            get
            {
                lock (_IsConnectedLock)
                {
                    return _IsConnected;
                }
            }
            set
            {
                lock (_IsConnectedLock)
                {
                    _IsConnected = value;
                }
            }
        }

        public IEvents Events
        {
            get { return this; }
        }


        #region События

        Action IEvents.OnStartConnect { get; set; }
        Action IEvents.OnConnected { get; set; }
        Action IEvents.OnDisconnect { get; set; }
        Action IEvents.OnPing { get; set; }
        Action<Exception> IEvents.OnError { get; set; }

        Action<int> IEvents.OnBark { get; set; }
        #endregion

        public Client()
        {

        }

        public void StartAsync()
        {
            ListenerToken = new CancellationTokenSource();
            ListenerTask = Task.Factory.StartNew(Listener, TaskCreationOptions.LongRunning);
        }

        public bool Connect(bool RaiseException)
        {
            try
            {
                _Connect();
            }
            catch (Exception ex)
            {
                _Dicsonnect();
                if (RaiseException) throw ex;
            }

            bool b = IsConnected;
            StartAsync();

            return b;
        }

        public void ReConnect()
        {
            if (ListenerTask != null && ListenerTask.Status == TaskStatus.Running)
            {
                ListenerToken.Cancel();
                ListenerTask.Wait();
                ListenerToken = new CancellationTokenSource();
                ListenerTask = Task.Factory.StartNew(Listener, ListenerTask.CreationOptions);
            }
            else
            {
                ListenerToken = new CancellationTokenSource();
                ListenerTask = Task.Factory.StartNew(Listener, ListenerTask.CreationOptions);
            }
        }

        public void ReConnectAsync()
        {
            if (ListenerTask != null && ListenerTask.Status == TaskStatus.Running)
            {
                ListenerToken.Cancel();
                ListenerTask = ListenerTask.ContinueWith(t =>
                {
                    t.Dispose();
                    ListenerToken = new CancellationTokenSource();
                    Listener();
                }, (TaskContinuationOptions)ListenerTask.CreationOptions);
            }
            else
            {
                ListenerToken = new CancellationTokenSource();
                ListenerTask = Task.Factory.StartNew(Listener, ListenerTask.CreationOptions);
            }
        }

        #region Private

        private void _Connect()
        {
            ServerSocket = new System.Net.Sockets.TcpClient();
            ServerSocket.ReceiveBufferSize = TCP_SIZE;
            ServerSocket.SendBufferSize = TCP_SIZE;
            ServerSocket.ReceiveTimeout = TIMEOUT;
            ServerSocket.SendTimeout = TIMEOUT;

            #region Подключение

            if (Events.OnStartConnect != null) Events.OnStartConnect.BeginInvoke(null, null);
            ServerSocket.Connect(Host, Port);
            if (Events.OnConnected != null) Events.OnConnected.BeginInvoke(null, null);
            IsConnected = true;
            #endregion
        }

        private void Authorized()
        {
            string login = "User";
            string pass = "IUser";
            Unit authUint = new Unit("ChangePrivileges", new object[] { login, pass });
            SendData(authUint);
            _IsAuthorized = true;
        }
        private void Listener()
        {
            while (true)
            {
                try
                {
                    if (ListenerToken.IsCancellationRequested) return;

                    if (!IsConnected) _Connect();

                    if (!_IsAuthorized) Authorized();

                    while (true)
                    {
                        if (ListenerToken.IsCancellationRequested) return;
                        {
                            Unit msg;
                            try
                            {
                                msg = ReceiveData<Unit>();                               
                            }
                            catch(Exception ex)
                            {
                                Unit exUnit = new Unit("Exception",null);
                                exUnit.Exception = ex.InnerException ?? ex;
                                SendData(exUnit);                                 
                                continue;
                            }

                            ProcessMessage(msg);

                        }

                    }
                }
                catch (TaskCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    if (Events.OnError != null) Events.OnError.BeginInvoke(ex, null, null);
                }
                finally
                {
                    _Dicsonnect();
                }

                Thread.Sleep(2000);
            }
        }

        private void _Dicsonnect()
        {
            if (ServerSocket != null) ServerSocket.Close();
            IsConnected = false;
            if (Events.OnDisconnect != null) Events.OnDisconnect.BeginInvoke(null, null);

            _OnResponce.Set();

            GC.Collect(2, GCCollectionMode.Optimized);
        }

        private void ProcessMessage(Unit msg)
        {
            string MethodName = msg.Command;
            if (MethodName == "OnPing") return;

            // ищем запрошенный метод в кольце текущего уровня
            MethodInfo method = functions.GetType().GetMethod(MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            try
            {
                if (method == null)
                {
                    Console.WriteLine(string.Concat(functions.GetType().ToString(), " -> ", MethodName, "(", string.Join(", ", msg.prms), ")"));
                    throw new Exception(string.Concat("Метод \"", MethodName, "\" недоступен"));
                }

                try
                {
                    // выполняем метод интерфейса
                    msg.ReturnValue = method.Invoke(functions, msg.prms);
                }
                catch (Exception ex)
                {
                    throw ex.InnerException ?? ex;
                }

                Console.WriteLine(string.Concat(functions.GetType().ToString(), ".", MethodName, "(", string.Join(", ", msg.prms), ")"));

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
                SendData(msg);
                Console.WriteLine("Отправлено: " + msg.Command);
            }
        }


        private T ReceiveData<T>() where T : class
        {
#if USE_COMPRESSION

            byte[] DataLength = BitConverter.GetBytes((int)1);
            ServerSocket.GetStream().Read(DataLength, 0, DataLength.Length);
            int len = BitConverter.ToInt32(DataLength, 0);
            byte[] BinaryData = new byte[len];
            ServerSocket.GetStream().Read(BinaryData, 0, BinaryData.Length);

            using (MemoryStream memory = new MemoryStream(BinaryData))
            {
                using (var gZipStream = new GZipStream(memory, CompressionMode.Decompress, false))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    return (T)binaryFormatter.Deserialize(gZipStream);
                }
            }
#else
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            return (T)binaryFormatter.Deserialize(ServerSocket.GetStream());
#endif
        }

        private void SendData(Unit msg)
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

                lock (tcpSendLock)
                {
                    ServerSocket.GetStream().Write(DataWithHeader, 0, DataWithHeader.Length);
                }
            }
#else
            lock (tcpSendLock)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(ServerSocket.GetStream(), msg);
            }
#endif
        }
        #endregion
    }
}