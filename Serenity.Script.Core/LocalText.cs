using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Class that holds local text dictionary
    /// </summary>
    [ScriptName("Q$LT"), IgnoreNamespace]
    public class LocalText
    {
        internal static JsDictionary<string, string> table = new JsDictionary<string, string>();

        static LocalText()
        {
        }

        private string key;

        /// <summary>
        ///   Creates a new local text object containing the given key.</summary>
        /// <param name="textKey">
        ///   Text key</param>
        public LocalText(string key)
        {
            this.key = key;
        }

        /// <summary>
        ///   Converts the <see cref="TextKey">text key</see>, 
        ///   local text object contains to its textual representation</see>.</summary>
        public string Get()
        {
            return LocalText.table[key] ?? key ?? "";
        }

        /// <summary>
        ///   Converts the <see cref="TextKey">text key</see>, 
        ///   local text object contains to its textual representation</see>.</summary>
        public override string ToString()
        {
            return LocalText.table[key] ?? key ?? "";
        }

        /// <summary>
        ///   Implicit operator overload that lets LocalText to be converted to string.</summary>
        /// <param name="localText">
        ///   Local text.</param>
        /// <returns>
        ///   Textual representation of the local text key.</returns>
        [InlineCode("{lt}.get()")]
        public static implicit operator string(LocalText lt)
        {
            return lt.ToString();
        }

        /// <summary>
        ///   Implicit operator overload that lets string to be converted to a LocalText.</summary>
        /// <param name="textKey">
        ///   Local text key.</param>
        /// <returns>
        ///   A local text object containing given key.</returns>
        [InlineCode("new Q$LT({key})")]
        public static implicit operator LocalText(string key)
        {
            if (Object.ReferenceEquals(key, null) || key.Length == 0)
                return LocalText.Empty;
            else
                return new LocalText(key);
        }

        /// <summary>
        ///   An empty local text similar to <c>String.Empty</c>.</summary>
        public static LocalText Empty = new LocalText("");

        /// <summary>
        /// Use to add local text to dictionary
        /// </summary>
        /// <param name="obj">Object with local text list in a format like Enums:{WeekDays:{Monday: 'Pazartesi', ... }}} to
        /// "Enums.WeekDays.Monday" = "Pazartesi"</param>
        /// <param name="prefix">Prefix to add to all keys</param>
        /// <remarks>Obj is provided in a compact format to keep size as small as possible.</remarks>
        public static void Add(JsDictionary<string, object> obj, string prefix)
        {
            if (!Script.IsValue(obj))
                return;

            prefix = prefix ?? "";

            foreach (var k in obj.As<JsDictionary<string, object>>().Keys)
            {
                var actual = prefix + k;
                var o = obj[k];
                if (Script.TypeOf(o) == "object")
                    Add(o.As<JsDictionary<string, object>>(), actual + ".");
                else
                    table[actual] = o.As<string>();
            }
        }

        /// <summary>
        /// Initializes a static class that contains static LocalText objects. These objects actually
        /// contains localized texts in default language, but this method adds them to local text
        /// dictionary and replaces their values with text keys generated from concatting a prefix
        /// and the variable name.
        /// </summary>
        /// <param name="type">Type to initialize that contains texts</param>
        /// <param name="prefix">Prefix to add text keys (e.g. "Db.SomeCategory.")</param>
        public static void InitializeTextClass(Type type, string prefix)
        {
            dynamic t = type;

            foreach (var member in Object.Keys((type)).Clone())
            {

                
                object value = t[member];
                if (Type.IsInstanceOfType(value, typeof(LocalText)))
                {
                    var lt = value.As<LocalText>();
                    var key = prefix + member;
                    table[key] = lt.key;
                    t[member] = new LocalText(key);
                }
            }
        }

        /// <summary>
        /// Tries to get localized text or if not found, default
        /// </summary>
        /// <param name="key">Text key</param>
        /// <param name="defaultText">Default text</param>
        /// <returns>Localized text or default</returns>
        public static string GetDefault(string key, string defaultText)
        {
            return table[key] ?? defaultText ?? key ?? "";
        }
    }
}