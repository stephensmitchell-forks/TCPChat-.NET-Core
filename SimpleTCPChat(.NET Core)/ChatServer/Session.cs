using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace ChatServer
{
    public class Session
    {
        private Server server;
        private int SessionID;
        private TcpClient client;
        private BinaryReader reader;
        private BinaryWriter writer;
        public Session(TcpClient client, BinaryReader reader, BinaryWriter writer, Server server, int SessionID)
        {
            this.client = client;
            this.reader = reader;
            this.writer = writer;
            this.server = server;
            this.SessionID = SessionID;
        }

        public int GetSessionID()
        {
            return SessionID;
        }

        public void StartSession()
        {
            Thread thread = new Thread(HandleSession);
            thread.Start();
        }

        private void HandleSession()
        {
            Console.WriteLine("Client {0} Session...", SessionID);
            writer.Write("You are connected.");
            while (true)
            {
                try
                {
                    string message = reader.ReadString();
                    BroadcastMessage(message);

                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                    server.GetClients().Remove(this);
                    Console.WriteLine("Session has been closed");
                    server.NotifyAboutClose(SessionID);
                    break;
                }
            }
        }
        private void BroadcastMessage(string message)
        {
            server.BroadcastMessage(SessionID, message);
        }
        public void SendMessage(string message)
        {
            writer.Write(message);
        }
    }
}
