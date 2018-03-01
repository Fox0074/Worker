using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

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
        private StringBuilder builder;
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
            client.Close();
        }

		public void Start()
		{
			Log.Send("Client.Start");

			client = null;

			try
			{
				address = GetFirstSucsessAdress();
				StartData.currentUser = address;
				Log.Send("SucsessIp: " + address);
                //client.SendTimeout = 2000;
                client = new TcpClient("127.0.0.1", port);
                netStreamWithoutEncrypt = client.GetStream();

                netStream = new NegotiateStream(netStreamWithoutEncrypt, false);
                IAsyncResult ar = netStream.BeginAuthenticateAsClient(
              new AsyncCallback(EndAuthenticateCallback),
              netStream
              );
                ar.AsyncWaitHandle.WaitOne();

                string text = "FirstConnect";
				byte[] array = Encoding.Unicode.GetBytes(text);
                netStream.Write(array, 0, array.Length);
				Log.Send("Отправлено: " + text);
				while(true)
				{
					array = new byte[64];
					builder = new StringBuilder();
					do
					{
						int count = netStream.Read(array, 0, array.Length);
						builder.Append(Encoding.Unicode.GetString(array, 0, count));
					}
					while (netStreamWithoutEncrypt.DataAvailable);
					text = builder.ToString();
					Log.Send("Сервер: " + text);
					handler.Analysis(text);
				}
			}
			catch (Exception ex)
			{
				Log.Send(ex.Message);
				handler.Reconnect();
			}
			finally
			{
				try
				{
                    client.Client.Shutdown(SocketShutdown.Both);
                    client.Close();
					Log.Send("Tcp connected close");
				}
				catch (Exception ex2)
				{
					Log.Send("Tcp connected cant close" + ex2.Message);
				}
			}
		}

        public static void EndAuthenticateCallback(IAsyncResult ar)
        {
            Console.WriteLine("Client ending authentication...");
            NegotiateStream authStream = (NegotiateStream)ar.AsyncState;
            Console.WriteLine("ImpersonationLevel: {0}", authStream.ImpersonationLevel);

            // End the asynchronous operation.
            authStream.EndAuthenticateAsClient(ar);
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
