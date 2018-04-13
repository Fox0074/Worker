using System;
namespace ConsoleApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            UniservServer server = new UniservServer(2000);
            server.Start();
            Console.ReadLine();
        }
    }
}
