using PocketSocket.Base;
using PocketSocket.Common;
using PocketSocket.Common.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Serialization;

namespace PocketSocket.Server
{
    public class SocketServer : SocketBase
    {
        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public override event EventHandler<ConnectedEventArgs> Connected;
        private SocketServer(IPHostEntry info) : base(info)
        {

            WorkSocket.Bind(new IPEndPoint(info.AddressList[0], 9000));

        }

        public static IPocketSocket Create()
        {
            return new SocketServer(Dns.GetHostEntry("localhost"));
        }

        public override void Start()
        {

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {

                WorkSocket.Listen(1);

                // Set the event to nonsignaled state.  
                allDone.Reset();

                // Start an asynchronous socket to listen for connections.  
                Console.WriteLine("Waiting for a connection...");
                WorkSocket.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    WorkSocket);

                // Wait until a connection is made before continuing.  
                allDone.WaitOne();
            }
            catch (Exception)
            {

                throw;
            }





        }

        private void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();


            Socket listener = (Socket)ar.AsyncState;
            WorkSocket = listener.EndAccept(ar);

            var args = new ConnectedEventArgs
            {
                HandlerContext = this
            };

            OnClientConnected(args);

            // Create the state object.  
            StateObject state = new StateObject
            {
                workSocket = this
            };


            WorkSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }


        protected virtual void OnClientConnected(ConnectedEventArgs e)
        {

            var docs = Store.GetAll();

            foreach (var item in docs.Select(x => new { x.RawValue.Key, Value = x.RawValue.Value.Replace("<EOF>", string.Empty) }))
            {
                var type = Resolver.GetMessageType(item.Value.ToString().Split('|').Last());
                var ser = new XmlSerializer(type);

                object responseObject;

                using (var stream = new StringReader(item.Value.Split('|')[1]))
                {
                    responseObject = ser.Deserialize(stream);

                }
                e.HandlerContext.Send(responseObject, Guid.Parse(item.Key));
            }

            Connected?.Invoke(e.HandlerContext, e);
        }

    }

}
