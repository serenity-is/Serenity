using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class DirectionsRequest
    {
        /// <summary>
        /// If true, instructs the Directions service to avoids highways where possible. Optional.
        /// </summary>
        [IntrinsicProperty]
        public bool AvoidHighways
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If true, instructs the Directions service to avoids toll roads where possible. Optional.
        /// </summary>
        [IntrinsicProperty]
        public bool AvoidTolls
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Location of destination. This can be specified as either a string to be geocoded or a LatLng. Required.
        /// </summary>
        [IntrinsicProperty]
        public LatLng Destination
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Location of destination. This can be specified as either a string to be geocoded or a LatLng. Required.
        /// </summary>
        [IntrinsicProperty]
        [ScriptName("destination")]
        public string DestinationAddress
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If set to true, the DirectionService will attempt to re-order the supplied intermediate waypoints to minimize overall cost of the route. If waypoints are optimized, inspect DirectionsRoute.waypoint_order in the response to determine the new ordering.
        /// </summary>
        [IntrinsicProperty]
        public bool OptimizeWaypoints
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Location of origin. Required.
        /// </summary>
        [IntrinsicProperty]
        public LatLng Origin
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Location of origin. This is either a string to be geocoded. Required.
        /// </summary>
        [IntrinsicProperty]
        [ScriptName("origin")]
        public string OriginAddress
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Whether or not route alternatives should be provided. Optional.
        /// </summary>
        [IntrinsicProperty]
        public bool ProvideRouteAlternatives
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Region code used as a bias for geocoding requests. Optional.
        /// </summary>
        [IntrinsicProperty]
        public string Region
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Type of routing requested. Required.
        /// </summary>
        [IntrinsicProperty]
        public TravelMode TravelMode
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Preferred unit system to use when displaying distance. Defaults to the unit system used in the country of origin.
        /// </summary>
        [IntrinsicProperty]
        public UnitSystem UnitSystem
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Array of intermediate waypoints. Directions will be calculated from the origin to the destination by way of each waypoint in this array. Optional.
        /// </summary>
        [IntrinsicProperty]
        public List<DirectionsWaypoint> Waypoints
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}