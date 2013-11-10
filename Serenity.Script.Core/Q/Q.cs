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
        }

        [InlineCode("$.toJSON({o})")]
        public static string ToJSON(object o)
        {
            return null;
        }

        [ScriptSkip]
        public static Exception AsException(this string s)
        {
            return null;
        }

        public static Exception ArgumentNull(this string s)
        {
            return String.Format("{0} is null!", s).AsException();
        }

        [InlineCode("!!({value})")]
        public static bool IsTrue(object value)
        {
            return false;
        }

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