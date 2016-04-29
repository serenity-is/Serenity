﻿using System;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Serenity.Data
{
    public sealed class GuidField : GenericValueField<Guid>, IIdField
    {
        public GuidField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default, 
            Func<Row, Guid?> getValue = null, Action<Row, Guid?> setValue = null)
            : base(collection, FieldType.Guid, name, caption, size, flags, getValue, setValue)
        {
        }

        public static GuidField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<Row, Guid?> getValue, Action<Row, Guid?> setValue)
        {
            return new GuidField(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, Row row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
                _setValue(row, reader.GetGuid(index));

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        public override void ValueToJson(Newtonsoft.Json.JsonWriter writer, Row row, JsonSerializer serializer)
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
                    var val = reader.Value as string;
                    if (val == "")
                        _setValue(row, null);
                    else
                        _setValue(row, Guid.Parse((string)reader.Value));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            if (row.tracking)
                row.FieldAssignedValue(this);
        }

        public override object ConvertValue(object source, IFormatProvider provider)
        {
            if (source == null)
                return null;

            if (source is Guid)
                return (Guid)source;

            var value = source as string;
            if (value != null)
            {
                if (value == "")
                    return null;

                return new Guid(value);
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
