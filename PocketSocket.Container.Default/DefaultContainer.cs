using PocketSocket.Common.Interfaces;
using System;
using System.Linq;

namespace PocketSocket.Container.Default
{
    public class DefaultContainer : IContainer
    {
        public void Handle(Type type, ISocketMessage message, IHandlerContext context)
        {
            var serviceType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(s => s.GetInterface(typeof(IHandleMessage).Name) != null).FirstOrDefault(a => a.GetInterfaces().Any(s => s.IsGenericType && s.GenericTypeArguments[0] == type));
            var service = (IHandleMessage)Activator.CreateInstance(serviceType.Assembly.FullName, serviceType.FullName).Unwrap();

            serviceType.GetMethods().FirstOrDefault(x => x.GetParameters().Any(a => a.ParameterType == type)).Invoke(service, new object[] { message, context });
        }
    }
}
