using PocketSocket.Common.Interfaces;
using System.Text;

namespace PocketSocket.Base
{
    // State object for receiving data from remote device.  
    public class StateObject
    {
        // Client socket.  
        public IHandlerContext workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
}
