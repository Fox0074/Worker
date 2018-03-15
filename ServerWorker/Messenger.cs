using System;
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
        public NegotiateStream authStream;

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
            authStream = new NegotiateStream(stream, false);

            try
            {
                cState = new ClientState(authStream, client);

                ars = authStream.BeginAuthenticateAsServer(
                               new AsyncCallback(EndAuthenticateCallback),
                               cState
                               );
                cState.Waiter.WaitOne();
                cState.Waiter.Reset();

                if (Program.form1.InvokeRequired) Program.form1.BeginInvoke(new Action(() => { Program.form1.WrileClientsInList(); }));
                else Program.form1.WrileClientsInList();

                authStream.BeginRead(cState.Buffer, 0, cState.Buffer.Length,
                       new AsyncCallback(EndReadCallback),
                       cState);
                cState.Waiter.WaitOne();
            }
            catch (Exception ex)
            {
                Log.Send("Messenger.Process() " + ex.Message);
            } 

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
            NegotiateStream authStream = (NegotiateStream)cState.AuthenticatedStream;
            Log.Send("Ending authentication.");

            try
            {
                authStream.EndAuthenticateAsServer(ar);
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
            id = authStream.RemoteIdentity;
            Log.Send(id.Name+" was authenticated using " + id.AuthenticationType);
            cState.Waiter.Set();

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

                    Functions.AnalysisAnswer(cState.Message.ToString(),this);
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

            id = authStream.RemoteIdentity;
            Functions.AnalysisAnswer(cState.Message.ToString(), this);
            Log.Send(id.Name+ " says "+ cState.Message.ToString());
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
        //Ну я хз
        public void CheckLostClient()
        {
            try
            {
                string message = "TestConnect";
                SendMessage(message);
            }
            catch
            {
                messangers.Remove(this);
            }

                
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
    }
}
