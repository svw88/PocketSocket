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
            var client1 = SocketClient.Create(new Guid("D3D67ED9-952D-4EC2-90F5-C6C09BC11E6F"));
            var client2 = SocketClient.Create(new Guid("3D6C3EE3-EFEC-4A07-8D73-34C6FA078F06"));

            client1.Start();
            client2.Start();

            while (true)
            {
                Console.ReadLine();
                client1.Send(new Request(), 0);
                client2.Send(new Request(), 0);
            }
        }
    }
}
