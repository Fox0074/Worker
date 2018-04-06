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
using ServerWorker.Commands;
using ServerWorker.UserCard;

namespace ServerWorker
{
    public class Messenger
    {
        public static List<Messenger> messangers = new List<Messenger>();

        public NetworkStream stream;
        public NetworkStream authStream;

        public TcpClient client;
        public ClientLog clientLog = new ClientLog();
        public UserData setting = new UserData();
        public string key = "";
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

                cState.Message.Clear();

                authStream.BeginRead(cState.Buffer, 0, cState.Buffer.Length,
                       new AsyncCallback(EndReadCallback),
                       cState);
                cState.Waiter.WaitOne();
            }
            catch (Exception ex)
            {
                Log.Send("Exception Messenger.Process: " + ex.Message);
            }

        }

        public void SendMessage(string message)
        {
            Log.Send("Send "+message);
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
            NetworkStream authStream = (NetworkStream)cState.NetStream;
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
            //Log.Send(id.Name + " was authenticated using " + id.AuthenticationType);
            cState.Waiter.Set();

        }
        public void EndReadCallback(IAsyncResult ar)
        {
            ClientState cState = (ClientState)ar.AsyncState;
            TcpClient clientRequest = cState.Client;
            NetworkStream authStream = (NetworkStream)cState.NetStream;

            int bytes = -1;

            try
            {
                bytes = authStream.EndRead(ar);
                cState.Message.Append(Encoding.UTF8.GetChars(cState.Buffer, 0, bytes));
                if (bytes != 0)
                {
                    if (bytes == cState.Buffer.Length )
                    {
                        authStream.BeginRead(cState.Buffer, 0, cState.Buffer.Length,
                                  new AsyncCallback(EndReadCallback),
                                  cState);
             
                        return;
                    }
                  
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

            try
            {
                Functions.AnalysisAnswer(cState.Message.ToString(), this);
                Log.Send(client.Client.RemoteEndPoint.ToString() + ": says " + cState.Message.ToString());
                cState.Message.Clear();
                cState.Waiter.Set();


                authStream.BeginRead(cState.Buffer, 0, cState.Buffer.Length,
                              new AsyncCallback(EndReadCallback),
                              cState);

            }
            catch(Exception ex)
            {
                Log.Send(ex.Message);
            }
        }
        public void EndWriteCallback(IAsyncResult ars)
        {
            NetworkStream authStream = (NetworkStream)ars.AsyncState;

            authStream.EndWrite(ars);
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
