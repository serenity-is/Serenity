using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Serenity.Data
{
    public class Int32Field : GenericValueField<int>
    {
        public Int32Field(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
            Func<IRow, int?> getValue = null, Action<IRow, int?> setValue = null)
            : base(collection, FieldType.Int32, name, caption, size, flags, getValue, setValue)
        {
        }

        public static Int32Field Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<IRow, int?> getValue, Action<IRow, int?> setValue)
        {
            return new Int32Field(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, IRow row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            var value = reader.GetValue(index);
            if (value is DBNull)
                _setValue(row, null);
            else if (value is int val)
                _setValue(row, val);
            else
                _setValue(row, Convert.ToInt32(value, CultureInfo.InvariantCulture));

            row.FieldAssignedValue(this);
        }

        public override void ValueToJson(JsonWriter writer, IRow row, JsonSerializer serializer)
        {
            var value = _getValue(row);
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(value.Value);
        }

        internal static long ConvertEnumFromInt(Type enumType, long v)
        {
            if (enumType.IsEnum)
            {
                var val = Enum.Parse(enumType, v.ToString());
                if (!Enum.IsDefined(enumType, val))
                    throw new InvalidCastException(string.Format("{0} is not a valid {1} enum value!", v, enumType.Name));

                return v;
            }
            else
                throw new InvalidProgramException(string.Format("{0} is not a valid enum type!", enumType.Name));
        }

        internal static long ConvertEnumFromString(Type enumType, string s)
        {
            if (enumType.IsEnum)
            {
                if (long.TryParse(s, out long v))
                {
                    var val = Enum.Parse(enumType, s);
                    if (!Enum.IsDefined(enumType, val))
                        throw new InvalidCastException(string.Format("{0} is not a valid {1} enum value!", v, enumType.Name));

                    return v;
                }
                else
                    return Convert.ToInt64(Enum.Parse(enumType, s), CultureInfo.InvariantCulture);
            }
            else
                throw new InvalidProgramException(string.Format("{0} is not a valid enum type!", enumType.Name));
        }

        public override void ValueFromJson(JsonReader reader, IRow row, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            int v;

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
                        _setValue(row, (int)ConvertEnumFromInt(EnumType, v));
                    break;
                case JsonToken.String:
                    string s = ((string)reader.Value).TrimToNull();
                    if (s == null)
                        _setValue(row, null);
                    else if (EnumType == null)
                        _setValue(row, Convert.ToInt32(s, CultureInfo.InvariantCulture));
                    else
                        _setValue(row, (int)ConvertEnumFromString(EnumType, s));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            row.FieldAssignedValue(this);
        }
    }
}
