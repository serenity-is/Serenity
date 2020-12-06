using Serenity.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    [NotMapped]
    public class RowField<TForeign> : CustomClassField<TForeign> where TForeign : class, IRow
    {
        public RowField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default | FieldFlags.NotMapped,
            Func<IRow, TForeign> getValue = null, Action<IRow, TForeign> setValue = null)
            : base(collection, name, caption, size, flags, getValue, setValue)
        {
        }

        protected override int CompareValues(TForeign value1, TForeign value2)
        {
            if (value1 == null && value2 == null)
                return 0;

            if (value1 == null)
                return -1;

            if (value2 == null)
                return 1;

            foreach (var f in value1.Fields)
            {
                var c = f.IndexCompare(value1, value2);
                if (c != 0)
                    return c;
            }

            return 0;
        }

        protected override TForeign Clone(TForeign value)
        {
            if (value == null)
                return null;

            return value.Clone();
        }
    }
}