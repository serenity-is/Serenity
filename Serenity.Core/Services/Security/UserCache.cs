using Serenity.Data;
using System;
using System.Collections.Generic;

namespace Serenity
{
    public interface IUserRetrieveService
    {
        IUserDefinition ById(Int64 id);
        IUserDefinition ByUsername(string username);
    }

    public interface IUserAuthenticationService
    {
        bool Validate(string username, string password);
    }

    //public class UserCache
    //{
    //    public const int MaxItems = 100000;
    //    public const int MakeSpaceBy = 100;

    //    private static object _sync;
    //    private static LruCache<Item> _lruCache;
    //    private static Dictionary<Int64, Item> _byId;
    //    private static Dictionary<string, Item> _byUsername;
    //    //private static CachedTableVersionChecker _versionChecker;

    //    static UserCache()
    //    {
    //        _sync = new Object();
    //        _byId = new Dictionary<Int64, Item>();
    //        _byUsername = new Dictionary<string, Item>(StringComparer.CurrentCultureIgnoreCase);
    //        _lruCache = new LruCache<Item>(MaxItems, MakeSpaceBy, RemoveItemCallback);
    //        //_versionChecker = new CachedTableVersionChecker(UserRow.TableName, CommonCacheDelays.Medium);
    //    }

    //    //private static void CheckForChanges()
    //    //{
    //    //    lock (_sync)
    //    //    {
    //    //        if ((_byId.Count > 0 ||
    //    //             _byUsername.Count > 0) &&
    //    //            _versionChecker.IsChanged())
    //    //        {
    //    //            RemoveAll();
    //    //        }
    //    //    }
    //    //}

    //    public static IUserDefinition ById(int userId)
    //    {
    //        if (userId == -1)
    //            return null;

    //        Item item;
    //        lock (_sync)
    //        {
    //            //CheckForChanges();

    //            if (_byId.TryGetValue(userId, out item))
    //            {
    //                _lruCache.Use(item);
    //                return item.isInvalid ? null : item.user;
    //            }
    //        }

    //        var row = IoC.Resolve<IUserRetrieveService>().ById(userId);
    //        item = row == null ? new Item { isInvalid = true, user = new UserDefinition { UserId = userId } } : new Item { user = row };

    //        lock (_sync)
    //        {
    //            Item old;
    //            if (_byId.TryGetValue(userId, out old))
    //                _lruCache.Remove(old);

    //            _lruCache.Use(item);
    //            _byId[userId] = item;

    //            if (item.isInvalid)
    //                return null;

    //            _byUsername[item.user.Username] = item;
    //        }

    //        return item.user;
    //    }

    //    public static void RemoveById(int userId)
    //    {
    //        if (userId == -1)
    //            return;

    //        Item item;
    //        lock (_sync)
    //        {
    //            if (_byId.TryGetValue(userId, out item))
    //                _lruCache.Remove(item);
    //        }
    //    }

    //    public static void RemoveByUsername(string username)
    //    {
    //        if (username == null)
    //            return;

    //        Item item;
    //        lock (_sync)
    //        {
    //            if (_byUsername.TryGetValue(username, out item))
    //                _lruCache.Remove(item);
    //        }
    //    }

    //    public static void RemoveAll()
    //    {
    //        lock (_sync)
    //        {
    //            _lruCache.Clear(false);
    //            _byId.Clear();
    //            _byUsername.Clear();
    //            //_versionChecker.LoadedVersion = _versionChecker.CurrentVersion;
    //        }
    //    }

    //    public static IUserDefinition ByUsername(string username)
    //    {
    //        if (username == null)
    //            return null;

    //        Item item;
    //        lock (_sync)
    //        {
    //            //CheckForChanges();

    //            if (_byUsername.TryGetValue(username, out item))
    //            {
    //                _lruCache.Use(item);
    //                return item.isInvalid ? null : item.user;
    //            }
    //        }

    //        var row = IoC.Resolve<IUserRetrieveService>().ByUsername(username);
    //        item = row == null ? new Item { isInvalid = true, user = new UserDefinition { Username = username } } : new Item { user = row };

    //        lock (_sync)
    //        {
    //            Item old;
    //            if (_byUsername.TryGetValue(username, out old))
    //                _lruCache.Remove(old);

    //            _lruCache.Use(item);
    //            _byUsername[username] = item;

    //            if (item.isInvalid)
    //                return null;

    //            _byId[item.user.UserId] = item;
    //        }

    //        return item.user;
    //    }

    //    private static void RemoveItemCallback(Item item)
    //    {
    //        if (item.user.UserId != -1)
    //            _byId.Remove(item.user.UserId);
    //        if (item.user.Username != null)
    //            _byUsername.Remove(item.user.Username);
    //    }

    //    public static bool Validate(string username, string password)
    //    {
    //        return IoC.Resolve<IUserAuthenticationService>().Validate(username, password);
    //    }

    //    private class Item : LruCacheNode
    //    {
    //        public IUserDefinition user;
    //        public bool isInvalid;
    //    }
    //}
}