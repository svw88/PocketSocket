using PocketSocket.Common;
using PocketSocket.Extensions.Unity;
using PocketSocket.Server;
using System;
using TestClient;
using Unity;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new ServerService();
            var server = SocketServer.Create();

            server.Connected += Connected;

            server.Start();
            var container = new UnityContainer();
            server.UseUnityContainer(container);

            Console.ReadKey();

            server.Shutdown();
        }

        private static void Connected(object sender, ConnectedEventArgs e)
        {
            Console.WriteLine("Client Connected");
        }
    }
}
