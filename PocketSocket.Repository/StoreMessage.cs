using System.Collections.Generic;

namespace PocketSocket.Repositories.LiteDb
{
    public class StoreMessage
    {
        public KeyValuePair<string, string> RawValue { get; set; }

        public StoreMessage(string key, string value)
        {
            RawValue = new KeyValuePair<string, string>(key, value);
        }


    }
}
