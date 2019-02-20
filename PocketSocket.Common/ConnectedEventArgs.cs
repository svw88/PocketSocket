using System;

namespace PocketSocket.Common
{
    public class ConnectedEventArgs : EventArgs
    {        
        public StateObject State { get; set; }
    }
}
