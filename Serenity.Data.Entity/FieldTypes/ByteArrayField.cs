using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Serenity.Data
{
    public class ByteArrayField : CustomClassField<byte[]>
    {
        public ByteArrayField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
            Func<Row, byte[]> getValue = null, Action<Row, byte[]> setValue = null)
            : base(collection, name, caption, size, flags, getValue, setValue)
        {
        }

        public static ByteArrayField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<Row, byte[]> getValue, Action<Row, byte[]> setValue)
        {
            return new ByteArrayField(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, Row row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
            {
                byte[] a;
#if COREFX
                if (reader.GetType().Name == "SqliteDataReader")
                {
                    a = (byte[])reader.GetValue(index);
                }
                else
                {
#endif
                    long available = reader.GetBytes(index, (long)0, null, 0, 0);
                    a = new byte[available];
                    if (a.Length > 0)
                        reader.GetBytes(index, (long)0, a, 0, a.Length);
#if COREFX
                }
#endif

                _setValue(row, a);
            }

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        protected override int CompareValues(byte[] value1, byte[] value2)
        {
            var length = Math.Min(value1.Length, value2.Length);
            for (var i = 0; i < length; i++)
            {
                var c = value1[i].CompareTo(value2[i]);
                if (c != 0)
                    return c;
            }

            return value2.Length.CompareTo(value2.Length);
        }

        protected override byte[] Clone(byte[] value)
        {
            return (byte[])value.Clone();
        }

        public override void ValueToJson(JsonWriter writer, Row row, JsonSerializer serializer)
        {
            var value = _getValue(row);
            if (value == null)
                writer.WriteNull();
            else
            {
                writer.WriteValue(value);
            }
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
                    _setValue(row, Convert.FromBase64String((string)reader.Value));
                    break;
                case JsonToken.Bytes:
                    _setValue(row, (byte[])reader.Value);
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            if (row.tracking)
                row.FieldAssignedValue(this);
        }
    }
}
