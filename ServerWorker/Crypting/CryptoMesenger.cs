using ServerWorker.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.Crypting
{
    public class CryptoMesenger
    {
        public CryptoStream CryptoStreamRead { get { return CryptoStreamRead; } private set { CryptoStreamRead = value; } }
        public NetworkStream NetStream { get { return NetStream; } private set { NetStream = value; } }

        private Rijndael RijndaelAlg;
        private IAsyncResult ars;
        private ClientState cState;
        public CryptoMesenger(NetworkStream netStream)
        {
            RijndaelAlg = Rijndael.Create();
            ICryptoTransform encryptor = RijndaelAlg.CreateEncryptor(RijndaelAlg.Key, RijndaelAlg.IV);
            CryptoStreamRead = new CryptoStream(netStream, encryptor, CryptoStreamMode.Read);

            cState = new ClientState(CryptoStreamRead,new TcpClient());//Хз "new TcpClient()"

            CryptoStreamRead.BeginRead(cState.Buffer, 0, cState.Buffer.Length,
                          new AsyncCallback(EndReadCallback),
                          cState);
        }


        public void CreateSecureStream()
        {
            //byte[] data = Encoding.UTF8.GetBytes(RijndaelAlg.Key.ToString());
            //NetStream.Write(data,0, data.Length);
        }

        public void GenerateSymmetricalKey()
        {
        }

        public void SetSymmetricalKey()
        {

        }

        public void SendSymmetricalKey()
        {

        }

        public void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            ars = CryptoStreamRead.BeginWrite(data, 0, data.Length,
                new AsyncCallback(EndWriteCallback),
                CryptoStreamRead);
            //ars.AsyncWaitHandle.WaitOne();
        }

        private void EndWriteCallback(IAsyncResult ars)
        {
            CryptoStream CryptoStream = (CryptoStream)ars.AsyncState;
            CryptoStream.EndWrite(ars);
        }

        public void EndReadCallback(IAsyncResult ar)
        {
            cState = (ClientState)ar.AsyncState;
            CryptoStream CryptoStream = (CryptoStream)cState.CryptoStream;

            int bytes = -1;

            try
            {
                bytes = CryptoStream.EndRead(ar);
                cState.Message.Append(Encoding.UTF8.GetChars(cState.Buffer, 0, bytes));
                if (bytes != 0)
                {
                    CryptoStream.BeginRead(cState.Buffer, 0, cState.Buffer.Length,
                          new AsyncCallback(EndReadCallback),
                          cState);

                    //Functions.AnalysisAnswer(cState.Message.ToString(), this);
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
                    //Log.Send("Обьект " + client.Client.RemoteEndPoint + " удален.");
                    //messangers.Remove(this);
                    //StopClientStream();
                    cState.Waiter.Set();
                }
                catch (Exception ex)
                {
                }
                return;
            }

            //id = authStream.RemoteIdentity;
            //Functions.AnalysisAnswer(cState.Message.ToString(), this);
            //Log.Send(id.Name + ": says " + cState.Message.ToString());
            cState.Waiter.Set();
        }
    }
}
