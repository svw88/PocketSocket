using System;
using System.Collections.Generic;

namespace PocketSocket.Repository
{
    public interface IMessageRepository<T>
    {
        T Insert(Guid id, string message);

        void Delete(string Id);

        IEnumerable<T> GetAll();
    }
}
