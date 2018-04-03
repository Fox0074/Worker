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
using ServerWorker.UserCard;

namespace ServerWorker
{
    public class ConnectedServers
    {
        public static List<ConnectedServers> servers = new List<ConnectedServers>();

        public NegotiateStream authStream;

        public TcpClient client;
        public ClientLog clientLog = new ClientLog();
        public UserData setting = new UserData();

        ClientState cState;
        IIdentity id;
        IAsyncResult ars;

        public ConnectedServers(NegotiateStream authStream, TcpClient client)
        {
            cState = new ClientState(authStream, client);

            authStream.BeginRead(cState.Buffer, 0, cState.Buffer.Length,
                      new AsyncCallback(EndReadCallback),
                      cState);

            servers.Add(this);
            //cState.Waiter.WaitOne();

        }

        public void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            ars = authStream.BeginWrite(data, 0, data.Length,
                new AsyncCallback(EndWriteCallback),
                authStream);
            ars.AsyncWaitHandle.WaitOne();
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

                    Log.Send("Server message: " + cState.Message.ToString());
                    //Functions.AnalysisAnswer(cState.Message.ToString(), this);
                    cState.Message.Clear();
                    return;
                }
            }
            catch (Exception e)
            {
                try
                {
                    Log.Send("Server message exception:");
                    Log.Send(e.Message);
                    Log.Send("Обьект " + client.Client.RemoteEndPoint + " удален.");
                    servers.Remove(this);
                    StopClientStream();
                    //if (Program.form1.InvokeRequired) Program.form1.BeginInvoke(new Action(() => { Program.form1.WrileClientsInList(); }));
                    //else Program.form1.WrileClientsInList();
                    cState.Waiter.Set();
                }
                catch (Exception ex)
                {
                }
                return;
            }

            id = authStream.RemoteIdentity;
            //Functions.AnalysisAnswer(cState.Message.ToString(), this);
            Log.Send(id.Name + " says " + cState.Message.ToString());
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

        public void StopClientStream()
        {
            try
            {
                authStream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Log.Send("Cant close client " + ex.Message);
            }
        }
    }
}
