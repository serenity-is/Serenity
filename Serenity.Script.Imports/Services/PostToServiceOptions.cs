using System;
using System.Html;
using System.Runtime.CompilerServices;
using jQueryApi;
using System.Collections.Generic;

namespace Serenity
{
    /// <summary>
    /// Options for the Q.Externals.PostToService function
    /// </summary>
    [Imported, Serializable]
    public class PostToServiceOptions
    {
        /// <summary>
        /// Gets or sets service URL
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Gets or sets service URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets target
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets the request object
        /// </summary>
        public Object Request { get; set; }
    }
}
