using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Serenity.Data
{
    public static class DictionaryHelper
    {
        /// <summary>
        ///   Gets a dictionary value from a dictionary, returning an empty dictionary if it is not in 
        ///   dictionary.</summary>
        /// <param name="dictionary">
        ///   Dictionary<string, object>() (required).</param>
        /// <param name="key">
        ///   Key (required).</param>
        /// <returns>
        ///   A dictionary</returns>
        public static IDictionary GetDictionary(this IDictionary dictionary,
            string key)
        {
            object obj;
            if (dictionary.Contains(key))
            {
                obj = dictionary[key];
                if (obj != null)
                    return (IDictionary)(obj);
            }
            return new Dictionary<string, object>();
        }

        /// <summary>
        ///   Gets a value from dictionary, returning an empty array if it is not in dictionary</summary>
        /// <param name="dictionary">
        ///   Dictionary<string, object>() (required).</param>
        /// <param name="key">
        ///   Key (required).</param>
        /// <returns>
        ///   An enumerable.</returns>
        public static IEnumerable GetEnumerable(this IDictionary<string, object> dictionary,
            string key)
        {
            object obj;
            if (dictionary.TryGetValue(key, out obj) && obj != null)
                return (IEnumerable)(obj);
            else
                return new object[0];
        }


        /// <summary>
        ///   Gets a value from dictionary, returning a default value if not exists</summary>
        /// <param name="dictionary">
        ///   Dictionary</param>
        /// <param name="key">
        ///   Key</param>
        /// <param name="defaultValue">
        ///   Default value to use if not available</param>
        /// <returns>
        ///   Value or default value</returns>
        public static string GetDefault(this IDictionary<string, string> dictionary,
            string key, string defaultValue)
        {
            string value;
            if (dictionary.TryGetValue(key, out value))
                return value;
            else
                return defaultValue;
        }

        /// <summary>
        ///   Gets a value from dictionary, returning null if not exists</summary>
        /// <param name="dictionary">
        ///   Dictionary</param>
        /// <param name="key">
        ///   Key</param>
        /// <returns>
        ///   Value or default value</returns>
        public static string GetDefault(this IDictionary<string, string> dictionary, string key)
        {
            string value;
            if (dictionary.TryGetValue(key, out value))
                return value;
            else
                return null;
        }

        /// <summary>
        ///   Gets a value from dictionary, returning a default value if it is not in dictionary.</summary>
        /// <param name="dictionary">
        ///   Dictionary<string, object>() (required).</param>
        /// <param name="key">
        ///   Key (required).</param>
        /// <param name="defaultValue">
        ///   Default value to be returned if not in dictionary.</param>
        /// <returns>
        ///   </returns>
        public static object GetDefault(this IDictionary<string, object> dictionary,
            string key, object defaultValue)
        {
            object obj;
            if (dictionary.TryGetValue(key, out obj))
                return obj;
            else
                return defaultValue;
        }

        /// <summary>
        ///   Gets a value from dictionary, returning a default value if it is not in dictionary,
        ///   or converting the value to boolean with invariant culture.</summary>
        /// <param name="dictionary">
        ///   Dictionary<string, object>() (required).</param>
        /// <param name="key">
        ///   Key (required).</param>
        /// <param name="defaultValue">
        ///   Default value to be returned if not in dictionary.</param>
        /// <returns>
        ///   </returns>
        public static bool GetConvert(this IDictionary<string, object> dictionary,
            string key, bool defaultValue)
        {
            object obj;
            if (dictionary.TryGetValue(key, out obj) && obj != null)
                return Convert.ToBoolean(obj, CultureInfo.InvariantCulture);
            else
                return defaultValue;
        }

        /// <summary>
        ///   Gets a value from dictionary, returning a default value if it is not in dictionary,
        ///   or converting the value to Int32 with invariant culture.</summary>
        /// <param name="dictionary">
        ///   Dictionary<string, object>() (required).</param>
        /// <param name="key">
        ///   Key (required).</param>
        /// <param name="defaultValue">
        ///   Default value to be returned if not in dictionary.</param>
        /// <returns>
        ///   </returns>
        public static Int32 GetConvert(this IDictionary<string, object> dictionary,
            string key, Int32 defaultValue)
        {
            object obj;
            if (dictionary.TryGetValue(key, out obj) && obj != null)
                return Convert.ToInt32(obj, Invariants.NumberFormat);
            else
                return defaultValue;
        }

        /// <summary>
        ///   Gets a value from dictionary, returning a default value if it is not in dictionary,
        ///   or converting the value to Int32 with invariant culture.</summary>
        /// <param name="dictionary">
        ///   Dictionary<string, object>() (required).</param>
        /// <param name="key">
        ///   Key (required).</param>
        /// <param name="defaultValue">
        ///   Default value to be returned if not in dictionary.</param>
        /// <returns>
        ///   </returns>
        public static Int64 GetConvert(this IDictionary<string, object> dictionary,
            string key, Int64 defaultValue)
        {
            object obj;
            if (dictionary.TryGetValue(key, out obj) && obj != null)
                return Convert.ToInt64(obj, Invariants.NumberFormat);
            else
                return defaultValue;
        }


        /// <summary>
        ///   Tries to read a field value from a data item as object.</summary>
        /// <param name="dataItem">
        ///   Data item to read field value from. Usually obtained by <see cref="DataBinder.GetDataItem(object)"/>. 
        ///   If specified as <c>null</c>, return value is <c>false</c>.</param>
        /// <param name="field">
        ///   Field to read (required). If no property with that name in data item
        ///   <c>null</c> is the return value.</param>
        /// <returns>
        ///   Field value or null if not available</returns>
        public static object TryAsObject(object dataItem, Field field)
        {
            return TryAsObject(dataItem, field.Name);
        }

        /// <summary>
        ///   Tries to read a property value from a data item as object.</summary>
        /// <param name="dataItem">
        ///   Data item to read property value from. Usually obtained by <see cref="DataBinder.GetDataItem(object)"/>. 
        ///   If specified as <c>null</c>, return value is <c>false</c>.</param>
        /// <param name="propName">
        ///   Property to read. If null or no property with that name in data item
        ///   <c>null</c> is the return value.</param>
        /// <returns>
        ///   Property value or null if not available</returns>
        public static object TryAsObject(object dataItem, string propName)
        {
            object value;
            if (!TryAsObject(dataItem, propName, out value))
                return null;
            else
                return value;
        }


        /// <summary>
        ///   Tries to read a property value from a data item as object.</summary>
        /// <param name="dataItem">
        ///   Data item to read property value from. Usually obtained by <see cref="DataBinder.GetDataItem(object)"/>. 
        ///   If specified as <c>null</c>, return value is <c>false</c>.</param>
        /// <param name="propName">
        ///   Property name to read. If <c>null</c> or no property with that name in data item
        ///   <c>false</c> is the return value.</param>
        /// <param name="propValue">
        ///   When method returns, contains the property value if available, otherwise null.</param>
        /// <returns>
        ///   True if data item has such a property</returns>
        public static bool TryAsObject(object dataItem, string propName, out object propValue)
        {
            propValue = null;

            // dataItem null ise false
            if (dataItem == null)
                return false;

            // propName boş ya da null ise false
            if (string.IsNullOrEmpty(propName))
                return false;

            // istenen property'nin descriptor'ını al
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(dataItem).Find(propName, true);
            if (descriptor == null)
                return false;

            // property değerini oku
            propValue = descriptor.GetValue(dataItem);

            return true;
        }
    }
}
