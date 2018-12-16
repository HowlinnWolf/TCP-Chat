using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ChatServer
{
    class ServerApp
    {
        static TcpListener tcpListener;
        List<ClientApp> clients = new List<ClientApp>();

        protected internal void AddConnection(ClientApp clientApp)
        {
            clients.Add(clientApp);
        }
        protected internal void RemoveConnection(string id)
        {
            ClientApp client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                clients.Remove(client);
        }
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientApp clientObject = new ClientApp(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }
        
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id)
                {
                    clients[i].Stream.Write(data, 0, data.Length);
                }
            }
        }
        protected internal void Disconnect()
        {
            tcpListener.Stop();

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            Environment.Exit(0);
        }
    }
}
