using System;
using System.Collections.Generic;
using System.Data;

namespace Serenity.Data
{
    [Flags]
    public enum DeltaOptions
    {
        Default = IgnoreInvalidNewId,
        IgnoreInvalidNewId = 1
    }

    public struct OldNewPair<TItem>
    {
        private TItem _old;
        private TItem _new;

        public OldNewPair(TItem o, TItem n)
        {
            _old = o;
            _new = n;
        }

        public TItem Old { get { return _old; } }
        public TItem New { get { return _new; } }
    }

    public class DeltaLister<TItem>
    {
        private DeltaOptions _options;
        private Dictionary<Int64, TItem> _oldById;
        private HashSet<Int64> _newById;
        private IEnumerable<TItem> _oldItems;
        private IEnumerable<TItem> _newItems;
        private Func<TItem, Int64?> _getItemId;

        public DeltaLister(IEnumerable<TItem> oldList, IEnumerable<TItem> newList,
            Func<TItem, Int64?> getItemId, DeltaOptions options = DeltaOptions.Default)
        {
            if (oldList == null)
                throw new ArgumentNullException("oldList");

            if (newList == null)
                throw new ArgumentNullException("newList");

            if (getItemId == null)
                throw new ArgumentNullException("getItemId");

            _options = options;
            _oldItems = oldList;
            _newItems = newList;
            _getItemId = getItemId;

            _oldById = new Dictionary<Int64, TItem>();
            _newById = new HashSet<Int64>();

            foreach (var item in oldList)
            {
                if (item == null)
                    throw new ArgumentNullException("oldItem");

                var id = getItemId(item);
                if (id == null)
                    throw new ArgumentNullException("oldItemId");

                _oldById.Add(id.Value, item);
            }

            foreach (var item in newList)
            {
                if (item == null)
                    throw new ArgumentNullException("newItem");

                var id = getItemId(item);
                if (id != null)
                {
                    if (!_oldById.ContainsKey(id.Value))
                    {
                        if ((_options & DeltaOptions.IgnoreInvalidNewId) != DeltaOptions.IgnoreInvalidNewId)
                            throw new ArgumentOutOfRangeException("newItemId");
                    }

                    if (_newById.Contains(id.Value))
                        throw new DuplicateNameException("newItemId");

                    _newById.Add(id.Value);
                }
            }
        }

        public IEnumerable<TItem> ItemsToDelete
        {
            get
            {
                foreach (var item in _oldItems)
                {
                    var id = _getItemId(item);
                    if (!_newById.Contains(id.Value))
                        yield return item;
                }
            }
        }

        public IEnumerable<TItem> ItemsToCreate
        {
            get
            {
                foreach (var item in _newItems)
                {
                    var id = _getItemId(item);
                    if (id == null || !_oldById.ContainsKey(id.Value))
                        yield return item;
                }
            }
        }

        public IEnumerable<OldNewPair<TItem>> ItemsToUpdate
        {
            get
            {
                foreach (var item in _newItems)
                {
                    var id = _getItemId(item);
                    TItem old;
                    if (id != null && _oldById.TryGetValue(id.Value, out old))
                        yield return new OldNewPair<TItem>(old, item);
                }
            }
        }
    }
}