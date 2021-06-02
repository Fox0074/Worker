#define USE_COMPRESSION

using Interfaces;
using ServerWorker.Server;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace ServerWorker
{
    public class SubServer
    {
        public User ServerUser;

        public SubServer(string host, string pass)
        {
            TcpClient ServerSocket = new System.Net.Sockets.TcpClient();
            ServerSocket.ReceiveBufferSize = 8192;
            ServerSocket.SendBufferSize = 8192;
            ServerSocket.ReceiveTimeout = 30000;
            ServerSocket.SendTimeout = 30000;

            ServerSocket.Connect(host, 7777);
            ServerUser = new User(ServerSocket);
            ServerUser.nStream.BeginRead(ServerUser.HeaderLength, OnDataReadCallback, ServerUser);
            Program.SubServer = this;
            SendMessage(new Unit("ChangePrivileges", new string[] { Program.authSystem.SessionLoginData.Login, pass }));

            var serverId = ServerUser.SystemCom.ServerIdentification(Program.ServerId);
            if (serverId != null)
                ServerUser.userData = new UserCard.UserData(serverId);

            ServerUser.UserType = ServerUser.AdminCom.GetUsertype();
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
                    if (unit.IsAnswer) user.PingResponce();
                    else
                    {
                        unit.IsAnswer = true;
                        SendMessage(unit);
                    }
                }
                else
                {
                    ProcessMessages.GuideMessage(unit,user);
                }

                user.nStream.BeginRead(user.HeaderLength, OnDataReadCallback, user);
            }
            catch (Exception ex)
            {
                ServerNet.ConnectedUsers.Remove(user);               
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

        public static void SendMessage(Unit msg)
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

                Program.SubServer.ServerUser.nStream.Add(DataWithHeader);
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
