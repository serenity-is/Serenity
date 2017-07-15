using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static partial class Q
    {
        [Imported, ScriptNamespace("Q"), ScriptName("Router")]
        public static class Router
        {
            [InlineCode("Q.Router.navigate({hash}, {tryBack})")]
            public static void Navigate(string hash, bool tryBack = false)
            {
            }

            [InlineCode("Q.Router.replace({hash}, {tryBack})")]
            public static void Replace(string hash, bool tryBack = false)
            {
            }

            [InlineCode("Q.Router.replaceLast({hash}, {tryBack})")]
            public static void ReplaceLast(string hash, bool tryBack = false)
            {
            }

            [InlineCode("Q.Router.resolve({hash})")]
            public static void Resolve(string hash = null)
            {
            }

            [InlineCode("Q.Router.dialog({owner}, {dialog}, {hash})")]
            public static void Dialog(jQueryObject owner, jQueryObject dialog, 
                Func<string> hash)
            {
            }
        }
    }
}