using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class LocationElevationRequest
    {
        /// <summary>
        /// The discrete locations for which to retrieve elevations.
        /// </summary>
        [IntrinsicProperty]
        public List<LatLng> Locations
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}