using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class GeocoderGeometry
    {
        /// <summary>
        /// The precise bounds of this GeocodeResult, if applicable
        /// </summary>
        [IntrinsicProperty]
        public LatLngBounds Bounds
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The latitude/longitude coordinates of this result
        /// </summary>
        [IntrinsicProperty]
        public LatLng Location
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The type of location returned in location
        /// </summary>
        [IntrinsicProperty]
        public GeocoderLocationType Location_type
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The bounds of the recommended viewport for displaying this GeocodeResult
        /// </summary>
        [IntrinsicProperty]
        public LatLngBounds Viewport
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}