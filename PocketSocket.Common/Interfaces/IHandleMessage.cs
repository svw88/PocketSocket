namespace PocketSocket.Common.Interfaces
{
    public interface IHandleMessage<T>: IHandleMessage where T: ISocketMessage
    {
        void Handle(T message, StateObject handlerContext);

    }

    public interface IHandleMessage
    {
       
    }
}
