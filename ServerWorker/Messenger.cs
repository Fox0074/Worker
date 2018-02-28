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

        StringBuilder builder;
        public TcpClient client;
        public ClientLog clientLog = new ClientLog();
        public ClientSetting setting = new ClientSetting();
        byte[] data;


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

                authStream.BeginAuthenticateAsServer(
                    new AsyncCallback(EndAuthenticateCallback),
                    cState
                    );

                cState.Waiter.WaitOne();
                cState.Waiter.Reset();

                authStream.BeginRead(cState.Buffer, 0, cState.Buffer.Length,
                       new AsyncCallback(EndReadCallback),
                       cState);

                cState.Waiter.WaitOne();



                data = new byte[64];
                while (true)
                {
                        builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = authStream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));

                            if (bytes == 0)
                            {
                                Log.Send(client.Client.RemoteEndPoint + " Пришло 0 байт, клиент отключен");
                                messangers.Remove(this);
                                stream.Close();
                                authStream.Close();
                                client.Close();
                                return;
                            }

                        }
                        while (stream.DataAvailable);

                        string message = builder.ToString();

                        Log.Send("Получено " + client.Client.RemoteEndPoint + " : " + message);

                        Functions.AnalysisAnswer(message, this);                  
                }
            }
            catch (Exception ex)
            {
                Log.Send(ex.Message);
            }
            finally
            {
                authStream.Close();
                client.Close();
            }
        }
        public static void EndAuthenticateCallback(IAsyncResult ar)
        {
            // Get the saved data.
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
                Log.Send("Closing connection.");
                cState.Waiter.Set();
                return;
            }
            // Display properties of the authenticated client.
            IIdentity id = authStream.RemoteIdentity;
            Log.Send(id.Name+" was authenticated using " + id.AuthenticationType);
            cState.Waiter.Set();

        }
        public static void EndReadCallback(IAsyncResult ar)
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
            IIdentity id = authStream.RemoteIdentity;
            Log.Send(id.Name+ " says "+ cState.Message.ToString());
            cState.Waiter.Set();
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
            List<string> log = new List<string>();
            byte[] data = Encoding.Unicode.GetBytes("GetLogList");
            authStream.Write(data, 0, data.Length);

        }

        public void Update()
        {
            string message = "DownlUpd";
            data = Encoding.Unicode.GetBytes(message);
            authStream.Write(data, 0, data.Length);
        }
        //Ну я хз
        public void CheckLostClient()
        {
            try
            {
                string message = "TestConnect";
                data = Encoding.Unicode.GetBytes(message);
                authStream.Write(data, 0, data.Length);
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
            authStream.Close();
        }

            private RSACryptoServiceProvider m_Rsa;
        private RSAParameters m_ExternKey;
        private RSAParameters m_InternKey;

        public void CryptoRsa()
        {
            m_Rsa = new RSACryptoServiceProvider(512);
            m_InternKey = m_Rsa.ExportParameters(true);
        }
    }
}
