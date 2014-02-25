using jQueryApi;
using System.Collections.Generic;

namespace Serenity
{
    public static partial class Q
    {
        public static class Config
        {
            /// <summary>
            /// Variable that holds web applications root path. Should be set when page is loaded.
            /// It can be also set by a link in page header like link id="ApplicationPath" value="/MyApplication/"
            /// </summary>
            public static string ApplicationPath = "/";

            /// <summary>
            /// Flags that controls overriding jQueryValidation email method to allow only ascii characters (default true)
            /// </summary>
            public static bool EmailAllowOnlyAscii;

            /// <summary>
            /// Variable that holds list of application namespaces to search for objects
            /// </summary>
            public static List<string> RootNamespaces;

            static Config()
            {
                var pathLink = J("link#ApplicationPath");
                if (pathLink.Length > 0)
                    ApplicationPath = pathLink.GetAttribute("href");

                RootNamespaces = new List<string>();
                RootNamespaces.Add("Serenity");

                EmailAllowOnlyAscii = true;
            }
        }
    }
}