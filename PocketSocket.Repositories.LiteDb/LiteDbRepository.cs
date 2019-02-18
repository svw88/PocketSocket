using LiteDB;
using PocketSocket.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PocketSocket.Repositories.LiteDb
{
    public class LiteDbRepository : IMessageRepository<StoreMessage>
    {
        private LiteCollection<BsonDocument> Store { get; set; }
        public LiteDbRepository()
        {
            Store = new LiteDatabase("PocketSocket.db").GetCollection<BsonDocument>("Store");
        }
        public void Delete(string id)
        {
            Store.Delete(x => x.Keys.LastOrDefault() == id);
        }

        public IEnumerable<StoreMessage> GetAll()
        {
            return Store.FindAll().Select(x => new StoreMessage(x.Values.FirstOrDefault(),x.Values.LastOrDefault()));
        }

        public StoreMessage Insert(Guid id, string message)
        {
            var item = new BsonDocument();
            item.Add($"{id}", new BsonValue(message));
            Store.Insert(item);

            return new StoreMessage($"{id}", message);
        }
    }
}
