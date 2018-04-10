using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Service
{
    public class ClientState
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
}
