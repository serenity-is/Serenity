using System;

namespace Serenity.Caching
{
    public class LruCacheNode
    {
        internal LruCacheNode _prev;
        internal LruCacheNode _next;
    }

    public class ExpiringLruCacheNode : LruCacheNode
    {
        internal DateTime _expiration;

        public ExpiringLruCacheNode(DateTime expiration)
        {
            _expiration = expiration;
        }

        public bool IsExpired
        {
            get { return _expiration <= DateTime.Now; }
        }

    }
}
