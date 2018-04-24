using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace ChatClient
{
    public class Client
    {
        private TcpClient client;
        private BinaryWriter writer;
        private BinaryReader reader;
        public Client()
        {
            client = new TcpClient();
        }
        public TcpClient GetClient()
        {
            return this.client;
        }
        public void Connect(string IPAddressString, int port)
        {
            try
            {
                client.Connect(IPAddressString, port);
                writer = new BinaryWriter(client.GetStream());
                reader = new BinaryReader(client.GetStream());

            }
            catch (Exception)
            {
                throw new Exception("Cannot connect to the server");
            }
        }
        public void SendMessage(string message)
        {
            writer.Write(message);
        }
        public string ReadStringFromServer()
        {
            return reader.ReadString();
        }
        private void ListenForMessages()
        {
            while (true)
            {
                try
                {
                    string message = reader.ReadString();
                    Console.WriteLine(message);
                }
                catch (Exception)
                {
                    Console.WriteLine("Connection lost! press any key to continue");
                    break;
                }
            }
        }

        public void HandleCommunication()
        {
            Thread listeningThread = new Thread(ListenForMessages);
            listeningThread.Start();
           
            while (true)
            {
                try
                {
                    string message = Console.ReadLine();
                    SendMessage(message);
                }
                catch (Exception)
                {
                    Console.WriteLine("Connection lost, please press any key to close the application");
                    Console.ReadLine();
                    break;
                }
            }
        }

        public static void Main(string[] args)
        {
            Client cl = new Client();
            Console.Write("Server IP Address: ");
            string IpAddress = Console.ReadLine();
            Console.Write("Server Listening Port: ");
            int port = Int32.Parse(Console.ReadLine());
            try
            {
                cl.Connect(IpAddress,port);
                Console.WriteLine(cl.ReadStringFromServer());
            }
            catch (Exception)
            {
                Console.WriteLine("Connection lost, press any key to close.");
                Console.ReadLine();
                return;
            }
            cl.HandleCommunication();
        }
    }
}
