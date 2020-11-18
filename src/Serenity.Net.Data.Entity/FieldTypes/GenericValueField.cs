using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    public abstract class GenericValueField<TValue> : Field, IEnumTypeField where TValue: struct, IComparable<TValue>
    {
        protected internal Func<IRow, TValue?> _getValue;
        protected internal Action<IRow, TValue?> _setValue;
        protected internal Type _enumType;

        internal GenericValueField(ICollection<Field> collection, FieldType type, string name, LocalText caption, int size, FieldFlags flags, 
            Func<IRow, TValue?> getValue = null, Action<IRow, TValue?> setValue = null)
            : base(collection, type, name, caption, size, flags)
        {
            _getValue = getValue ?? (r => (TValue?)(r.GetIndexedData(index)));
            _setValue = setValue ?? ((r, v) => r.SetIndexedData(index, v));
        }

        public override object ConvertValue(object source, IFormatProvider provider)
        {
            if (source is JValue jValue)
                source = jValue.Value;

            if (source == null)
                return null;
            else
            {
                if (source is TValue val)
                    return val;

                return Convert.ChangeType(source, typeof(TValue), provider);
            }
        }

        public Type EnumType
        {
            get { return _enumType; }
            set { _enumType = value; }
        }

        public override void Copy(IRow source, IRow target)
        {
            _setValue(target, _getValue(source));
            if (target.TrackAssignments)
                target.FieldAssignedValue(this);
        }

        public TValue? this[IRow row]
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

        public override object AsObject(IRow row)
        {
            return _getValue(row);
        }

        public override void AsObject(IRow row,object value)
        {
            if (value == null)
                _setValue(row, null);
            else
            {
                try
                {
                    _setValue(row, (TValue)value);
                } catch (InvalidCastException ex)
                {
                    throw new InvalidCastException($"Invalid cast exception while trying to set the value of {Name} field on {row.GetType().Name} as object.", ex);
                }
            }

            row.FieldAssignedValue(this);
        }

        protected override bool GetIsNull(IRow row)
        {
            return _getValue(row) == null;
        }

        public override int IndexCompare(IRow row1, IRow row2)
        {
            var val1 = _getValue(row1);
            if (val1.HasValue)
            {
                var val2 = _getValue(row2);
                return val2.HasValue ? val1.Value.CompareTo(val2.Value) : 1;
            }
            else
                return _getValue(row2).HasValue ? -1 : 0;
        }

        public override Type ValueType
        {
            get { return typeof(TValue?); }
        }
    }
}