using System;

namespace PocketSocket.Common.Interfaces
{
    public interface IContainer
    {
        void Handle(Type type, ISocketMessage message, IHandlerContext context);
    }
}
