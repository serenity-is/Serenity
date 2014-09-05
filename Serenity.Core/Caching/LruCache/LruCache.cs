using System;

namespace Serenity.Caching
{
    public class LruCache<T> where T: LruCacheNode
    {
        private int _maxCount;
        private int _makeSpaceBy;
        private LruCacheNode _head;
        private int _count;
        private Action<T> _removeItem;

        public LruCache(
            int maxCount, int makeSpaceBy, Action<T> removeItem)
        {
            if (removeItem == null)
                throw new ArgumentNullException("removeItem");

            if (maxCount <= 0)
                throw new ArgumentOutOfRangeException("maxCount");

            if (makeSpaceBy <= 0)
                throw new ArgumentOutOfRangeException("makeSpaceBy");

            _maxCount = maxCount;
            _makeSpaceBy = makeSpaceBy;
            _removeItem = removeItem;
        }

        private void InitHead(LruCacheNode item)
        {
            _count = 1;
            item._next = item;
            item._prev = item;
            _head = item;
        }

        public void Use(T item)
        {
            if (_head == item)
                return;

            if (_head == null)
            {
                InitHead(item);
                return;
            }

            if (item._next != null)
            {
                item._next._prev = item._prev;
                item._prev._next = item._next;
            }
            else
            {
                if (_count >= _maxCount)
                {
                    MakeSpace();
                    if (_head == null)
                    {
                        InitHead(item);
                        return;
                    }
                }

                _count++;
            }

            item._next = _head;
            item._prev = _head._prev;
            _head._prev._next = item;
            _head._prev = item;
            _head = item;
        }

        private void MakeSpace()
        {
            if (_count == 0)
                return;

            var remove = _makeSpaceBy;
            if (_count < remove)
                remove = _count;

            var prev = _head._prev;
            for (var i = 0; i < remove; i++)
            {
                _removeItem((T)prev);
                prev = prev._prev;
            }

            _count -= remove;

            if (_count == 0)
                _head = null;
            else
            {
                prev._next = _head;
                _head._prev = prev;
            }
        }

        public void Remove(LruCacheNode item)
        {
            if (item._next == item)
            {
                _head = null;
            }
            else
            {
                item._next._prev = item._prev;
                item._prev._next = item._next;
                if (_head == item)
                    _head = item._next;
            }
            _count--;
            _removeItem((T)item);
        }

        public void Clear(bool callRemoveItem)
        {
            if (_head != null)
            {
                if (callRemoveItem)
                {
                    var next = _head;
                    do
                    {
                        _removeItem((T)next);
                        next = next._next;
                    }
                    while (next != _head);
                }
            }
        }
    }
}
