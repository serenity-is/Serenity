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

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
                _setValue(row, Convert.ToInt32(reader.GetValue(index), CultureInfo.InvariantCulture));

            if (row._tracking)
                row.FieldAssignedValue(this);
        }

        public override void ValueToJson(JsonWriter writer, Row row, JsonSerializer serializer)
        {
            var value = _getValue(row);
            if (value == null)
                writer.WriteNull();
            else if (EnumType == null)
                writer.WriteValue(value.Value);
            else if (EnumType.IsEnum)
                writer.WriteValue(Enum.GetName(EnumType, value.Value));
            else if (EnumType.IsSubclassOf(typeof(DataEnum)))
                writer.WriteValue(DataEnum.ConvertFromInt32(EnumType, value.Value).Key);
            else
                throw new InvalidProgramException(String.Format("{0} geçerli bir enum tipi deðil!", EnumType.Name));
        }

        internal static Int64 ConvertEnumFromInt(Type enumType, Int64 v)
        {
            if (enumType.IsEnum)
            {
                if (!Enum.IsDefined(enumType, v))
                    throw new InvalidCastException(String.Format("{0} geçerli bir {1} deðeri deðil!", v, enumType.Name));

                return v;
            }
            else if (enumType.IsSubclassOf(typeof(DataEnum)))
            {
                DataEnum.ConvertFromInt32(enumType, (Int32)v);
                return v;
            }
            else
                throw new InvalidProgramException(String.Format("{0} geçerli bir enum tipi deðil!", enumType.Name));
        }

        internal static Int64 ConvertEnumFromString(Type enumType, string s)
        {
            if (enumType.IsEnum)
            {
                Int64 v;
                if (Int64.TryParse(s, out v))
                {
                    if (!Enum.IsDefined(enumType, v))
                        throw new InvalidCastException(String.Format("{0} geçerli bir {1} deðeri deðil!", v, enumType.Name));

                    return v;
                }
                else
                    return Convert.ToInt64(Enum.Parse(enumType, s), CultureInfo.InvariantCulture);
            }
            else if (enumType.IsSubclassOf(typeof(DataEnum)))
            {
                return ((DataEnum)DataEnum.ConvertFromString(enumType, s)).Id;
            }
            else
                throw new InvalidProgramException(String.Format("{0} geçerli bir enum tipi deðil!", enumType.Name));
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

            if (row._tracking)
                row.FieldAssignedValue(this);
        }

        Int64? IIdField.this[Row row]
        {
            get
            {
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
                
                if (row._tracking)
                    row.FieldAssignedValue(this);
            }
        }
    }
}
