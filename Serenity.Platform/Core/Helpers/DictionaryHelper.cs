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
    }
}
