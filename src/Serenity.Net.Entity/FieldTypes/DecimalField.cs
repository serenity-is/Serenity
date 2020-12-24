using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Serenity.Data
{
    public sealed class DecimalField : GenericValueField<decimal>
    {
        public DecimalField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
            Func<IRow, decimal?> getValue = null, Action<IRow, decimal?> setValue = null)
            : base(collection, FieldType.Decimal, name, caption, size, flags, getValue, setValue)
        {
        }

        public static DecimalField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<IRow, decimal?> getValue, Action<IRow, decimal?> setValue)
        {
            return new DecimalField(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, IRow row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            var value = reader.GetValue(index);
            if (value is DBNull)
                _setValue(row, null);
            else if (value is decimal d)
                _setValue(row, d);
            else
                _setValue(row, Convert.ToDecimal(value, CultureInfo.InvariantCulture));

            row.FieldAssignedValue(this);
        }

        public override void ValueToJson(JsonWriter writer, IRow row, JsonSerializer serializer)
        {
            writer.WriteValue(_getValue(row));
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
                    _setValue(row, Convert.ToDecimal(reader.Value, CultureInfo.InvariantCulture));
                    break;
                case JsonToken.String:
                    var s = ((string)reader.Value).TrimToNull();
                    if (s == null)
                        _setValue(row, null);
                    else
                        _setValue(row, Convert.ToDecimal(s, CultureInfo.InvariantCulture));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            row.FieldAssignedValue(this);
        }
    }
}
