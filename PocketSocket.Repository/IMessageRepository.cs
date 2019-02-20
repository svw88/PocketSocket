using System;
using System.Collections.Generic;

namespace PocketSocket.Repository
{
    public interface IMessageRepository<T>
    {
        T Insert(string store, Guid id, string message);

        void Delete(string store, string Id);

        IEnumerable<T> GetAll(string store);
    }
}
