using System;
using System.Collections.Generic;
using Serenity.Services;

namespace Serenity.Data
{
    public class LocalUserStateCache
    {
        private static LocalUserStateCache _instance;
        private static Dictionary<string, Item> _stringItems;
        private static Dictionary<string, Item> _objectItems;

        static LocalUserStateCache()
        {
            _instance = new LocalUserStateCache();
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

            var state = IoC.Resolve<ILocalUserStateService>().Load(stateKey);

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

            var state = Json.ParseTolerant<T>(IoC.Resolve<ILocalUserStateService>().Load(stateKey).TrimToNull() ?? "{}"); 

            lock (_objectItems)
            {
                var item = new Item();
                item.Expiration = DateTime.Now.AddMinutes(5);
                item.State = state;
                _objectItems[stateKey] = item;
            }

            return state;
        }

        public static void Remove(string stateKey)
        {
            lock (_objectItems)
            {
                _objectItems.Remove(stateKey);
                _stringItems.Remove(stateKey);
            }
        }

        private class Item
        {
            public DateTime Expiration;
            public object State;
        }
    }
}