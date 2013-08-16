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
            /// </summary>
            public static string ApplicationPath = "/";

            /// <summary>
            /// Variable that holds list of application namespaces to search for objects
            /// </summary>
            public static List<string> RootNamespaces;

            static Config()
            {
                var pathLink = jQuery.Select("link#ApplicationPath");
                if (pathLink.Length > 0)
                    ApplicationPath = pathLink.GetAttribute("href");

                RootNamespaces = new List<string>();
                RootNamespaces.Add("Serenity");
            }
        }
    }
}