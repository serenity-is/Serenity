using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class DirectionsWaypoint
    {
        /// <summary>
        /// Waypoint location. Can be an address string or LatLng. Optional.
        /// </summary>
        [IntrinsicProperty]
        public LatLng Location
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Waypoint location. Can be an address string or LatLng. Optional.
        /// </summary>
        [IntrinsicProperty]
        [ScriptName("location")]
        public string LocationAddress
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If true, indicates that this waypoint is a stop between the origin and destination.  This has the effect of splitting the route into two.  This value is true by default. Optional.
        /// </summary>
        [IntrinsicProperty]
        public bool Stopover
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}