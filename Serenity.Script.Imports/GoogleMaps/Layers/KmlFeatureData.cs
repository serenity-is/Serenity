using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// Data for a single KML feature in JSON format, returned when a KML feature is clicked. The data contained in this object mirrors that associated with the feature in the KML or GeoRSS markup in which it is declared.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class KmlFeatureData
    {
        /// <summary>
        /// The feature's &lt;atom:author&gt;, extracted from the layer markup (if specified).
        /// </summary>
        [IntrinsicProperty]
        public KmlAuthor Author
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The feature's &lt;description&gt;, extracted from the layer markup.
        /// </summary>
        [IntrinsicProperty]
        public string Description
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The feature's &lt;id&gt;, extracted from the layer markup. If no &lt;id&gt; has been specified, a unique ID will be generated for this feature.
        /// </summary>
        [IntrinsicProperty]
        public string Id
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The feature's balloon styled text, if set.
        /// </summary>
        [IntrinsicProperty]
        public string InfoWindowHtml
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The feature's &lt;name&gt;, extracted from the layer markup.
        /// </summary>
        [IntrinsicProperty]
        public string Name
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The feature's &lt;Snippet&gt;, extracted from the layer markup.
        /// </summary>
        [IntrinsicProperty]
        public string Snippet
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}