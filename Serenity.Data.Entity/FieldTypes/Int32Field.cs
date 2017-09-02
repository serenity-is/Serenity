using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Newtonsoft.Json;

namespace Serenity.Data
{
    public sealed class Int32Field : GenericValueField<Int32>, IIdField
    {
        public Int32Field(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default, 
            Func<Row, Int32?> getValue = null, Action<Row, Int32?> setValue = null)
            : base(collection, FieldType.Int32, name, caption, size, flags, getValue, setValue)
        {
        }

        public static Int32Field Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<Row, Int32?> getValue, Action<Row, Int32?> setValue)
        {
            return new Int32Field(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, Row row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            var value = reader.GetValue(index);
            if (value is DBNull)
                _setValue(row, null);
            else if (value is Int32)
                _setValue(row, (Int32)value);
            else
                _setValue(row, Convert.ToInt32(value, CultureInfo.InvariantCulture));

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        public override void ValueToJson(JsonWriter writer, Row row, JsonSerializer serializer)
        {
            var value = _getValue(row);
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(value.Value);
        }

        internal static Int64 ConvertEnumFromInt(Type enumType, Int64 v)
        {
            if (enumType.IsEnum)
            {
                var val = Enum.Parse(enumType, v.ToString());
                if (!Enum.IsDefined(enumType, val))
                    throw new InvalidCastException(String.Format("{0} is not a valid {1} enum value!", v, enumType.Name));

                return v;
            }
            else
                throw new InvalidProgramException(String.Format("{0} is not a valid enum type!", enumType.Name));
        }

        internal static Int64 ConvertEnumFromString(Type enumType, string s)
        {
            if (enumType.IsEnum)
            {
                Int64 v;
                if (Int64.TryParse(s, out v))
                {
                    var val = Enum.Parse(enumType, s);
                    if (!Enum.IsDefined(enumType, val))
                        throw new InvalidCastException(String.Format("{0} is not a valid {1} enum value!", v, enumType.Name));

                    return v;
                }
                else
                    return Convert.ToInt64(Enum.Parse(enumType, s), CultureInfo.InvariantCulture);
            }
            else
                throw new InvalidProgramException(String.Format("{0} is not a valid enum type!", enumType.Name));
        }

        public override void ValueFromJson(JsonReader reader, Row row, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            Int32 v;

            switch (reader.TokenType)
            {
                case JsonToken.Null:
                case JsonToken.Undefined:
                    _setValue(row, null);
                    break;
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.Boolean:
                    v = Convert.ToInt32(reader.Value, CultureInfo.InvariantCulture);
                    if (EnumType == null)
                        _setValue(row, v);
                    else
                        _setValue(row, (Int32)ConvertEnumFromInt(EnumType, v));
                    break;
                case JsonToken.String:
                    string s = ((string)reader.Value).TrimToNull();                  
                    if (s == null)
                        _setValue(row, null);
                    else if (EnumType == null)
                        _setValue(row, Convert.ToInt32(s, CultureInfo.InvariantCulture));
                    else
                        _setValue(row, (Int32)ConvertEnumFromString(EnumType, s));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        Int64? IIdField.this[Row row]
        {
            get
            {
                CheckUnassignedRead(row);
                return _getValue(row);
            }
            set
            {
                if (value.HasValue)
                    checked
                    {
                        _setValue(row, (Int32)value.Value);
                    }
                else
                    _setValue(row, null);
                
                if (row.tracking)
                    row.FieldAssignedValue(this);
            }
        }

        bool IIdField.IsIntegerType
        {
            get { return true; }
        }
    }
}
