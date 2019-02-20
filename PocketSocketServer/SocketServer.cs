using PocketSocket.Base;
using PocketSocket.Common;
using PocketSocket.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Serialization;

namespace PocketSocket.Server
{
    public class SocketServer : SocketBase
    {
        public List<int> SocketIds { get; set; }
        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private SocketServer(IPHostEntry info) : base(info)
        {

            Listener.Bind(new IPEndPoint(info.AddressList[0], 9000));
            Listener.Listen(2);
            SocketIds = new List<int>() { 0 };


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



                // Set the event to nonsignaled state.  
                allDone.Reset();
                StateObject state = new StateObject
                {
                    workSocket = this
                };
                // Start an asynchronous socket to listen for connections.  
                Console.WriteLine("Waiting for a connection...");
                Listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    state);

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


            var listener = (StateObject)ar.AsyncState;
          
            WorkSockets.Add(("Store", ((SocketServer)listener.workSocket).Listener.EndAccept(ar)));

            // Create the state object.  
            StateObject state = new StateObject
            {
                SocketId = SocketIds.LastOrDefault(),
                workSocket = this
            };
          

            WorkSockets[SocketIds.LastOrDefault()].WorkSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
            SocketIds.Add(SocketIds.LastOrDefault() + 1);
            Listener.BeginAccept(
                   new AsyncCallback(AcceptCallback),
                   state);
        }


        

    }

}
