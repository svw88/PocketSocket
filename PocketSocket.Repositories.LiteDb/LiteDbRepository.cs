using LiteDB;
using PocketSocket.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PocketSocket.Repositories.LiteDb
{
    public class LiteDbRepository : IMessageRepository<StoreMessage>
    {
        private LiteDatabase Store = new LiteDatabase("PocketSocket.db");
        private Dictionary<string, LiteCollection<BsonDocument>> Collection { get; set; }
        public LiteDbRepository()
        {

            Collection = Store.GetCollectionNames().ToDictionary(x => x, x => Store.GetCollection<BsonDocument>(x));
        }


        public void Delete(string store, string id)
        {
            if (!Collection.ContainsKey(store))
            {
                Collection.Add(store, Store.GetCollection<BsonDocument>(store));
            }

            Collection[store].Delete(x => x.Keys.LastOrDefault() == id);
        }

        public IEnumerable<StoreMessage> GetAll(string store)
        {
            if (!Collection.ContainsKey(store))
            {
                Collection.Add(store, Store.GetCollection<BsonDocument>(store));
            }
            return Collection[store].FindAll().Select(x => new StoreMessage(x.RawValue.LastOrDefault().Key, x.RawValue.LastOrDefault().Value.AsString));
        }

        public StoreMessage Insert(string store, Guid id, string message)
        {
            if (!Collection.ContainsKey(store))
            {
                Collection.Add(store, Store.GetCollection<BsonDocument>(store));
            }

            var item = new BsonDocument();
            item.Add($"{id}", new BsonValue(message));
            Collection[store].Insert(item);

            return new StoreMessage($"{id}", message);
        }
    }
}
