using PocketSocket.Common;
using PocketSocket.Common.Interfaces;
using PocketSocket.Repositories.LiteDb;
using PocketSocket.Repository;
using PocketSocket.Resolver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;

namespace PocketSocket.Base
{
    public abstract class SocketBase : IPocketSocket
    {
        internal protected ResolverService Resolver { get; set; }

        internal protected IMessageRepository<StoreMessage> Store { get; set; }

        internal protected Socket Listener = null;

        internal protected List<(string Id, Socket WorkSocket)> WorkSockets = new List<(string Id, Socket WorkSocket)>();

        public virtual event EventHandler<ConnectedEventArgs> Connected;

        public SocketBase(IPHostEntry info)
        {
            Listener = new Socket(info.AddressList[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Store = new LiteDbRepository();

            Resolver = new ResolverService();
        }

        public void SetContainer(IContainer container)
        {
            Resolver.Container = container;
        }

        public void Send(object message, int socketId, Guid? id = null)
        {
            string data = string.Empty;



            using (var stream = new StringWriter())
            {
                var ser = new XmlSerializer(message.GetType());

                ser.Serialize(stream, message);
                if (id == null)
                {


                    id = Guid.NewGuid();
                    data = $"{id}|{stream.ToString()}|{message.GetType().Name}<EOF>";

                    Store.Insert(WorkSockets[socketId].Id, id.Value, data);

                }
                else
                {
                    data = $"{id}|{stream.ToString()}|{message.GetType().Name}<EOF>";
                }

            }



            byte[] byteData = Encoding.ASCII.GetBytes(data);

            var state = new StateObject
            {
                SocketId = socketId,
                workSocket = this
            };
            // Begin sending the data to the remote device.  
            WorkSockets[socketId].WorkSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), state);
        }

        public void Send(string message, int socketId)
        {

            byte[] byteData = Encoding.ASCII.GetBytes(message);

            var state = new StateObject
            {
                SocketId = socketId,
                workSocket = this
            };
            // Begin sending the data to the remote device.  
            Listener.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), state);
        }

        internal void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                var state = (StateObject)ar.AsyncState;
                // Complete sending the data to the remote device.  
                int bytesSent = ((SocketBase)state.workSocket).WorkSockets[state.SocketId].WorkSocket.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);


            }
            catch (Exception e)
            {
                throw;
            }
        }

        protected void ReadCallback(IAsyncResult ar)
        {
            try
            {
                var content = string.Empty;

                // Retrieve the state object and the handler socket  
                // from the asynchronous state object.  
                var state = (StateObject)ar.AsyncState;

                // Read data from the client socket.   
                int bytesRead = ((SocketBase)state.workSocket).WorkSockets[state.SocketId].WorkSocket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));

                    content = state.sb.ToString();
                    if (content.IndexOf("<EOF>") > -1)
                    {

                        // All the data has been read from the   
                        // client. Display it on the console.  
                        Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                            content.Length, content);

                        while (content.IndexOf("<EOF>") > -1)
                        {

                            if (content.IndexOf("ConnectionId=") > -1 && content.IndexOf("<EOF>") > content.IndexOf("ConnectionId="))
                            {

                                var id = content.Substring(content.IndexOf("ConnectionId=") + 13, 36);

                                ((SocketBase)state.workSocket).WorkSockets[state.SocketId] = (id, ((SocketBase)state.workSocket).WorkSockets[state.SocketId].WorkSocket);

                                var args = new ConnectedEventArgs
                                {
                                    State = state
                                };

                                OnConnected(args);

                                content = content.Substring(content.IndexOf("<EOF>") + 5, content.Length - (content.IndexOf("<EOF>") + 5));
                                continue;
                            }

                            if (content.IndexOf("Id=") > -1 && content.IndexOf("<EOF>") > content.IndexOf("Id="))
                            {

                                var id = content.Substring(content.IndexOf("Id=") + 3, 36);

                                Store.Delete(WorkSockets[state.SocketId].Id, id);



                                content = content.Substring(content.IndexOf("<EOF>") + 5, content.Length - (content.IndexOf("<EOF>") + 5));
                                continue;
                            }
                            // Echo the data back to the client.  
                            CreateAndSendObject(content.Substring(0, content.IndexOf("<EOF>")), state);


                            byte[] byteData = Encoding.ASCII.GetBytes($"Id={content.Split('|')[0]}<EOF>");

                            content = content.Substring(content.IndexOf("<EOF>") + 5, content.Length - (content.IndexOf("<EOF>") + 5));

                            ((SocketBase)state.workSocket).WorkSockets[state.SocketId].WorkSocket.BeginSend(byteData, 0, byteData.Length, 0,
                                new AsyncCallback(SendCallback), state);


                        }

                        state.sb.Clear().Append(content);

                        ((SocketBase)state.workSocket).WorkSockets[state.SocketId].WorkSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);

                    }
                    else
                    {
                        // Not all data received. Get more.  
                        ((SocketBase)state.workSocket).WorkSockets[state.SocketId].WorkSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    }



                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        internal void CreateAndSendObject(string response, StateObject handler)
        {
            var message = response.Split('|')[1];

            var typeName = response.Split('|')[2];

            var type = Resolver.GetMessageType(typeName);

            if (type == null)
            {
                throw new Exception($"Response Type {typeName} Cannot Be Resolved Please Ensure The Relevant Type Exists");
            }

            var ser = new XmlSerializer(type);

            ISocketMessage responseObject;

            using (var stream = new StringReader(message))
            {
                responseObject = (ISocketMessage)ser.Deserialize(stream);

            }


            Resolver.Handle(type, responseObject, handler);


        }

        protected virtual void OnConnected(ConnectedEventArgs e)
        {

            var docs = Store.GetAll(WorkSockets[e.State.SocketId].Id);

            foreach (var item in docs.Select(x => new { x.RawValue.Key, Value = x.RawValue.Value.Replace("<EOF>", string.Empty) }))
            {
                var type = Resolver.GetMessageType(item.Value.ToString().Split('|').Last());
                var ser = new XmlSerializer(type);

                object responseObject;

                using (var stream = new StringReader(item.Value.Split('|')[1]))
                {
                    responseObject = ser.Deserialize(stream);

                }
                e.State.workSocket.Send(responseObject, e.State.SocketId, Guid.Parse(item.Key));
            }

            Connected?.Invoke(e.State, e);
        }

        public void Shutdown()
        {
            WorkSockets.ForEach(x => x.WorkSocket.Shutdown(SocketShutdown.Both));
            WorkSockets.ForEach(x => x.WorkSocket.Close());

        }

        public abstract void Start();
    }
}
