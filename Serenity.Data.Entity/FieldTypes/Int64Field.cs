using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using System.Globalization;
using System.Linq.Expressions;

namespace Serenity.Data
{
    public sealed class Int64Field : GenericValueField<Int64>, IIdField
    {
        public Int64Field(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default, 
            Func<Row, Int64?> getValue = null, Action<Row, Int64?> setValue = null)
            : base(collection, FieldType.Int64, name, caption, size, flags, getValue, setValue)
        {
        }

        public static Int64Field Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<Row, Int64?> getValue, Action<Row, Int64?> setValue)
        {
            return new Int64Field(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, Row row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
                _setValue(row, Convert.ToInt64(reader.GetValue(index), CultureInfo.InvariantCulture));

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        public override void ValueToJson(JsonWriter writer, Row row, JsonSerializer serializer)
        {
            var value = _getValue(row);
            if (value == null)
                writer.WriteNull();
            else //if (EnumType == null)
                writer.WriteValue(value.Value);
            //else if (EnumType.IsEnum)
            //    writer.WriteValue(Enum.GetName(EnumType, value.Value));
            //else if (EnumType.IsSubclassOf(typeof(DataEnum)))
            //    writer.WriteValue(DataEnum.ConvertFromInt32(EnumType, (Int32)value.Value).Key);
        }

        public override void ValueFromJson(JsonReader reader, Row row, JsonSerializer serializer)
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
                _setValue(row, value);
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
