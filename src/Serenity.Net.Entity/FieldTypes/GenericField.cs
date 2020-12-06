using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    public abstract class GenericField<TValue> : Field
    {
        protected internal Func<IRow, TValue> _getValue;
        protected internal Action<IRow, TValue> _setValue;

        public GenericField(ICollection<Field> collection, FieldType type, string name, string caption, int size, FieldFlags flags,
            Func<IRow, TValue> getValue, Action<IRow, TValue> setValue)
            : base(collection, type, name, caption, size, flags)
        {
            _getValue = getValue;
            _setValue = setValue;
        }

        public override void Copy(IRow source, IRow target)
        {
            _setValue(target, _getValue(source));
            target.FieldAssignedValue(this);
        }

        public TValue this[IRow row]
        {
            get
            {
                CheckUnassignedRead(row);
                return _getValue(row);
            }
            set
            {
                _setValue(row, value);
                row.FieldAssignedValue(this);
            }
        }
    }
}