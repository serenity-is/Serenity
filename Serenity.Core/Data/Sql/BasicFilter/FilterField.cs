using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Serenity.Data;

namespace Serenity.Data
{
    public enum DateFilterKind
    {
        DateOnly = 1,
        DateTimeUTC = 2,
        DateTimeLocal = 3
    }

    public interface IFilterField
    {
        string Name { get; set; }
        LocalText Title { get; set; }
        string Handler { get; set; }
        bool IsRequired { get; set; }
        string Textual { get; set; }
        Dictionary<string, object> Params { get; set; }
    }

    /// <summary>
    ///   Corresponds to the set of configurable options for fields in a javascript FilterPanel 
    ///   object.</summary>
    /// <remarks>
    ///   By using chaining system, this class tries to make it as easy as it can to configure a field's
    ///   filter options.</remarks>
    [JsonConverter(typeof(FilterField.Converter))]
    public class FilterField : IFilterField
    {
        private string _name;
        private string _textual;
        private LocalText _title;
        private string _handler;
        private Dictionary<string, object> _params = new Dictionary<string,object>();
        private bool _isRequired;

        /// <summary>
        ///   Creates a new FilterField object with given options.</summary>
        /// <param name="name">
        ///   Field name (required).</param>
        /// <param name="display">
        ///   Display text.</param>
        /// <param name="type">
        ///   Field type.</param>
        /// <param name="isRequired">
        ///   Required flag.</param>
        public FilterField(string name, LocalText title, string handler, bool isRequired)
        {
            Initialize(name, title, handler, isRequired);
        }

        /// <summary>
        ///   Creates a new FilterField object with given options.</summary>
        /// <param name="name">
        ///   Field name (required).</param>
        /// <param name="display">
        ///   Display text.</param>
        /// <param name="type">
        ///   Field type.</param>
        public FilterField(string name, LocalText title, string handler)
        {
            Initialize(name, title, handler, true);
        }

        /// <summary>
        ///   Creates a new FilterField object with given options.</summary>
        /// <param name="name">
        ///   Field name (required).</param>
        /// <param name="display">
        ///   Display text.</param>
        /// <param name="type">
        ///   Field type.</param>
        public FilterField(string name, LocalText title)
        {
            Initialize(name, title, "String", true);
        }

        /// <summary>
        ///   Creates a new FilterField object by reading field name, caption and nullable flag from
        ///   a given field schema.</summary>
        /// <param name="field">
        ///   Field schema (required).</param>
        /// <param name="type">
        ///   Field type.</param>
        public FilterField(Field field, string type)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            Initialize(field.Name, field.Caption ?? field.Title, type, 
                (field.Flags & FieldFlags.NotNull) == FieldFlags.NotNull);

            ConsiderTextualField(field);
        }

        protected void ConsiderTextualField(Field field)
        {
            if (field.TextualField == null)
                return;

            var textualField = field.Fields.FindFieldByPropertyName(field.TextualField) ??
                field.Fields.FindField(field.TextualField);

            if (textualField == null)
                return;

            _textual = textualField.Name;
            _title = textualField.Title ?? _title;
        }

        /// <summary>
        ///   Creates a new FilterField object by reading field name, caption, nullable flag and type from
        ///   a given field and its schema.</summary>
        /// <param name="field">
        ///   Field object (required).</param>
        public FilterField(Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            Initialize(field.Name, field.Caption ?? field.Title, ToFilterHandlerType(field),
                (field.Flags & FieldFlags.NotNull) == FieldFlags.NotNull);

            ConsiderTextualField(field);
        }

        /// <summary>
        ///   Initializes the FilterField object.</summary>
        /// <param name="name">
        ///   Field name (required).</param>
        /// <param name="display">
        ///   Caption</param>
        /// <param name="type">
        ///   Field type</param>
        /// <param name="isRequired">
        ///   Required flag</param>
        private void Initialize(string name, LocalText title, string handler, bool isRequired)
        {
            if (name.IsNullOrEmpty())
                throw new ArgumentNullException("name");

            _name = name;
            _title = title;
            _handler = handler;
            _isRequired = isRequired;
        }

