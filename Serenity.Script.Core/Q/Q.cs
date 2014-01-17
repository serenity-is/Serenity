using jQueryApi;
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
    }
}