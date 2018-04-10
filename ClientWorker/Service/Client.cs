using Service;
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
        

        private string address = StartData.ddnsHostName[0];
        private int port = 7777;
        public TcpClient client;
        public NetworkStream netStream;

        public Functions handler;
        private ClientState cState;
        private IAsyncResult ars;
        public Client()
		{
			Log.Send("Client конструктор");
			handler = new Functions();
			handler.Start();
		}    
		public void Close()
		{
            try
            {
                Log.Send("Client.Close()");
                netStream.Close();
                client.Close();
            }
            catch(Exception ex)
            {
                Log.Send("Client.Close Error: " + ex.Message);
            }
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

                //client = new TcpClient(address, port);
                client = new TcpClient(Dns.GetHostName(), port);
                client.SendTimeout = 5000;
           
                netStream = client.GetStream();

                cState = new ClientState(netStream, client);

                netStream.BeginRead(cState.Buffer, 0, cState.Buffer.Length,
                       new AsyncCallback(EndReadCallback),
                       cState);

                SendMessage("FirstConnect" + StartData.delimiter + Service.Properties.Settings.Default.Version + StartData.delimiter + Service.Properties.Settings.Default.Key+ StartData.delimiter+ "EndFirstConnect");

                cState.Waiter.Reset();
                cState.Waiter.WaitOne();

            }
            catch (Exception ex)
            {
                Log.Send("Client.Start Exception: " + ex.Message);             
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

                if (bytes != 0 || authStream.DataAvailable)
                {

                    authStream.BeginRead(cState.Buffer, 0, cState.Buffer.Length,new AsyncCallback(EndReadCallback),cState);

                    if (authStream.DataAvailable)
                    {
                        return;
                    }

                    handler.Analysis(cState.Message.ToString());
                    Log.Send("Server says: " + cState.Message.ToString());

                    cState.Message.Remove(0, cState.Message.Length);
                    return;
                }
                else
                {
                    Log.Send("EndReadCallback(): authStream.DataAvailable = " + authStream.DataAvailable + " bytes = " + bytes);
                    Close();
                    Start();
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Send("Client message exception:" + e.Message);
                cState.Waiter.Set();
                return;
            }
        }
        public void EndWriteCallback(IAsyncResult ars)
        {
            NetworkStream authStream = (NetworkStream)ars.AsyncState;

            authStream.EndWrite(ars);
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
