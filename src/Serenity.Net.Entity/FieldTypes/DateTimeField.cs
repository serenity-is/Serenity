using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Serenity.Data
{
    /// <summary>
    /// DateTimeField
    /// </summary>
    /// <seealso cref="Serenity.Data.GenericValueField{System.DateTime}" />
    public sealed class DateTimeField : GenericValueField<DateTime>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeField"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="name">The name.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="size">The size.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="getValue">The get value.</param>
        /// <param name="setValue">The set value.</param>
        public DateTimeField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
            Func<IRow, DateTime?> getValue = null, Action<IRow, DateTime?> setValue = null)
            : base(collection, FieldType.DateTime, name, caption, size, flags, getValue, setValue)
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
        public static DateTimeField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<IRow, DateTime?> getValue, Action<IRow, DateTime?> setValue)
        {
            return new DateTimeField(collection, name, caption, size, flags, getValue, setValue);
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public override object ConvertValue(object source, IFormatProvider provider)
        {
            if (source is JValue jValue)
                source = jValue.Value;

            if (source == null)
                return null;
            else
            {
                if (source is DateTime dt)
                    return dt;

                if (source is DateTimeOffset dto)
                    return dto.DateTime;

                return Convert.ChangeType(source, typeof(DateTime), provider);
            }
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
            {
                var value = reader.GetValue(index);
                DateTime datetime = value is DateTimeOffset dto ?
                    dto.DateTime : (value is DateTime dt ? dt : Convert.ToDateTime(value));
                if (DateTimeKind != System.DateTimeKind.Unspecified)
                    datetime = DateTime.SpecifyKind(datetime, DateTimeKind);
                _setValue(row, datetime);
            }

            row.FieldAssignedValue(this);
        }

        private DateTimeKind? dateTimeKind;

        /// <summary>
        /// Gets or sets a value indicating whether [date only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [date only]; otherwise, <c>false</c>.
        /// </value>
        public bool DateOnly
        {
            get
            {
                return dateTimeKind == null;
            }
            set
            {
                if (value != (dateTimeKind == null))
                    dateTimeKind = value ? (DateTimeKind?)null : DateTimeKind.Unspecified;
            }
        }

        /// <summary>
        /// Gets or sets the kind of the date time.
        /// </summary>
        /// <value>
        /// The kind of the date time.
        /// </value>
        public DateTimeKind DateTimeKind
        {
            get { return dateTimeKind ?? DateTimeKind.Unspecified; }
            set { dateTimeKind = value; }
        }

        /// <summary>
        /// Converts to datetimekind.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dateTimeKind">Kind of the date time.</param>
        /// <returns></returns>
        public static DateTime ToDateTimeKind(DateTime value, DateTimeKind? dateTimeKind)
        {
            if (dateTimeKind == null || dateTimeKind == System.DateTimeKind.Unspecified)
                return value;

            if (dateTimeKind == System.DateTimeKind.Utc)
                return value.ToUniversalTime();
            else
                return value.ToLocalTime();
        }

        /// <summary>
        /// Converts to datetimekind.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dateTimeKind">Kind of the date time.</param>
        /// <returns></returns>
        public static DateTime ToDateTimeKind(DateTimeOffset value, DateTimeKind? dateTimeKind)
        {
            if (dateTimeKind == null || dateTimeKind == System.DateTimeKind.Unspecified)
                return value.DateTime;

            if (dateTimeKind == System.DateTimeKind.Utc)
                return value.UtcDateTime;
            else
                return value.LocalDateTime;
        }

        private DateTime ToDateTimeKind(DateTimeOffset value)
        {
            return ToDateTimeKind(value, dateTimeKind);
        }

        private DateTime ToDateTimeKind(DateTime value)
        {
            return ToDateTimeKind(value, dateTimeKind);
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Nullable{DateTime}"/> with the specified row.
        /// </summary>
        /// <value>
        /// The <see cref="System.Nullable{DateTime}"/>.
        /// </value>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public new DateTime? this[IRow row]
        {
            get
            {
                CheckUnassignedRead(row);
                return _getValue(row);
            }
            set
            {
                if (value != null)
                    _setValue(row, ToDateTimeKind(value.Value));
                else
                    _setValue(row, value);
                row.FieldAssignedValue(this);
            }
        }

        /// <summary>
        /// Ases the object.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override object AsObject(IRow row)
        {
            CheckUnassignedRead(row);
            return _getValue(row);
        }

        /// <summary>
        /// Ases the object.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="value">The value.</param>
        public override void AsObject(IRow row, object value)
        {
            if (value == null)
                _setValue(row, null);
            else
                _setValue(row, ToDateTimeKind((DateTime)value));

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
            if (value.HasValue)
            {
                var dt = value.Value;
                if (DateTimeKind == DateTimeKind.Local)
                    dt = dt.ToUniversalTime();
                writer.WriteValue(dt.ToString(
                    (DateTimeKind == System.DateTimeKind.Unspecified ?
                        DateHelper.ISODateTimeFormatLocal :
                        DateHelper.ISODateTimeFormatUTC), CultureInfo.InvariantCulture));
            }
            else
                writer.WriteNull();
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
                case JsonToken.Date:
                    var obj = reader.Value;
                    DateTime value;
                    if (obj is DateTime dt)
                        value = dt;
                    else if (obj is DateTimeOffset dto)
                    {
                        _setValue(row, ToDateTimeKind(dto));
                        break;
                    }
                    else
                        value = Convert.ToDateTime(obj, CultureInfo.InvariantCulture);

                    _setValue(row, ToDateTimeKind(value));
                    break;
                case JsonToken.String:
                    var s = ((string)reader.Value).TrimToNull();
                    if (s == null)
                        _setValue(row, null);
                    else
                        _setValue(row, ToDateTimeKind(Convert.ToDateTime(s, CultureInfo.InvariantCulture)));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            row.FieldAssignedValue(this);
        }
    }
}