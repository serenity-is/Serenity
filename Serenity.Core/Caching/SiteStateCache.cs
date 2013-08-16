using System;
using System.Collections.Generic;
using Serenity.Services;
using Newtonsoft.Json;

namespace Serenity.Data
{
    public class SiteStateCache
    {
        private static SiteStateCache _instance;
        private static Dictionary<string, Item> _stringItems;
        private static Dictionary<string, Item> _objectItems;

        static SiteStateCache()
        {
            _instance = new SiteStateCache();
            _stringItems = new Dictionary<string, Item>();
            _objectItems = new Dictionary<string, Item>();
        }

        public static string Get(string stateKey)
        {
            lock (_stringItems)
            {
                Item item;
                if (_stringItems.TryGetValue(stateKey, out item) &&
                    item.Expiration < DateTime.Now)
                    return (string)item.State;
            }

            var state = IoC.Resolve<ISiteStateService>().Load(stateKey);

            lock (_stringItems)
            {
                var item = new Item();
                item.Expiration = DateTime.Now.AddMinutes(5);
                item.State = state;
                _stringItems[stateKey] = item;
            }

            return state;
        }

        public static T Deserialize<T>(string stateKey) where T : class, new()
        {
            lock (_objectItems)
            {
                Item item;
                if (_objectItems.TryGetValue(stateKey, out item) &&
                    item.Expiration < DateTime.Now)
                    return (T)item.State;
            }

            var stateString = IoC.Resolve<ISiteStateService>().Load(stateKey);
            var state = JsonConvert.DeserializeObject<T>(stateString.TrimToNull() ?? "{}");

            lock (_objectItems)
            {
                var item = new Item();
                item.Expiration = DateTime.Now.AddMinutes(5);
                item.State = state;
                _objectItems[stateKey] = item;
            }

            return state;
        }

        private class Item
        {
            public DateTime Expiration;
            public object State;
        }
    }
}