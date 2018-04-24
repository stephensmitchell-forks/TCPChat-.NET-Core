using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    public class Server
    {
        private TcpListener listener;
        private List<Session> clients;
        public Server(string IpAddress,int port)
        {
            listener = new TcpListener(IPAddress.Parse(IpAddress), port);
            clients = new List<Session>();
        }

        public TcpListener GetListener()
        {
            return this.listener;
        }
        public List<Session> GetClients()
        {
            return this.clients;
        }

        public void NotifyAboutClose(int sessionID)
        {
            Console.WriteLine($"Client {sessionID} has been disconnected.");
            BroadcastMessage(sessionID, " has been disconnected.");
        }

        public void BroadcastMessage(int SessionID, string message)
        {
            foreach (Session session in clients)
            {
                if (session.GetSessionID() != SessionID)
                {
                    session.SendMessage($"[Client{SessionID}]:{message}");
                }
            }
        }

        public static void Main(string[] args)
        {
            Console.Write("IP Address: ");
            string ipAddress = Console.ReadLine();
            Console.Write("Port: ");
            int port = Int32.Parse(Console.ReadLine());
            Server server = new Server(ipAddress,port);
            server.GetListener().Start();
            Console.WriteLine("Server is listening for new clients...");
            while (true)
            {
                TcpClient newClient = server.GetListener().AcceptTcpClient();
                BinaryReader reader = new BinaryReader(newClient.GetStream());
                BinaryWriter writer = new BinaryWriter(newClient.GetStream());
                Session newSession = new Session(newClient, reader, writer, server, server.GetClients().Count);
                newSession.StartSession();
                server.GetClients().Add(newSession);
                Console.WriteLine("Connected clients: {0}", server.GetClients().Count);
            }
        }
    }
}
