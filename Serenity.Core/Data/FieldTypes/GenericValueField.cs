using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Reflection.Emit;
using System.Linq.Expressions;
using System.ComponentModel;

namespace Serenity.Data
{
    public interface IEnumTypeField
    {
        Type EnumType { get; set; }
    }

    public abstract class GenericValueField<TValue> : Field, IEnumTypeField where TValue: struct, IComparable<TValue>
    {
        protected internal Func<Row, TValue?> _getValue;
        protected internal Action<Row, TValue?> _setValue;
        protected internal Type _enumType;
        protected internal TypeConverter _converter;

        internal GenericValueField(ICollection<Field> collection, FieldType type, string name, LocalText caption, int size, FieldFlags flags, 
            Func<Row, TValue?> getValue = null, Action<Row, TValue?> setValue = null)
            : base(collection, type, name, caption, size, flags)
        {
            _getValue = getValue ?? (r => (TValue?)(r.GetIndexedData(this.index)));
            _setValue = setValue ?? ((r, v) => r.SetIndexedData(this.index, v));
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

        public Type EnumType
        {
            get { return _enumType; }
            set { _enumType = value; }
        }

        public override void Copy(Row source, Row target)
        {
            _setValue((Row)(target), _getValue((Row)(source)));
            if (target.tracking)
                target.FieldAssignedValue(this);
        }

        public TValue? this[Row row]
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

        public override object AsObject(Row row)
        {
            return _getValue(row);
        }

        public override void AsObject(Row row, object value)
        {
            if (value == null)
                _setValue(row, null);
            else
                _setValue(row, (TValue)value);

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        protected override bool GetIsNull(Row row)
        {
            return _getValue(row) == null;
        }

        public override int IndexCompare(Row row1, Row row2)
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