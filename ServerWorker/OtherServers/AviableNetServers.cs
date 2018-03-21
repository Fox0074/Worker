using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerWorker
{
    public class AviableNetServers : ServerBehaviour
    {
        private string address = "fokes1.asuscomm.com";
        private int port = 7777;
        public TcpClient client;
        public NetworkStream clientNetStream;
        public NetworkStream netStream;
        public bool isWorking = false;

        private List<string> ddnsHostNames = new List<string> { "fokes1.asuscomm.com" };
        private ClientState cState;
        private IAsyncResult ars;
        private IIdentity id;

        public void Close()
        {
            Log.Send("Client.Clear()");
            isWorking = false;
            try
            {
                client.Close();
                client.Dispose();
                
                clientNetStream.Dispose();
                clientNetStream.Close();
                netStream.Dispose();
                netStream.Close();
                
                return;
            }
            catch (Exception ex)
            {
                Log.Send("Исключение закрытия подключения к удаленному серверу: "+ex.Message);
            }
        }

        public new void  Start()
        {
            Log.Send("Подключение к удаленному серверу");
            isWorking = true;
            try
            {
                client = null;
                address = GetSucsessAdress();
                Log.Send("Определен досупный сервер: " + address);
                //client.SendTimeout = 5000;

                IPAddress[] ipAddress = Dns.GetHostAddresses(address);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress[0], 7777);
                client = new TcpClient();
                client.Connect(remoteEP);
                //client = new TcpClient(address, port);

                clientNetStream = client.GetStream();
                netStream = clientNetStream;

                cState = new ClientState(netStream, client);

                netStream.BeginRead(cState.Buffer, 0, cState.Buffer.Length,
                       new AsyncCallback(EndReadCallback),
                       cState);

                //SendMessage("Key_" + key);

                cState.Waiter.Reset();
                cState.Waiter.WaitOne();

            }
            catch (Exception ex)
            {
                Log.Send("Exception Client.Start(): " + ex.Message);
                //int t = 5000;
                //Thread.Sleep(t);             
            }
            finally
            {
               //Start();
            }
        }

        public void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            ars = netStream.BeginWrite(data, 0, data.Length,
                new AsyncCallback(EndWriteCallback),
                netStream);
            ars.AsyncWaitHandle.WaitOne();
        }

        public static void EndAuthenticateCallback(IAsyncResult ars)
        {
            NetworkStream authStream = (NetworkStream)ars.AsyncState;
           // authStream.EndAuthenticateAsClient(ars);
        }

        public void EndReadCallback(IAsyncResult ar)
        {
            ClientState cState = (ClientState)ar.AsyncState;
            TcpClient clientRequest = cState.Client;
            NetworkStream authStream = (NetworkStream)cState.AuthenticatedStream;

            int bytes = -1;

            try
            {
                bytes = authStream.EndRead(ar);
                cState.Message.Append(Encoding.UTF8.GetChars(cState.Buffer, 0, bytes));
                if (bytes != 0)
                {
                    authStream.BeginRead(cState.Buffer, 0, cState.Buffer.Length,
                          new AsyncCallback(EndReadCallback),
                          cState);

                    //id = authStream.RemoteIdentity;
                    //handler.Analysis(cState.Message.ToString());
                    Log.Send("Server says: " + cState.Message.ToString());

                    cState.Message.Remove(0, cState.Message.Length);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Send("Client message exception:");
                Log.Send(e.Message);
                cState.Waiter.Set();
                return;
            }
            Log.Send("Connections was close on server");
            cState.Waiter.Set();
        }
        public void EndWriteCallback(IAsyncResult ars)
        {
            NetworkStream authStream = (NetworkStream)ars.AsyncState;

            authStream.EndWrite(ars);
        }

        internal class ClientState
        {
            private NetworkStream authStream = null;
            private TcpClient client = null;
            byte[] buffer = new byte[2048];
            StringBuilder message = null;
            ManualResetEvent waiter = new ManualResetEvent(false);
            internal ClientState(NetworkStream a, TcpClient theClient)
            {
                authStream = a;
                client = theClient;
            }
            internal TcpClient Client
            {
                get { return client; }
            }
            internal NetworkStream AuthenticatedStream
            {
                get { return authStream; }
            }
            internal byte[] Buffer
            {
                get { return buffer; }
            }
            internal StringBuilder Message
            {
                get
                {
                    if (message == null)
                        message = new StringBuilder();
                    return message;
                }
            }
            internal ManualResetEvent Waiter
            {
                get
                {
                    return waiter;
                }
            }
        }
        private IPAddress[] GetIpDns(string ddns)
        {
            return Dns.GetHostAddresses(ddns);
        }

        private IPStatus PingIp(string hostName)
        {
            Ping ping = new Ping();
            PingReply pingReply = ping.Send(hostName);
            return pingReply.Status;
        }

        private string GetSucsessAdress()
        {
            string result = "";
            foreach (string text in ddnsHostNames)
            {
                if (PingIp(text) == IPStatus.Success)
                {
                    return text;
                }
            }
            return result;
        }
    }
}