        /// <summary>
        ///   Returns a filter field type corresponding to the class of a given field</summary>
        /// <param name="field">
        ///   Field object to be used to determine filter field type.</param>
        /// <returns>
        ///   Filter field type.</returns>
        public string ToFilterHandlerType(Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            string type = "String";
            
            if (field is StringField)
                type = "String";
            if (field is Int16Field || field is Int32Field || field is Int64Field)
            {
                Type enumType = null;
                if (field is IEnumTypeField)
                    enumType = ((IEnumTypeField)field).EnumType;

                if (enumType != null && (enumType.IsEnum || enumType.IsSubclassOf(typeof(DataEnum))))
                {
                    _params["Values"] = FilterFieldExtensions.GetEnumOptionList(enumType);
                    type = "Enum";
                }
                else
                    type = "Integer";
            }
            else if (field is DoubleField || field is DecimalField)
                type = "Decimal";
            else if (field is BooleanField)
                type = "Boolean";
            else if (field is DateTimeField)
            {
                DateTimeKind k = ((DateTimeField)field).DateTimeKind;
                type = "Date";
                if (k == DateTimeKind.Unspecified)
                    _params["DateKind"] = (int)DateFilterKind.DateOnly;
                else if (k == DateTimeKind.Utc)
                    _params["DateKind"] = (int)DateFilterKind.DateTimeUTC;
                else
                    _params["DateKind"] = (int)DateFilterKind.DateTimeLocal;
            }

            return type;
        }

        /// <summary>
        ///   Adds a value to the list of drop down options for the filter field.</summary>
        /// <param name="key">
        ///   Key (required)</param>
        /// <param name="value">
        ///   Value (required)</param>
        /// <returns>
        ///   The FilterField object itself.</returns>
        public FilterField AddValue(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            List<string[]> values;
            object valuesObj;
            if (!_params.TryGetValue("Values", out valuesObj))
            {
                values = new List<string[]>();
                _params["Values"] = values;
            }
            else
                values = (List<string[]>)valuesObj;


            values.Add(new string[] { key, value });
            return this;
        }

        /// <summary>
        ///   Adds a value to the list of drop down options for the filter field.</summary>
        /// <param name="key">
        ///   Key as an integer</param>
        /// <param name="value">
        ///   Value (required)</param>
        /// <returns>
        ///   The FilterField object itself.</returns>
        public FilterField AddValue(Int64 key, string value)
        {
            return AddValue(key.ToInvariant(), value);
        }

        /// <summary>
        ///   Adds a value to the list of drop down options for the filter field.</summary>
        /// <param name="keyValue">
        ///   Key and value (required)</param>
        /// <returns>
        ///   The FilterField object itself.</returns>
        public FilterField AddValue(string keyValue)
        {
            return AddValue(keyValue, keyValue);
        }

        public FilterField Param(string key, object value)
        {
            _params[key] = value;
            return this;
        }

        /// <summary>
        ///   Sets the required flag for the filter field.</summary>
        /// <returns>
        ///   The FilterField object itself.</returns>
        public FilterField Required(bool required)
        {
            _isRequired = required;
            return this;
        }

        /// <summary>
        ///   Sets the caption for the filter field.</summary>
        /// <param name="display">
        ///   Filter field caption.</param>
        /// <returns>
        ///   The FilterField object itself.</returns>
        public FilterField Title(LocalText title)
        {
            _title = title;
            return this;
        }

        /// <summary>
        ///   Sets the name of the filter field.</summary>
        /// <returns>
        ///   The FilterField object itself.</returns>
        public FilterField Name(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            _name = name;
            return this;
        }

        public FilterField Textual(string name)
        {
            _textual = name;
            return this;
        }

