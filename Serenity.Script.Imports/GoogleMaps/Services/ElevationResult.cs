using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class ElevationResult
    {
        /// <summary>
        /// The elevation of this point on Earth, in meters above sea level.
        /// </summary>
        [IntrinsicProperty]
        public double Elevation
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The location of this elevation result.
        /// </summary>
        [IntrinsicProperty]
        public LatLng Location
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}