using System;
using System.Threading;

namespace ChatServer
{
    class Program
    {
        static ServerApp server;
        static Thread listenThread;
        static void Main(string[] args)
        {
            try
            {
                server = new ServerApp();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start();
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
