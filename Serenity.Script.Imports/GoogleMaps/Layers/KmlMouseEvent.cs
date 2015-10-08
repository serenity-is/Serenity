using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// The properties of a click event on a KML/KMZ or GeoRSS document.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class KmlMouseEvent
    {
        /// <summary>
        /// A KmlFeatureData object, containing information about the clicked feature.
        /// </summary>
        [IntrinsicProperty]
        public KmlFeatureData FeatureData
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
    }
}