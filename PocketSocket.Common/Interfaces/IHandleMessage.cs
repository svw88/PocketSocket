namespace PocketSocket.Common.Interfaces
{
    public interface IHandleMessage<T>: IHandleMessage where T: ISocketMessage
    {
        void Handle(T message, IHandlerContext handlerContext);

    }

    public interface IHandleMessage
    {
       
    }
}
