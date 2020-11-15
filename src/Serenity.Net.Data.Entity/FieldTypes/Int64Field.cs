using System;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;

namespace Serenity.Data
{
    public sealed class Int64Field : GenericValueField<Int64>, IIdField
    {
        public Int64Field(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default, 
            Func<IRow, Int64?> getValue = null, Action<IRow, Int64?> setValue = null)
            : base(collection, FieldType.Int64, name, caption, size, flags, getValue, setValue)
        {
        }

        public static Int64Field Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<IRow, Int64?> getValue, Action<IRow, Int64?> setValue)
        {
            return new Int64Field(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, IRow row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
                _setValue(row, Convert.ToInt64(reader.GetValue(index), CultureInfo.InvariantCulture));

            row.FieldAssignedValue(this);
        }

        public override void ValueToJson(JsonWriter writer, IRow row, JsonSerializer serializer)
        {
            var value = _getValue(row);
            if (value == null)
                writer.WriteNull();
            else
            {
                var intvalue = value.Value;
                if (intvalue > 9007199254740992 ||
                    intvalue < -9007199254740992)
                    writer.WriteValue(intvalue.ToString(CultureInfo.InvariantCulture));
                else
                    writer.WriteValue(intvalue);
            }
        }

        public override void ValueFromJson(JsonReader reader, IRow row, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            switch (reader.TokenType)
            {
                case JsonToken.Null:
                case JsonToken.Undefined:
                    _setValue(row, null);
                    break;
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.Boolean:
                    var v = Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture);
                    if (EnumType == null)
                        _setValue(row, v);
                    else
                        _setValue(row, Int32Field.ConvertEnumFromInt(EnumType, v));
                    break;
                case JsonToken.String:
                    string s = ((string)reader.Value).TrimToNull();
                    if (s == null)
                        _setValue(row, null);
                    else if (EnumType == null)
                        _setValue(row, Convert.ToInt64(s, CultureInfo.InvariantCulture));
                    else
                        _setValue(row, Int32Field.ConvertEnumFromString(EnumType, s));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            row.FieldAssignedValue(this);
        }

        Int64? IIdField.this[IRow row]
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

        bool IIdField.IsIntegerType
        {
            get { return true; }
        }
    }
}
