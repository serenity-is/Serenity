using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    public class ListField<TItem> : CustomClassField<List<TItem>>
    {
        public ListField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default, 
            Func<Row, List<TItem>> getValue = null, Action<Row, List<TItem>> setValue = null)
            : base(collection, name, caption, size, flags, getValue, setValue)
        {
        }

        protected override int CompareValues(List<TItem> value1, List<TItem> value2)
        {
            if (value1 == null && value2 == null)
                return 0;

            if (value1 == null)
                return -1;

            if (value2 == null)
                return 1;

            if (value1.Count != value2.Count)
                return value1.Count.CompareTo(value2.Count);

            var comparer = Comparer<TItem>.Default;

            for (var i = 0; i < value1.Count; i++)
            {
                var c = comparer.Compare(value1[i], value2[i]);
                if (c != 0)
                    return c;
            }

            return 0;
        }

        protected override List<TItem> Clone(List<TItem> value)
        {
            var clone = new List<TItem>();
            foreach (var item in value)
                clone.Add(item);
            return clone;
        }
    }
}