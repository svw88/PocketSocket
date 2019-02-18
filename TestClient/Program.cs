using PocketSocket.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new ClientService();
            var client = SocketClient.Create();
            
            
            client.Start();


            while (true)
            {
                Console.ReadLine();
                client.Send(new Request());
            }
        }
    }
}
