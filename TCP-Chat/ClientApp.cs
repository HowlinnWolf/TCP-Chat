using System;
using System.Text;
using System.Net.Sockets;
using System.Globalization;

namespace ChatServer
{
    public class ClientApp
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerApp server;

        internal ClientApp(TcpClient tcpClient, ServerApp serverApp)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverApp;
            serverApp.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                string message = GetMessage();
                userName = message;

                message = userName + " вошел в чат";
                server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message);
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("({0}) {1}: {2}", DateTime.Now.ToString("HH:mm:ss"), userName, message);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        message = String.Format("{0} покинул чат", userName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }
        
        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }
        
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
