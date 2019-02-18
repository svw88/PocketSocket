using PocketSocket.Common.Interfaces;
using System;

namespace PocketSocket.Common
{
    public class ConnectedEventArgs : EventArgs
    {
        public IHandlerContext HandlerContext { get; set; }
    }
}
