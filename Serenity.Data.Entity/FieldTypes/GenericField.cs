using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;

namespace Serenity.Data
{
    public abstract class GenericField<TValue> : Field
    {
        protected internal Func<Row, TValue> _getValue;
        protected internal Action<Row, TValue> _setValue;

        public GenericField(ICollection<Field> collection, FieldType type, string name, string caption, int size, FieldFlags flags,
            Func<Row, TValue> getValue, Action<Row, TValue> setValue)
            : base(collection, type, name, caption, size, flags)
        {
            _getValue = getValue;
            _setValue = setValue;
        }

        public override void Copy(Row source, Row target)
        {
            _setValue((Row)(target), _getValue((Row)(source)));
            if (target.tracking)
                target.FieldAssignedValue(this);
        }

        public TValue this[Row row]
        {
            get
            {
                CheckUnassignedRead(row);
                return _getValue(row);
            }
            set
            {
                _setValue(row, value);
                if (row.tracking)
                    row.FieldAssignedValue(this);
            }
        }
    }
}