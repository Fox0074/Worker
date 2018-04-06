﻿using System;
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
        public NetworkStream netStream;

        public Functions handler;
        private ClientState cState;
        private IAsyncResult ars;
        private IIdentity id;
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
                netStreamWithoutEncrypt.Close();
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

                netStreamWithoutEncrypt = client.GetStream();             
                netStream = netStreamWithoutEncrypt;

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
            NetworkStream authStream = (NetworkStream)cState.AuthenticatedStream;

            int bytes = -1;

            try
            {
                bytes = authStream.EndRead(ar);
                cState.Message.Append(Encoding.UTF8.GetChars(cState.Buffer, 0, bytes));
                if (bytes != -1)
                {
                    if (bytes == 0)
                        Console.WriteLine("Пришло 0 ");
                    authStream.BeginRead(cState.Buffer, 0, cState.Buffer.Length,
                          new AsyncCallback(EndReadCallback),
                          cState);

                    //id = authStream.RemoteIdentity;
                    handler.Analysis(cState.Message.ToString());
                    Log.Send("Server says: " + cState.Message.ToString());

                    cState.Message.Remove(0, cState.Message.Length);
                    return;
                }
                else
                {
                    Log.Send("EndReadCallback(): Пришло 0 байт");
                    Close();
                    Start();
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
