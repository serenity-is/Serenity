using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// Metadata for a single KML layer, in JSON format.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class KmlLayerMetadata
    {
        /// <summary>
        /// The layer's &lt;atom:author&gt;, extracted from the layer markup.
        /// </summary>
        [IntrinsicProperty]
        public KmlAuthor Author
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The layer's &lt;description&gt;, extracted from the layer markup.
        /// </summary>
        [IntrinsicProperty]
        public string Description
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The layer's &lt;name&gt;, extracted from the layer markup.
        /// </summary>
        [IntrinsicProperty]
        public string Name
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The layer's &lt;Snippet&gt;, extracted from the layer markup
        /// </summary>
        [IntrinsicProperty]
        public string Snippet
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}