using System;
using System.Collections.Generic;
using Serenity.Data;

namespace Serenity.Data
{
    public abstract class BaseOnDemandRowCache<TRow>
        where TRow: Row, new()
    {
        public class Params
        {
            public Params()
            {
                MaxItems = 10000;
                MakeSpaceBy = 100;
                Expiration = TimeSpan.FromMinutes(5);
            }

            public int MaxItems;
            public int MakeSpaceBy;
            public TimeSpan Expiration;
        }

        private Dictionary<Int32, Item> _byId;
        private Dictionary<string, Item> _byCode;

        private object _sync;
        private LruCache<Item> _lruCache;

        private TimeSpan _expiration;

        private IIdField _idField;
        private StringField _codeField;

        protected BaseOnDemandRowCache()
            : this(new Params())
        {
        }

        protected virtual IIdField GetIdField(Row instance)
        {
            return ((IIdRow)instance).IdField;
        }

        protected BaseOnDemandRowCache(Params prm)
        {
            _sync = new Object();

            _expiration = prm.Expiration;

            var row = new TRow();

            _idField = GetIdField(row);
            _byId = new Dictionary<int, Item>();

            if (row is ICodeRow)
            {
                _codeField = ((ICodeRow)row).CodeField;
                _byCode = new Dictionary<string, Item>(StringComparer.CurrentCultureIgnoreCase);
            }

            _lruCache = new LruCache<Item>(prm.MaxItems, prm.MakeSpaceBy, RemoveItemCallback);
        }

        protected abstract TRow LoadById(int id);
        protected abstract TRow LoadByCode(string code);

        protected TRow GetById(int id)
        {
            if (id == Int32.MinValue)
                return null;

            Item item;
            lock (_sync)
            {
                if (_byId.TryGetValue(id, out item) &&
                    !item.IsExpired)
                {
                    _lruCache.Use(item);
                    return item.isInvalid ? null : item.row;
                }
            }

            var row = LoadById(id);

            if (row == null)
            {
                item = new Item(DateTime.Now.Add(_expiration));
                _idField[item.row] = id;
            }
            else
            {
                item = new Item(DateTime.Now.Add(_expiration), row);
            }

            lock (_sync)
            {
                Item old;
                if (_byId.TryGetValue(id, out old))
                    _lruCache.Remove(old);

                _lruCache.Use(item);
                _byId[id] = item;
                if (!item.isInvalid && _byCode != null)
                {
                    var code = _codeField[row];
                    if (!code.IsEmptyOrNull())
                        _byCode[code] = item;
                }
            }

            return item.isInvalid ? null : item.row;
        }

        public void RemoveById(int id)
        {
            if (id == -1)
                return;

            Item item;
            lock (_sync)
            {
                if (_byId.TryGetValue(id, out item))
                    _lruCache.Remove(item);
            }
        }

        protected void RemoveByCode(string code)
        {
            if (code.IsEmptyOrNull() ||
                _byCode == null)
                return;

            Item item;
            lock (_sync)
            {
                if (_byCode.TryGetValue(code, out item))
                    _lruCache.Remove(item);
            }
        }

        public void RemoveRow(TRow row)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            Item item;
            lock (_sync)
            {
                var id = _idField[row];

                if (id != null &&
                    _byId.TryGetValue((int)id.Value, out item))
                    _lruCache.Remove(item);

                if (_codeField != null)
                {
                    var code = _codeField[row];

                    if (!code.IsEmptyOrNull() &&
                        _byCode.TryGetValue(code, out item))
                        _lruCache.Remove(item);
                }
            }
        }

        public void RemoveAll()
        {
            lock (_sync)
            {
                _lruCache.Clear(false);
                _byId.Clear();
                if (_byCode != null)
                    _byCode.Clear();
            }
        }

        protected TRow GetByCode(string code)
        {
            if (code.IsEmptyOrNull())
                return null;

            if (_byCode == null)
                throw new InvalidOperationException("byCode");

            Item item;
            lock (_sync)
            {
                if (_byCode.TryGetValue(code, out item) &&
                    !item.IsExpired)
                {
                    _lruCache.Use(item);
                    return item.isInvalid ? null : item.row;
                }
            }

            var row = LoadByCode(code);

            if (row == null)
            {
                item = new Item(DateTime.Now.Add(_expiration));
                _codeField[item.row] = code;
            }
            else
                item = new Item(DateTime.Now.Add(_expiration), row);

            lock (_sync)
            {
                Item old;
                if (_byCode.TryGetValue(code, out old))
                    _lruCache.Remove(old);

                _lruCache.Use(item);
                _byCode[code] = item;
                if (!item.isInvalid)
                    _byId[(int)_idField[item.row].Value] = item;
            }

            return item.isInvalid ? null : item.row;
        }

        private void RemoveItemCallback(Item item)
        {
            var id = _idField[item.row];
            if (id >= 0)
                _byId.Remove((int)id);
            if (_codeField != null)
            {
                var code = _codeField[item.row];
                if (!code.IsEmptyOrNull())
                    _byCode.Remove(code);
            }
        }

        private class Item : ExpiringLruCacheNode
        {
            public TRow row;
            public bool isInvalid;

            public Item(DateTime expiration)
                : base(expiration)
            {
                isInvalid = true;
                this.row = new TRow();
            }

            public Item(DateTime expiration, TRow row)
                : base(expiration)
            {
                this.row = row;
            }
        }
    }
}