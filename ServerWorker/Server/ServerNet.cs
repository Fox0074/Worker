﻿#define USE_COMPRESSION

using Interfaces;
using ServerWorker.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;

namespace ServerWorker
{

    public interface IEvents
    {
        Action OnStartConnect { get; set; }
        Action OnConnected { get; set; }
        Action OnDisconnect { get; set; }
        Action OnPing { get; set; }
        Action<Exception> OnError { get; set; }
        Action OnAuthorized { get; set; }

        Action<int> OnBark { get; set; }
    }

    public enum UserType
    {
        UnAuthorized,
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
        Action IEvents.OnAuthorized { get; set; }
        Action<int> IEvents.OnBark { get; set; }
        #endregion

      
        public System.Net.Sockets.TcpListener SERV;
        public static readonly SyncAccess ConnectedUsers = new SyncAccess();


        public ServerNet(int Port)
        {
            SERV = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, Port);
            //SERV = new System.Net.Sockets.TcpListener(IPAddress.Parse("127.0.0.1"), Port);
        }

        public void Start()
        {
            SERV.Start();
            SERV.BeginAcceptTcpClient(OnAcceptClient, null);
        }

        public void Stop()
        {
            SERV.Stop();
        }

        private void OnAcceptClient(IAsyncResult asyncResult)
        {
            var client = SERV.EndAcceptTcpClient(asyncResult);
            SERV.BeginAcceptTcpClient(OnAcceptClient, null);

            User up = new User(client);
            ConnectedUsers.Add(up);
            Log.Send("Подключился клиент: " + up.UserType);
            Events.OnConnected.Invoke();

            try
            {
                up.nStream.BeginRead(up.HeaderLength, OnDataReadCallback, up);
            }
            catch (IOException ex)
            {
                Log.Send(ex.Message);
                ConnectedUsers.Remove(up);
                Events.OnDisconnect.Invoke();
                Log.Send("Не удалось подключить пользователя: " + ex.Message);
            }
        }
    
        private void OnDataReadCallback(IAsyncResult asyncResult)
        {
            User user = (User)asyncResult.AsyncState;
            byte[] data;           
            try
            {
                user.nStream.EndRead(asyncResult);
                int dataLength = BitConverter.ToInt32(user.HeaderLength, 0);
                data = new byte[dataLength];
                WaitData(user._socket,dataLength);
                user.nStream.Read(data);

                Unit unit = MessageFromBinary<Unit>(data);
                if (unit.Command == "OnPing")
                {
                    // отражаем пинг
                    SendMessage(user.nStream, unit);
                    if (Events.OnPing != null) Events.OnPing.BeginInvoke(null, null);
                }
                else
                {
                    ProcessMessages.GuideMessage(unit,user);
                }

                user.nStream.BeginRead(user.HeaderLength, OnDataReadCallback, user);
            }
            catch (Exception ex)
            {
                ConnectedUsers.Remove(user);
                Events.OnDisconnect.Invoke();
                Log.Send("Пользователь " + user.UserType + ": " + user.EndPoint.ToString() + " удален. Ошибка: " + ex.Message);
                GC.Collect(2, GCCollectionMode.Optimized);                
                return;
            }
        }

        private void WaitData(TcpClient stream,int dataLength)
        {
            int x = 0;
            while (x < dataLength)
            {               
                x = stream.Available;
                Thread.Sleep(5);
            }
        }
        #region Send/Receive

        sealed class DeserializeBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                //TODO: Профиксиль после перевода на 24 либо 25 версию
                if (typeName.Split('.').Last() == "Unit") return typeof(Unit);
                return Type.GetType(typeName);

            }
        }

        private T MessageFromBinary<T>(byte[] BinaryData) where T : class
        {
#if USE_COMPRESSION
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