        /// <summary>
        ///   Sets the type of the filter field.</summary>
        /// <returns>
        ///   The FilterField object itself.</returns>
        public FilterField Handler(string handler)
        {
            _handler = handler;
            return this;
        }

        /// <summary>
        ///   Gets the name of the filter field.</summary>
        /// <returns>
        ///   The field name of the filter field.</returns>
        public string GetName()
        {
            return _name;
        }

        public string GetTextual()
        {
            return _textual;
        }

        /// <summary>
        ///   Gets the display of the filter field.</summary>
        /// <returns>
        ///   The field display of the filter field.</returns>
        public LocalText GetTitle()
        {
            return _title;
        }

        /// <summary>
        ///   Gets the type of the filter field.</summary>
        /// <returns>
        ///   The field type of the filter field.</returns>
        public string GetHandler()
        {
            return _handler;
        }

        /// <summary>
        ///   Gets the type of the filter field.</summary>
        /// <returns>
        ///   The field type of the filter field.</returns>
        public Dictionary<string, object> GetParams()
        {
            return _params;
        }

        /// <summary>
        ///   Serialize/deserialize a SortBy object as string</summary>
        /// <typeparam name="T">
        ///   Type</typeparam>
        public class Converter : JsonConverter
        {
            /// <summary>
            ///   Writes the JSON representation of the object.</summary>
            /// <param name="writer">
            ///   The <see cref="JsonWriter"/> to write to.</param>
            /// <param name="value">
            ///   The value.</param>
            /// <param name="serializer">
            ///   The calling serializer.</param>
            public override void WriteJson(JsonWriter jw, object value, JsonSerializer serializer)
            {
                var v = (FilterField)value;

                jw.WriteStartObject();

                if (null != v._name)
                {
                    jw.WritePropertyName("Name");
                    jw.WriteValue(v._name);
                }

                if (null != v._textual)
                {
                    jw.WritePropertyName("Textual");
                    jw.WriteValue(v._textual);
                }

                if (null != v._title)
                {
                    jw.WritePropertyName("Title");
                    jw.WriteValue(v._title);
                }

                if (null != v._handler)
                {
                    jw.WritePropertyName("Handler");
                    jw.WriteValue(v._handler);
                }

                if (null != v._params &&
                    v._params.Count > 0)
                {
                    jw.WritePropertyName("Params");
                    jw.WriteRawValue(v._params.ToJsonString());
                }

                if (v._isRequired)
                {
                    jw.WritePropertyName("Required");
                    jw.WriteValue(v._isRequired);
                }

                jw.WriteEndObject();

            }

            /// <summary>
            ///   Determines whether this instance can convert the specified object type.</summary>
            /// <param name="objectType">
            ///   Type of the object.</param>
            /// <returns>
            ///   True if this instance can convert the specified object type; otherwise, false.</returns>
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(FilterField);
            }

            /// <summary>
            ///   Gets a value indicating whether this <see cref="JsonConverter"/> can read JSON.</summary>
            /// <value>
            ///   True if this <see cref="JsonConverter"/> can write JSON; otherwise, false.</value>
            public override bool CanRead
            {
                get { return false; }
            }

            /// <summary>
            ///   Gets a value indicating whether this <see cref="JsonConverter"/> can write JSON.</summary>
            /// <value>
            ///   True if this <see cref="JsonConverter"/> can write JSON; otherwise, false.</value>
            public override bool CanWrite
            {
                get { return true; }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        string IFilterField.Textual
        {
            get { return _textual; }
            set { Textual(value); }
        }

        LocalText IFilterField.Title
        {
            get { return _title; }
            set { Title(value); }
        }

        string IFilterField.Handler
        {
            get { return _handler; }
            set { Handler(value); }
        }

        bool IFilterField.IsRequired
        {
            get { return _isRequired; }
            set { _isRequired = value; }
        }

        Dictionary<string, object> IFilterField.Params 
        { 
            get { return GetParams(); }
            set { _params = value; }
        }

        string IFilterField.Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}