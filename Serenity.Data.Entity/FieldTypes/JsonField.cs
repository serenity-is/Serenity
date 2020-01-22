using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Serenity.Data
{
    public class JsonField<TValue> : GenericClassField<TValue>
        where TValue: class
    {
        public JsonField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
            Func<Row, TValue> getValue = null, Action<Row, TValue> setValue = null)
            : base(collection, FieldType.Object, name, caption, size, flags, getValue, setValue)
        {
        }

        public static JsonField<TValue> Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<Row, TValue> getValue, Action<Row, TValue> setValue)
        {
            return new JsonField<TValue>(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, Row row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
                _setValue(row, JsonConvert.DeserializeObject<TValue>(reader.GetString(index), Settings ?? JsonSettings.Strict));

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        public JsonSerializerSettings Settings { get; set; }

        public override object AsSqlValue(Row row)
        {
            var value = AsObject(row);
            if (value == null)
                return null;

            return JsonConvert.SerializeObject(value, Settings ?? JsonSettings.Strict);
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
                return value1.ToJson().CompareTo(value2.ToJson());
        }

        public override void ValueToJson(JsonWriter writer, Row row, JsonSerializer serializer)
        {
            serializer.Serialize(writer, _getValue(row));
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
                    if (typeof(TValue) == typeof(string))
                        _setValue(row, serializer.Deserialize<TValue>(reader));
                    else
                        _setValue(row, JSON.Parse<TValue>((string)reader.Value, includeNulls: serializer.NullValueHandling == NullValueHandling.Include));
                    break;
                default:
                    _setValue(row, serializer.Deserialize<TValue>(reader));
                    break;
            }

            if (row.tracking)
                row.FieldAssignedValue(this);
        }
    }
}
