using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Serenity.Data;

namespace Serenity.Data
{
    [JsonConverter(typeof(FilterFields.Converter))]
    public class FilterFields : IEnumerable<IFilterField>
    {
        private List<IFilterField> inDisplayOrder;
        private Dictionary<string, IFilterField> byNameOrTextual;

        public FilterFields()
        {
            inDisplayOrder = new List<IFilterField>();
            byNameOrTextual = new Dictionary<string, IFilterField>(StringComparer.OrdinalIgnoreCase);
        }

        public void Add(IFilterField field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            inDisplayOrder.Add(field);
            
            byNameOrTextual.Add(field.Name, field);

            string textual = field.Textual.TrimToNull();
            if (textual != null)
                byNameOrTextual.Add(textual, field);
        }

        public IFilterField ByNameOrTextual(string name)
        {
            IFilterField field;
            if (byNameOrTextual.TryGetValue(name, out field))
                return field;

            return null;
        }

        public void Sort()
        {
            this.inDisplayOrder.Sort((f1, f2) => { return String.Compare(f1.Title.ToString(), f2.Title.ToString(), StringComparison.CurrentCultureIgnoreCase); });
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
                var list = (IEnumerable<FilterField>)value;

                if (list == null)
                    throw new ArgumentNullException("list");

                jw.WriteStartArray();
                foreach (var field in list)
                {
                    serializer.Serialize(jw, field);
                }
                jw.WriteEndArray();
            }

            /// <summary>
            ///   Determines whether this instance can convert the specified object type.</summary>
            /// <param name="objectType">
            ///   Type of the object.</param>
            /// <returns>
            ///   True if this instance can convert the specified object type; otherwise, false.</returns>
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(FilterFields) ||
                    objectType.IsSubclassOf(typeof(FilterFields));
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

        IEnumerator<IFilterField> IEnumerable<IFilterField>.GetEnumerator()
        {
            return inDisplayOrder.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return inDisplayOrder.GetEnumerator();
        }
    }
}