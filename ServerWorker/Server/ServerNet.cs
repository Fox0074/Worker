#define USE_COMPRESSION

using Interfaces;
using ServerWorker.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace ServerWorker
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

    public enum UserType
    {
        User,
        Admin,
        System,
    }

    public class ServerNet : IEvents
    {
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



        public const int PING_TIME = 7000;
        public System.Net.Sockets.TcpListener SERV;
        public static readonly SyncAccess ConnectedUsers = new SyncAccess();


        public ServerNet(int Port)
        {
            //SERV = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, Port);
            SERV = new System.Net.Sockets.TcpListener(IPAddress.Parse("127.0.0.1"), Port);
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
            Log.Send("Подключился клиент: " + up.UserType);



            try
            {
                up.nStream.BeginRead(up.HeaderLength, OnDataReadCallback, up);
                try
                {
                    Log.Send(up.UsersCom.TestFunc("OLOLO"));
                }
                catch (Exception ex)
                {
                    Log.Send(string.Concat("-> \"", GetAllNestedMessages(ex), "\""));
                }
            }
            catch (IOException ex)
            {
                ConnectedUsers.Remove(up);
                Log.Send("Не удалось подключить пользователя: " + ex.Message);
            }
        }

        private static string GetAllNestedMessages(Exception ex)
        {
            string s = ex.Message;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                s += string.Concat(Environment.NewLine, ex.Message);
            }
            return s;
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
                if (msg.Command == "OnPing")
                {
                    // отражаем пинг
                    SendMessage(up.nStream, msg);
                    if (Events.OnPing != null) Events.OnPing.BeginInvoke(null, null);
                }
                else
                {
                    if (msg.IsSync)
                    {  // получен результат синхронной процедуры
                        up.SyncResult(msg);
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

                up.nStream.BeginRead(up.HeaderLength, OnDataReadCallback, up);
            }
            catch (Exception ex)
            {
                ConnectedUsers.Remove(up);
                Log.Send("Пользователь " + up.UserType + " удален. Ошибка: " + ex.Message);
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

        public static void SendMessage(ConqurentNetworkStream nStream, Unit msg)
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
}
