using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker
{
    public class Answer
    {
        public void AnalysisAnswer(string answer, NetworkStream stream)
        {
            switch (answer)
            {
                case "GetListUsers":
                    SendUserList(stream);
                    break;

                default:
                    Console.WriteLine("UnknownAnswer " + answer);
                    break;
            }
        }

        public void SendUserList(NetworkStream stream)
        {
            //stream.Write();
        }
    }
}
