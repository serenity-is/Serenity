using System;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;

namespace Serenity.Data
{
    public sealed class Int16Field : GenericValueField<Int16>, IIdField
    {
        public Int16Field(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default, 
            Func<Row, Int16?> getValue = null, Action<Row, Int16?> setValue = null)
            : base(collection, FieldType.Int16, name, caption, size, flags, getValue, setValue)
        {
        }

        public static Int16Field Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<Row, Int16?> getValue, Action<Row, Int16?> setValue)
        {
            return new Int16Field(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, Row row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
                _setValue(row, Convert.ToInt16(reader.GetValue(index)));

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        public override void ValueToJson(Newtonsoft.Json.JsonWriter writer, Row row, JsonSerializer serializer)
        {
            var value = _getValue(row);
            if (value == null)
                writer.WriteNull();
            else //if (EnumType == null) şimdilik enum tipi belirtilse de string e çevrilerek yazma işlemi iptal, flag ile daha sonra ekleyelim
                writer.WriteValue(value.Value);
            //else if (EnumType.IsEnum)
            //    writer.WriteValue(Enum.GetName(EnumType, value.Value));
            //else if (EnumType.IsSubclassOf(typeof(DataEnum)))
            //    writer.WriteValue(DataEnum.ConvertFromInt32(EnumType, value.Value).Key);
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
                    var v = Convert.ToInt16(reader.Value, CultureInfo.InvariantCulture);
                    if (EnumType == null)
                        _setValue(row, v);
                    else
                        _setValue(row, (Int16)Int32Field.ConvertEnumFromInt(EnumType, v));
                    break;
                case JsonToken.String:
                    string s = ((string)reader.Value).TrimToNull();                  
                    if (s == null)
                        _setValue(row, null);
                    else if (EnumType == null)
                        _setValue(row, Convert.ToInt16(s, CultureInfo.InvariantCulture));
                    else
                        _setValue(row, (Int16)Int32Field.ConvertEnumFromString(EnumType, s));
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
                        _setValue(row, (Int16)value.Value);
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
