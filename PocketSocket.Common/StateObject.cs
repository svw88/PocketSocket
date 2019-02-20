using PocketSocket.Common.Interfaces;
using System;
using System.Text;

namespace PocketSocket.Common
{
    // State object for receiving data from remote device.  
    public class StateObject
    {
        public int SocketId { get; set; }
        // Client socket.  
        public IHandlerContext workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();

        public void Send(ISocketMessage message, Guid? id = null)
        {
            workSocket.Send(message, SocketId, id);
        }
    }
}
