using PocketSocket.Base;
using PocketSocket.Common.Interfaces;
using PocketSocket.Container.Unity;
using System;
using System.Linq;
using Unity;

namespace PocketSocket.Extensions.Unity
{
    public static class UnityExtensions
    {
        public static IPocketSocket UseUnityContainer(this IPocketSocket pocketSocket, IUnityContainer container)
        {
            container.RegisterServices();
            var socketContainer = new PocketUnityContainer(container);
            ((SocketBase)pocketSocket).SetContainer(socketContainer);

            return pocketSocket;
        }

        private static void RegisterServices(this IUnityContainer container)
        {
            var serviceTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(s => s.GetInterface(typeof(IHandleMessage).Name) != null).SelectMany(a => a.GetInterfaces().Where(s => s.IsGenericType).Select(s => new { Interface = s, Concrete = a }));

            foreach (var type in serviceTypes)
            {
                container.RegisterType(type.Interface, type.Concrete);
            }
        }
    }
}
