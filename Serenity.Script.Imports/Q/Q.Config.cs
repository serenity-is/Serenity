using jQueryApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static partial class Q
    {
        [Imported]
        public static class Config
        {
            /// <summary>
            /// Variable that holds web applications root path. Should be set when page is loaded.
            /// It can be also set by a link in page header like link id="ApplicationPath" value="/MyApplication/"
            /// </summary>
            public static string ApplicationPath
            {
                [InlineCode("Q.Config.applicationPath")]
                get;
                [InlineCode("Q.Config.applicationPath = {value}")]
                set;
            }

            /// <summary>
            /// Flags that controls overriding jQueryValidation email method to allow only ascii characters (default true)
            /// </summary>
            public static bool EmailAllowOnlyAscii
            {
                [InlineCode("Q.Config.emailAllowOnlyAscii")]
                get;
                [InlineCode("Q.Config.emailAllowOnlyAscii = {value}")]
                set;
            }

            /// <summary>
            /// Variable that holds list of application namespaces to search for objects
            /// </summary>
            public static List<string> RootNamespaces
            {
                [InlineCode("Q.Config.rootNamespaces")]
                get { return null; }
            }

            /// <summary>
            /// A callback that will handle login in case of ajax error
            /// </summary>
            public static Func<ServiceCallOptions, ServiceResponse, bool> NotLoggedInHandler
            {
                [InlineCode("Q.Config.notLoggedInHandler")]
                get;
                [InlineCode("Q.Config.notLoggedInHandler = {value}")]
                set;
            }
        }
    }
}