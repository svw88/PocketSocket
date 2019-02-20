using PocketSocket.Base;
using PocketSocket.Common;
using PocketSocket.Common.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Serialization;

namespace PocketSocket.Client
{
    public class SocketClient : SocketBase
    {
        internal protected Guid ClientId { get; set; }
        internal protected IPEndPoint RemoteEndpointAddress { get; set; }

        internal protected const int port = 11000;

        // ManualResetEvent instances signal completion.  
        internal protected static ManualResetEvent connectDone =
            new ManualResetEvent(false);

        // The response from the remote device.  
        internal protected static string response = string.Empty;

        public override event EventHandler<ConnectedEventArgs> Connected;

        internal protected SocketClient(IPHostEntry info, Guid clientId) : base(info)
        {
            ClientId = clientId;
            RemoteEndpointAddress = new IPEndPoint(info.AddressList[0], 9000);
            WorkSockets.Add(("Store", Listener));
        }

        public override void Start()
        {
            // Connect to a remote device.  
            try
            {
                // Connect to the remote endpoint.  
                WorkSockets.FirstOrDefault().WorkSocket.BeginConnect(RemoteEndpointAddress,
                    new AsyncCallback(ConnectCallback), this);
                connectDone.WaitOne();

            }
            catch (Exception)
            {
                throw;
            }
        }

        internal protected void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                var listener = (SocketClient)ar.AsyncState;
                // Complete the connection.  
                listener.WorkSockets.FirstOrDefault().WorkSocket.EndConnect(ar);

              

                Console.WriteLine("Socket connected to {0}",
                    RemoteEndpointAddress.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();

             
                // Begin sending the data to the remote device.  
                Send($"ConnectionId={ClientId}<EOF>", 0);

                StateObject state = new StateObject
                {
                    workSocket = this
                };

                // Begin receiving the data from the remote device.  
                WorkSockets.FirstOrDefault().WorkSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                var docs = Store.GetAll(WorkSockets.FirstOrDefault().Id);

                foreach (var item in docs.Select(x => new { x.RawValue.Key, Value = x.RawValue.Value.Replace("<EOF>", string.Empty) }))
                {
                    var type = Resolver.GetMessageType(item.Value.ToString().Split('|').Last());
                    var ser = new XmlSerializer(type);

                    object responseObject;

                    using (var stream = new StringReader(item.Value.Split('|')[1]))
                    {
                        responseObject = ser.Deserialize(stream);

                    }


                    Send(responseObject, 0, Guid.Parse(item.Key));
                }


                var args = new ConnectedEventArgs
                {
                    State = state
                };

                Connected?.Invoke(args.State, args);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static IPocketSocket Create(Guid clientId)
        {
            return new SocketClient(Dns.GetHostEntry("localhost"), clientId);
        }



    }

}
