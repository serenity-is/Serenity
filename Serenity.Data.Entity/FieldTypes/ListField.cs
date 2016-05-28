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
            throw new NotImplementedException();
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