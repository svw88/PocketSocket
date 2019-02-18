using PocketSocket.Common.Interfaces;
using System;
using TestCommon;

namespace TestClient
{
    public class ClientService : IHandleMessage<Response>
    {
        public void Handle(Response message, IHandlerContext handlerContext)
        {
            Console.WriteLine("I Have Recieved Data");
        }
    }
}
