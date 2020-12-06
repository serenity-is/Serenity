using Serenity.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    [NotMapped]
    public class ListField<TItem> : CustomClassField<List<TItem>>
    {
        public ListField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default | FieldFlags.NotMapped,
            Func<IRow, List<TItem>> getValue = null, Action<IRow, List<TItem>> setValue = null)
            : base(collection, name, caption, size, flags, getValue, setValue)
        {
        }

        protected override int CompareValues(List<TItem> value1, List<TItem> value2)
        {
            var comparer = Comparer<TItem>.Default;
            var length = Math.Min(value1.Count, value2.Count);
            for (var i = 0; i < length; i++)
            {
                var c = comparer.Compare(value1[i], value2[i]);
                if (c != 0)
                    return c;
            }

            return value1.Count.CompareTo(value2.Count);
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
