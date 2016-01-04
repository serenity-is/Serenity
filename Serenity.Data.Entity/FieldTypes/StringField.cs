using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using System.Globalization;

namespace Serenity.Data
{
    public class StringField : GenericClassField<String>, IIdField
    {
        public StringField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
            Func<Row, String> getValue = null, Action<Row, String> setValue = null)
            : base(collection, FieldType.String, name, caption, size, flags, getValue, setValue)
        {
        }

        public static StringField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<Row, String> getValue, Action<Row, String> setValue)
        {
            return new StringField(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, Row row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
                _setValue(row, reader.GetString(index));

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        public override int IndexCompare(Row row1, Row row2)
        {
            var value1 = _getValue(row1);
            var value2 = _getValue(row2);

            bool null1 = value1 == null;
            bool null2 = value2 == null;
            if (null1 || null2)
            {
                if (null1 && null2)
                    return 0;
                else if (null1)
                    return -1;
                else
                    return 1;
            }
            else 
                return value1.CompareTo(value2);
        }

        public override void ValueToJson(JsonWriter writer, Row row, JsonSerializer serializer)
        {
            writer.WriteValue(_getValue(row));
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
                case JsonToken.String:
                    _setValue(row, (string)reader.Value);
                    break;
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.Boolean:
                    _setValue(row, Convert.ToString(reader.Value, CultureInfo.InvariantCulture));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        bool IIdField.IsIntegerType
        {
            get
            {
                return false;
            }
        }

        long? IIdField.this[Row row]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
