using PocketSocket.Common.Interfaces;
using PocketSocket.Container.Default;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PocketSocket.Resolver
{
    public class ResolverService
    {
        public IContainer Container;
        private IEnumerable<Type> MessageTypes { get; set; }
        public ResolverService()
        {
            Container = new DefaultContainer();
            MessageTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => x.GetInterface(typeof(ISocketMessage).Name) != null);
        }

        public Type GetMessageType(string typeName)
        {
            return MessageTypes.FirstOrDefault(x => x.Name == typeName);
        }

        public void Handle(Type type, ISocketMessage message, IHandlerContext context)
        {
            Container.Handle(type, message, context);
            
        }


    }
}
