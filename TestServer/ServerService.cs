using PocketSocket.Common;
using PocketSocket.Common.Interfaces;
using System;
using TestCommon;

namespace TestClient
{
    public class ServerService : IHandleMessage<Request>
    {
        public void Handle(Request message, StateObject handlerContext)
        {
            Console.WriteLine("Message REceived");
            handlerContext.Send(new Response());
        }
    }
}
