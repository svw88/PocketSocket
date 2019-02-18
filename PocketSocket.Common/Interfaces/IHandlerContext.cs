using System;
using System.Net.Sockets;

namespace PocketSocket.Common.Interfaces
{
    public interface IHandlerContext
    {
        void Send(object request, Guid? id = null);
    }
}
