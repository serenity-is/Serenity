using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// This object defines the properties that can be set on a KmlLayer object.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class KmlLayerOptions
    {
        /// <summary>
        /// If true, the layer receives mouse events. Default value is true.
        /// </summary>
        [IntrinsicProperty]
        public bool Clickable
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The map on which to display the layer.
        /// </summary>
        [IntrinsicProperty]
        public Map Map
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// By default, the input map is centered and zoomed to the bounding box of the contents of the layer. If this option is set to true, the viewport is left unchanged, unless the map's center and zoom were never set.
        /// </summary>
        [IntrinsicProperty]
        public bool PreserveViewport
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Suppress the rendering of info windows when layer features are clicked.
        /// </summary>
        [IntrinsicProperty]
        public bool SuppressInfoWindows
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}