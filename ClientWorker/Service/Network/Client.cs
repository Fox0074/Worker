﻿#define USE_COMPRESSION

using Interfaces;
using Service;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

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
        public TcpClient ServerSocket;
        private readonly object tcpSendLock = new object();
        private readonly object syncLock = new object();
        //private Unit _syncResult;
        private Functions functions;

        private Task ListenerTask;
        private CancellationTokenSource ListenerToken;
        private readonly ManualResetEventSlim _OnResponce = new ManualResetEventSlim(false);
        private readonly object _IsConnectedLock = new object();
        private bool _IsConnected = false;
        private bool _IsAuthorized = false;
        private int serverId;
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
            serverId = 0;
            functions = new Functions(this);
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
            string key = Service.Properties.Settings.Default.Key;
            string version = Service.Properties.Settings.Default.Version;
            Unit identificationUint = new Unit("Identification", new object[] { key , version , MClass.isWorking});
            SendData(identificationUint);   
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
                    Log.Send("Filed connected to: " + Host + ":" + Port + "Error: " + ex.Message);
                }
                finally
                {
                    _Dicsonnect();
                }

                if (StartData.ddnsHostName.Contains(Host))
                {
                    foreach (Client client in Program.Servers)
                    {
                        if (client.IsConnected && StartData.ddnsHostName.Contains(client.Host) && client != this)
                        {
                            Program.Servers.Remove(this);
                            return;
                        }
                    }

                    Host = StartData.ddnsHostName[serverId];
                    serverId++;
                    if (serverId > StartData.ddnsHostName.Count - 1) serverId = 0;
                }

                Thread.Sleep(2000);
            }
        }

        public void _Dicsonnect()
        {
            if (ServerSocket != null) ServerSocket.Close();
            IsConnected = false;
            if (Events.OnDisconnect != null) Events.OnDisconnect.BeginInvoke(null, null);

            _OnResponce.Set();
            _IsAuthorized = false;
            GC.Collect(2, GCCollectionMode.Optimized);
        }

        private void ProcessMessage(Unit msg)
        {
            string MethodName = msg.Command;
            if (MethodName == "OnPing")
            {
                if (!msg.IsAnswer)
                {
                    msg.IsAnswer = true;
                    SendData(msg);
                }
                return;
            }

            // ищем запрошенный метод в кольце текущего уровня
             MethodInfo method = functions.GetType().GetMethod(MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            try
            {
                if (method == null)
                {
                    Log.Send(string.Concat(functions.GetType().ToString(), " -> ", MethodName, "(", string.Join(", ", msg.prms), ")"));
                    throw new Exception(string.Concat("Метод \"", MethodName, "\" недоступен"));
                }

                try
                {
                    // выполняем метод интерфейса
                    msg.ReturnValue = method.Invoke(functions, msg.prms);
                }
                catch (Exception ex)
                {
                    Log.Send((ex.InnerException.Message ?? ex.Message).ToString());
                    throw ex.InnerException ?? ex;
                }

                try { Log.Send(string.Concat(functions.GetType().ToString(), ".", MethodName, "(", string.Join(", ", msg.prms), ")")); } catch(Exception ex) { Log.Send("Не удалось залогировать функцию: " + ex.Message); }

                // возвращаем ref и out параметры
                msg.prms = method.GetParameters().Select(x => x.ParameterType.IsByRef ? msg.prms[x.Position] : null).ToArray();
            }
            catch (Exception ex)
            {
                msg.Exception = ex;
            }
            finally
            {
                if (msg.IsSync)
                {
                    // возвращаем результат выполнения запроса
                    msg.IsAnswer = true;
                    SendData(msg);
                    Log.Send("Отправлено: " + msg.Command  + " " + msg.Exception ?? "");
                }
                else
                {
                    if (msg.Exception != null)
                    {
                        msg.IsAnswer = true;
                        SendData(msg);
                        Log.Send("Отправлена ошибка: " + msg.Exception);
                    }
                }
            }
        }

        sealed class DeserializeBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                //TODO: Профиксиль после перевода на 24 либо 25 версию
                if (typeName.Split('.').Last() == "Unit") return typeof(Unit);
                return Type.GetType(typeName);

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
                    binaryFormatter.Binder = new DeserializeBinder();
                    return (T)binaryFormatter.Deserialize(gZipStream);
                }
            }
#else
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            return (T)binaryFormatter.Deserialize(ServerSocket.GetStream());
#endif
        }

        public void SendData(Unit msg)
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