using System;

namespace PocketSocket.Common.Interfaces
{
    public interface IPocketSocket: IHandlerContext
    {
        void Start();

        void Shutdown();

        event EventHandler<ConnectedEventArgs> Connected;

    }

   
}
