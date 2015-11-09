using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// The properties of a mouse event on a FusionTablesLayer.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class FusionTablesMouseEvent
    {
        /// <summary>
        /// Pre-rendered HTML content, as placed in the infowindow by the default UI.
        /// </summary>
        [IntrinsicProperty]
        public string InfoWindowHtml
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The position at which to anchor an infowindow on the clicked feature.
        /// </summary>
        [IntrinsicProperty]
        public LatLng LatLng
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The offset to apply to an infowindow anchored on the clicked feature.
        /// </summary>
        [IntrinsicProperty]
        public Size PixelOffset
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// A collection of FusionTablesCell objects, indexed by column name, representing the contents of the table row which included the clicked feature.
        /// </summary>
        [IntrinsicProperty]
        public object Row
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}