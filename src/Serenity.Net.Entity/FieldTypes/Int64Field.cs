using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Serenity.Data
{
    /// <summary>
    /// Int64Field
    /// </summary>
    public sealed class Int64Field : GenericValueField<long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int64Field"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="name">The name.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="size">The size.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="getValue">The get value.</param>
        /// <param name="setValue">The set value.</param>
        public Int64Field(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
            Func<IRow, long?> getValue = null, Action<IRow, long?> setValue = null)
            : base(collection, FieldType.Int64, name, caption, size, flags, getValue, setValue)
        {
        }

        /// <summary>
        /// Factories the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="name">The name.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="size">The size.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="getValue">The get value.</param>
        /// <param name="setValue">The set value.</param>
        /// <returns></returns>
        public static Int64Field Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<IRow, long?> getValue, Action<IRow, long?> setValue)
        {
            return new Int64Field(collection, name, caption, size, flags, getValue, setValue);
        }

        /// <summary>
        /// Gets from reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="index">The index.</param>
        /// <param name="row">The row.</param>
        /// <exception cref="ArgumentNullException">reader</exception>
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

        /// <summary>
        /// Values to json.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="row">The row.</param>
        /// <param name="serializer">The serializer.</param>
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

        /// <summary>
        /// Values from json.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="row">The row.</param>
        /// <param name="serializer">The serializer.</param>
        /// <exception cref="ArgumentNullException">reader</exception>
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
    }
}
