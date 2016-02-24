using jQueryApi;
using System;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Contains core script functions
    /// </summary>
    [IgnoreNamespace]
    public static partial class Q
    {
        static Q()
        {
            var window = ((dynamic)Window.Instance);
            var rsvp = window.RSVP;
            if (Script.IsValue(rsvp) && Script.IsValue(rsvp.on))
                rsvp.on("error", new Action<dynamic>(e =>
                {
                    window.console.log(e);
                    window.console.log((e.get_stack != null ? e.get_stack() : e.stack));
                }));

            window["Q$Externals"] = window.Q;

            jQuery.OnDocumentReady(() =>
            {
                SetMobileDeviceMode();
                Window.OnResize += e => SetMobileDeviceMode();
            });
        }

        /// <summary>
        /// Converts object to its JSON representation (serializes it)
        /// </summary>
        /// <param name="o">Object to convert to JSON</param>
        /// <returns>JSON serialized string</returns>
        [InlineCode("$.toJSON({o})")]
        public static string ToJSON(object o)
        {
            return null;
        }

        /// <summary>
        /// Converts object to its JSON representation (serializes it)
        /// </summary>
        /// <param name="o">Object to convert to JSON</param>
        /// <returns>JSON serialized string</returns>
        [InlineCode("$.toJSON({o})")]
        public static string ToJson(this object o)
        {
            return null;
        }

        /// <summary>
        /// Returns true for any other value than false, 0 (number) or null / undefined. 
        /// String '0' also considered true, unlike numeric 0.
        /// </summary>
        /// <param name="value">Value to be checked</param>
        /// <returns>true, if value is something other than false, 0 or null / undefined, otherwise false</returns>
        [InlineCode("!!({value})")]
        public static bool IsTrue(object value)
        {
            return false;
        }

        /// <summary>
        /// Returns true for only false, 0 (number) or null / undefined. 
        /// String '0' also returns false, unlike numeric 0.
        /// </summary>
        /// <param name="value">Value to be checked</param>
        /// <returns>true  if value is false, 0 or null / undefined, otherwise false</returns>
        [InlineCode("!({value})")]
        public static bool IsFalse(object value)
        {
            return false;
        }

        /// <summary>
        /// Returns true if obj is an array
        /// </summary>
        /// <param name="obj">Object to be checked</param>
        /// <returns>True if obj is an array</returns>
        [InlineCode("ss.isArray({obj})")]
        public static bool IsArray(object obj)
        {
            return false;
        }

        [InlineCode("$({p})")]
        private static jQueryObject J(object p)
        {
            return null;
        }

        [InlineCode("$({p}, {context})")]
        private static jQueryObject J(object p, object context)
        {
            return null;
        }

        [ScriptAlias("Q.deepClone"), ExpandParams, IncludeGenericArguments(false)]
        public static T DeepClone<T>(T source)
        {
            return default(T);
        }

        [ScriptAlias("Q.deepClone"), IncludeGenericArguments(false)]
        public static T DeepExtend<T>(T target, T merge)
        {
            return default(T);
        }

        [ScriptAlias("Q.deepClone"), IncludeGenericArguments(false)]
        public static T DeepExtend<T>(T target, T merge1, T merge2)
        {
            return default(T);
        }

        [ScriptAlias("Q.deepClone"), IncludeGenericArguments(false)]
        public static T DeepExtend<T>(T target, T merge1, T merge2, T merge3)
        {
            return default(T);
        }    
    }
}