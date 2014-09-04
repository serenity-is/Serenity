using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    public class RowListField<TForeign> : CustomClassField<List<TForeign>> where TForeign: Row
    {
        public RowListField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default, 
            Func<Row, List<TForeign>> getValue = null, Action<Row, List<TForeign>> setValue = null)
            : base(collection, name, caption, size, flags, getValue, setValue)
        {
        }

        protected override int CompareValues(List<TForeign> value1, List<TForeign> value2)
        {
            throw new NotImplementedException();
        }

        protected override List<TForeign> Clone(List<TForeign> value)
        {
            var clone = new List<TForeign>();
            foreach (var row in value)
                clone.Add(row.Clone());
            return clone;
        }
    }
}