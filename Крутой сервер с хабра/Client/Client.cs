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

namespace Client
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

    public class UniservClient : IEvents
    {
        public const int TCP_SIZE = 8192;
        public const int TIMEOUT = 30000;
        public int Port;
        public string Host;
        private TcpClient ServerSocket;
        private readonly object tcpSendLock = new object();
        private readonly object syncLock = new object();
        private Message _syncResult;
        private Task ListenerTask;
        private CancellationTokenSource ListenerToken;
        private readonly ManualResetEventSlim _OnResponce = new ManualResetEventSlim(false);
        private readonly object _IsConnectedLock = new object();
        private bool _IsConnected = false;
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
        private readonly Proxy<IDog> DogProxy;
        private readonly Proxy<ICat> CatProxy;
        private readonly Proxy<ICommon> CommonProxy;

        public IEvents Events
        {
            get { return this; }
        }
        public ICommon Common { get; private set; }
        public IDog Dog { get; private set; }
        public ICat Cat { get; private set; }

        #region Proxy

        private class Proxy<T> : RealProxy where T : class
        {
            UniservClient client;

            public Proxy(UniservClient client): base(typeof(T))
            {
                this.client = client;
            }

            public override IMessage Invoke(IMessage msg)
            {
                IMethodCallMessage call = (IMethodCallMessage)msg;
                object[] parameters = call.Args;
                int OutArgsCount = call.MethodBase.GetParameters().Where(x => x.IsOut).Count();

                Message result = client.Execute(call.MethodName, parameters);
                parameters = parameters.Select((x, index) => result.prms[index] ?? x).ToArray();
                return new ReturnMessage(result.ReturnValue, parameters, OutArgsCount, call.LogicalCallContext, call);
            }
        }
        #endregion

        #region События

        Action IEvents.OnStartConnect { get; set; }
        Action IEvents.OnConnected { get; set; }
        Action IEvents.OnDisconnect { get; set; }
        Action IEvents.OnPing { get; set; }
        Action<Exception> IEvents.OnError { get; set; }

        Action<int> IEvents.OnBark { get; set; }
        #endregion

        public UniservClient()
        {
            CommonProxy = new Proxy<ICommon>(this);
            DogProxy = new Proxy<IDog>(this);
            CatProxy = new Proxy<ICat>(this);

            Common = (ICommon)CommonProxy.GetTransparentProxy();
            Dog = (IDog)DogProxy.GetTransparentProxy();
            Cat = (ICat)CatProxy.GetTransparentProxy();
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

        private void Listener()
        {
            while (true)
            {
                try
                {
                    if (ListenerToken.IsCancellationRequested) return;

                    if (!IsConnected) _Connect();

                    while (true)
                    {
                        if (ListenerToken.IsCancellationRequested) return;

                        Message msg = ReceiveData<Message>();
                        if (msg.Command == "OnPing")
                        {
                            // отражаем пинг
                            SendData(msg);
                            if (Events.OnPing != null) Events.OnPing.BeginInvoke(null, null);
                            continue;
                        }

                        if (msg.IsSync)
                        {  // получен результат синхронной процедуры
                            SyncResult(msg);
                        }
                        else
                        {
                            // асинхронный вызов события
                            try
                            {
                                // ищем соответствующий Action
                                var pi = typeof(IEvents).GetProperty(msg.Command, BindingFlags.Instance | BindingFlags.Public);
                                if (pi == null) throw new Exception(string.Concat("Свойство \"", msg.Command, "\" не найдено"));
                                var delegateRef = pi.GetValue(this, null) as Delegate;

                                // инициализируем событие
                                if (delegateRef != null) ThreadPool.QueueUserWorkItem(state => delegateRef.DynamicInvoke(msg.prms));
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(string.Concat("Не удалось выполнить делегат \"", msg.Command, "\""), ex);
                            }
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

        private Message Execute(string MethodName, object[] parameters)
        {
            lock (syncLock)
            {
                _syncResult = new Message(MethodName, parameters);
                _syncResult.IsSync = true;

                _OnResponce.Reset(); 
                SendData(_syncResult);
                _OnResponce.Wait();  // ожидаем ответ сервера

                if (_syncResult.IsEmpty)
                {// произошел дисконект, результат не получен
                    throw new Exception(string.Concat("Ошибка при получении результата на команду \"", MethodName, "\""));
                }

                if (_syncResult.Exception != null) throw _syncResult.Exception;  // исключение переданное сервером
                return _syncResult;
            }
        }

        private void SyncResult(Message msg)
        {  // получен результат выполнения процедуры

            _syncResult = msg;
            _syncResult.IsEmpty = false;

            _OnResponce.Set();  // разблокируем поток
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

        private void SendData(Message msg)
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
