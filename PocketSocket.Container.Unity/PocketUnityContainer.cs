using System;
using System.Linq;
using PocketSocket.Common.Interfaces;
using Unity;

namespace PocketSocket.Container.Unity
{
    public class PocketUnityContainer : IContainer
    {
        private IUnityContainer Container { get; set; }

        public PocketUnityContainer(IUnityContainer container)
        {
            Container = container;
        }
        public void Handle(Type type, ISocketMessage message, IHandlerContext context)
        {
            var serviceType = typeof(IHandleMessage<>).MakeGenericType(type);

            var service = Container.Resolve(serviceType);

            serviceType.GetMethods().FirstOrDefault(x => x.GetParameters().Any(a => a.ParameterType == type)).Invoke(service, new object[] { message, context });


        }
    }
}
