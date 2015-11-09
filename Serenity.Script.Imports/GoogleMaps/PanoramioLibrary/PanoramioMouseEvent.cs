using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps.panoramio
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class PanoramioMouseEvent
    {
        /// <summary>
        /// A PanoramioFeature object containing information about the clicked feature.
        /// </summary>
        [IntrinsicProperty]
        public PanoramioFeature FeatureDetails
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Pre-rendered HTML content to display within a feature's InfoWindow when clicked.
        /// </summary>
        [IntrinsicProperty]
        public string InfoWindowHtml
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The position at which to anchor an info window on the clicked feature.
        /// </summary>
        [IntrinsicProperty]
        public LatLng LatLng
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The offset to apply to an info window anchored on the clicked feature.
        /// </summary>
        [IntrinsicProperty]
        public Size PixelOffset
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}