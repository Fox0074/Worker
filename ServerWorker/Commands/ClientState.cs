using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerWorker.Commands
{
    public class ClientState
    {
        private NetworkStream netStream = null;
        private CryptoStream cryptoStream = null;
        private TcpClient client = null;
        byte[] buffer = new byte[2048];
        StringBuilder message = null;
        ManualResetEvent waiter = new ManualResetEvent(false);

        public ClientState(NetworkStream a, TcpClient theClient)
        {
            netStream = a;
            client = theClient;
        }

        public ClientState(CryptoStream a, TcpClient theClient)
        {
            cryptoStream = a;
            client = theClient;
        }
        public TcpClient Client
        {
            get { return client; }
        }
        public NetworkStream NetStream
        {
            get { return netStream; }
        }
        public CryptoStream CryptoStream
        {
            get { return cryptoStream; }
        }
        public byte[] Buffer
        {
            get { return buffer; }
        }
        public StringBuilder Message
        {
            get
            {
                if (message == null)
                    message = new StringBuilder();
                return message;
            }
        }
        public ManualResetEvent Waiter
        {
            get
            {
                return waiter;
            }
        }
    }
}
