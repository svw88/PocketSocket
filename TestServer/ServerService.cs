using PocketSocket.Common.Interfaces;
using System;
using TestCommon;

namespace TestClient
{
    public class ServerService : IHandleMessage<Request>
    {
        public void Handle(Request message, IHandlerContext handlerContext)
        {
            handlerContext.Send(new Response());
        }
    }
}
