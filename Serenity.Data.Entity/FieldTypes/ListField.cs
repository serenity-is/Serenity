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
    }
}