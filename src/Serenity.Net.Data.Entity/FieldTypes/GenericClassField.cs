using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    public abstract class GenericClassField<TValue> : Field where TValue : class
    {
        protected internal Func<IRow, TValue> _getValue;
        protected internal Action<IRow, TValue> _setValue;

        internal GenericClassField(ICollection<Field> collection, FieldType type, string name, LocalText caption, int size, FieldFlags flags,
            Func<IRow, TValue> getValue = null, Action<IRow, TValue> setValue = null)
            : base(collection, type, name, caption, size, flags)
        {
            _getValue = getValue ?? (r => (TValue)(r.GetIndexedData(index)));
            _setValue = setValue ?? ((r, v) => r.SetIndexedData(index, v));
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

        public override object ConvertValue(object source, IFormatProvider provider)
        {
            if (source is JValue jValue)
                source = jValue.Value;

            if (source == null)
                return null;
            else
            {
                if (source is TValue value)
                    return value;

                return Convert.ChangeType(source, typeof(TValue), provider);
            }
        }

        public override object AsObject(IRow row)
        {
            CheckUnassignedRead(row);
            return _getValue(row);
        }

        public override void AsObject(IRow row, object value)
        {
            _setValue(row, (TValue)value);
            row.FieldAssignedValue(this);
        }

        protected override bool GetIsNull(IRow row)
        {
            return _getValue(row) == null;
        }

        public override Type ValueType { get { return typeof(TValue); } }
    }
}