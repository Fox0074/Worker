﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography;
using System.Net.Security;
using System.Security.Principal;
using System.Security.Authentication;

namespace ServerWorker
{
    public class Messenger
    {
        public static List<Messenger> messangers = new List<Messenger>();

        public NetworkStream stream;
        public NetworkStream authStream;

        public TcpClient client;
        public ClientLog clientLog = new ClientLog();
        public ClientSetting setting = new ClientSetting();
        IIdentity id;
        IAsyncResult ars;


        public void Process(TcpClient client)
        {
            messangers.Add(this);
            this.client = client;
            stream = null;
            ClientState cState;

            stream = client.GetStream();
            authStream = stream;

            try
            {
                cState = new ClientState(authStream, client);


                if (Program.form1.InvokeRequired) Program.form1.BeginInvoke(new Action(() => { Program.form1.WrileClientsInList(); }));
                else Program.form1.WrileClientsInList();

                authStream.BeginRead(cState.Buffer, 0, cState.Buffer.Length,
                       new AsyncCallback(EndReadCallback),
                       cState);
                cState.Waiter.WaitOne();
            }
            catch (Exception ex)
            {
                Log.Send("Exception Messenger.Process: " + ex.Message);
            }
            Console.WriteLine("test");

        }

        public void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            ars = authStream.BeginWrite(data, 0, data.Length,
                new AsyncCallback(EndWriteCallback),
                authStream);
            ars.AsyncWaitHandle.WaitOne();
        }

        public void EndAuthenticateCallback(IAsyncResult ar)
        {
            ClientState cState = (ClientState)ar.AsyncState;
            TcpClient clientRequest = cState.Client;
            NetworkStream authStream = (NetworkStream)cState.AuthenticatedStream;
            //Log.Send("Ending authentication.");

            try
            {
                //authStream.EndAuthenticateAsServer(ar);
            }
            catch (AuthenticationException e)
            {
                Log.Send(e.Message);
                Log.Send("Authentication failed - closing connection.");
                cState.Waiter.Set();
                return;
            }
            catch (Exception e)
            {
                Log.Send(e.Message);
                Log.Send(client.Client.RemoteEndPoint + "Closing connection.");
                cState.Waiter.Set();
                messangers.Remove(this);
                return;
            }
           // id = authStream.RemoteIdentity;
            Log.Send(id.Name + " was authenticated using " + id.AuthenticationType);
            cState.Waiter.Set();

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

                    Functions.AnalysisAnswer(cState.Message.ToString(), this);
                    cState.Message.Clear();
                    return;
                }
            }
            catch (Exception e)
            {
                try
                {
                    Log.Send("Client message exception:");
                    Log.Send(e.Message);
                    Log.Send("Обьект " + client.Client.RemoteEndPoint + " удален.");
                    messangers.Remove(this);
                    StopClientStream();
                    if (Program.form1.InvokeRequired) Program.form1.BeginInvoke(new Action(() => { Program.form1.WrileClientsInList(); }));
                    else Program.form1.WrileClientsInList();
                    cState.Waiter.Set();
                }
                catch (Exception ex)
                {
                }
                return;
            }

            //id = authStream.RemoteIdentity;
            Functions.AnalysisAnswer(cState.Message.ToString(), this);
            Log.Send(id.Name + ": says " + cState.Message.ToString());
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
        public void RequestLog()
        {
            Log.Send("Запрос логов у клиента " + client.Client.RemoteEndPoint);
            SendMessage("GetLogList");
        }

        public void Update()
        {
            string message = "DownlUpd";
            SendMessage(message);
        }

        public static void UpdateAll()
        {
            List<Messenger> remover = new List<Messenger>();

            foreach (Messenger meseng in messangers)
            {
                try
                {
                    meseng.Update();
                }
                catch
                {
                    remover.Add(meseng);
                }
            }

            foreach (Messenger meseng in remover)
            {
                messangers.Remove(meseng);
            }

        }

        public void StopClientStream()
        {
            try
            {
                authStream.Close();
                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Log.Send("Cant close client " + ex.Message);
            }
        }
        public void EndReadClient()
        {
            authStream.EndRead(ars);
            authStream.EndWrite(ars);
        }
    }
}
