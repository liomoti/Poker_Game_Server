using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Poker_Server_v1
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(8001);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;
            GameDealer gamed = new GameDealer();
            serverSocket.Start();
            Console.WriteLine(" >> " + "Server Started");
            Console.WriteLine(" >> " + "Server IP: "+ GetLocalIP());
            Console.WriteLine(" >> " + "Waiting for 2 Clients...");
            counter = 0;
            while (true)
            {
                counter++;
                clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine(" >> " + "Client No:" + Convert.ToString(counter) + " started!");
                HandleClient client = new HandleClient();
                client.startClient(clientSocket, Convert.ToString(counter),gamed);
                if(counter==2)
                    Console.WriteLine("2 Clients has connected to the server.");                
            }
            
            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine(" >> " + "exit");
            Console.ReadLine();
        }

        private static string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "127.0.0.1";
        }
    }
}
