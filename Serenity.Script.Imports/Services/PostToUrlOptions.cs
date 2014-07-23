using System;
using System.Html;
using System.Runtime.CompilerServices;
using jQueryApi;
using System.Collections.Generic;

namespace Serenity
{
    using System.Collections;

    /// <summary>
    /// Options for the Q.Externals.PostToService function
    /// </summary>
    [Imported, Serializable]
    public class PostToUrlOptions
    {
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
        public object Params { get; set; }
    }
}
