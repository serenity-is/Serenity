using Serenity.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    [NotMapped]
    public class RowListField<TForeign> : CustomClassField<List<TForeign>> where TForeign: class, IRow
    {
        public RowListField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default | FieldFlags.NotMapped, 
            Func<IRow, List<TForeign>> getValue = null, Action<IRow, List<TForeign>> setValue = null)
            : base(collection, name, caption, size, flags, getValue, setValue)
        {
        }

        protected override int CompareValues(List<TForeign> value1, List<TForeign> value2)
        {
            var length = Math.Min(value1.Count, value2.Count);

            for (var i = 0; i < length; i++)
            {
                var v1 = value1[i];
                var v2 = value2[i];

                if (v1 == null && v2 == null)
                    continue;

                if (v1 == null)
                    return -1;

                if (v2 == null)
                    return 1;

                foreach (var f in v1.Fields)
                {
                    var c = f.IndexCompare(v1, v2);
                    if (c != 0)
                        return c;
                }
            }

            return value1.Count.CompareTo(value2.Count);
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
