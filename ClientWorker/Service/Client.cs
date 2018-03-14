using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace ClientWorker
{
	public class Client
	{
        

        private string address = "fokes1.asuscomm.com";
        private int port = 7777;
        public TcpClient client;
        public NetworkStream netStreamWithoutEncrypt;
        public NegotiateStream netStream;

        public Functions handler;
        private ClientState cState;
        private IAsyncResult ars;
        private IIdentity id;
        public Client()
		{
			Log.Send("Client.Client()");
			handler = new Functions();
			handler.Start();
		}
        
		public void Clear()
		{
			Log.Send("Client.Clear()");
            netStreamWithoutEncrypt.Close();
            netStream.Close();
            client.Close();
        }

		public void Start()
		{
			Log.Send("Client.Start");			

            try
            {
                client = null;
                address = GetFirstSucsessAdress();
                StartData.currentUser = address;
                Log.Send("SucsessIp: " + address);
                //client.SendTimeout = 5000;
                client = new TcpClient(address, port);

                netStreamWithoutEncrypt = client.GetStream();             
                netStream = new NegotiateStream(netStreamWithoutEncrypt, false);

                cState = new ClientState(netStream, client);

                ars = netStream.BeginAuthenticateAsClient(
              new AsyncCallback(EndAuthenticateCallback),
              netStream
              );

                ars.AsyncWaitHandle.WaitOne();

                netStream.BeginRead(cState.Buffer, 0, cState.Buffer.Length,
                       new AsyncCallback(EndReadCallback),
                       cState);

                SendMessage("First Connect");

                cState.Waiter.Reset();
                cState.Waiter.WaitOne();

            }
            catch (Exception ex)
            {
                Log.Send("Client.Start() " + ex.Message);
                //int t = 5000;
                //Thread.Sleep(t);             
            }
            finally
            {
                Start();
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
            NegotiateStream authStream = (NegotiateStream)ars.AsyncState;
            authStream.EndAuthenticateAsClient(ars);
        }

        public void EndReadCallback(IAsyncResult ar)
        {
            ClientState cState = (ClientState)ar.AsyncState;
            TcpClient clientRequest = cState.Client;
            NegotiateStream authStream = (NegotiateStream)cState.AuthenticatedStream;

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

                    id = authStream.RemoteIdentity;
                    handler.Analysis(cState.Message.ToString());
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
            NegotiateStream authStream = (NegotiateStream)ars.AsyncState;

            authStream.EndWrite(ars);
        }

        internal class ClientState
        {
            private AuthenticatedStream authStream = null;
            private TcpClient client = null;
            byte[] buffer = new byte[2048];
            StringBuilder message = null;
            ManualResetEvent waiter = new ManualResetEvent(false);
            internal ClientState(AuthenticatedStream a, TcpClient theClient)
            {
                authStream = a;
                client = theClient;
            }
            internal TcpClient Client
            {
                get { return client; }
            }
            internal AuthenticatedStream AuthenticatedStream
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

		private string GetFirstSucsessAdress()
		{
			string result = "";
			foreach (string text in StartData.ddnsHostName)
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
