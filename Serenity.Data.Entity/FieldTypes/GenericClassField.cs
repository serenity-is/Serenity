using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;

namespace Serenity.Data
{
    public abstract class GenericClassField<TValue> : Field where TValue : class
    {
        protected internal Func<Row, TValue> _getValue;
        protected internal Action<Row, TValue> _setValue;

        internal GenericClassField(ICollection<Field> collection, FieldType type, string name, LocalText caption, int size, FieldFlags flags,
            Func<Row, TValue> getValue = null, Action<Row, TValue> setValue = null)
            : base(collection, type, name, caption, size, flags)
        {
            _getValue = getValue ?? (r => (TValue)(r.GetIndexedData(this.index)));
            _setValue = setValue ?? ((r, v) => r.SetIndexedData(this.index, v));
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

        public override object ConvertValue(object source, IFormatProvider provider)
        {
            if (source == null)
                return null;
            else
            {
                if (source is TValue)
                    return (TValue)source;

                return Convert.ChangeType(source, typeof(TValue), provider);
            }
        }

        public override object AsObject(Row row)
        {
            CheckUnassignedRead(row);
            return _getValue(row);
        }

        public override void AsObject(Row row, object value)
        {
            _setValue(row, (TValue)value);

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        protected override bool GetIsNull(Row row)
        {
            return _getValue(row) == null;
        }

        public override Type ValueType { get { return typeof(TValue); } }
    }
}