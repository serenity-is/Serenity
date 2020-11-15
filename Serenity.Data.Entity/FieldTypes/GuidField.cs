using System;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Serenity.Data
{
    public sealed class GuidField : GenericValueField<Guid>, IIdField
    {
        public GuidField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default, 
            Func<IRow, Guid?> getValue = null, Action<IRow, Guid?> setValue = null)
            : base(collection, FieldType.Guid, name, caption, size, flags, getValue, setValue)
        {
        }

        public static GuidField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<IRow, Guid?> getValue, Action<IRow, Guid?> setValue)
        {
            return new GuidField(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, IRow row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
                _setValue(row, reader.GetGuid(index));

            row.FieldAssignedValue(this);
        }

        public override void ValueToJson(Newtonsoft.Json.JsonWriter writer, IRow row, JsonSerializer serializer)
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
                case JsonToken.String:
                    var val = reader.Value as string;
                    if (val == "")
                        _setValue(row, null);
                    else
                        _setValue(row, Guid.Parse((string)reader.Value));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            row.FieldAssignedValue(this);
        }

        public override object ConvertValue(object source, IFormatProvider provider)
        {
            if (source is JValue jValue)
                source = jValue.Value;

            if (source == null)
                return null;

            if (source is Guid guid)
                return guid;

            if (source is string str)
            {
                if (str == "")
                    return null;

                return new Guid(str);
            }

            if (source is byte[])
                return new Guid(source as byte[]);

            return Convert.ChangeType(source, typeof(Guid), provider);
        }

        bool IIdField.IsIntegerType
        {
            get
            {
                return false;
            }
        }

        long? IIdField.this[IRow row]
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