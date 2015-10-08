using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// A representation of a location in the Street View panorama.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class StreetViewLocation
    {
        /// <summary>
        /// A localized string describing the location.
        /// </summary>
        [IntrinsicProperty]
        public string Description
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The latlng of the panorama.
        /// </summary>
        [IntrinsicProperty]
        public LatLng LatLng
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// A unique identifier for the panorama. This is stable within a session       but unstable across sessions.
        /// </summary>
        [IntrinsicProperty]
        public string Pano
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}