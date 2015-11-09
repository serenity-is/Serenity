using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class DirectionsRoute
    {
        /// <summary>
        /// The bounds for this route.
        /// </summary>
        [IntrinsicProperty]
        public LatLngBounds Bounds
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Copyrights text to be displayed for this route.
        /// </summary>
        [IntrinsicProperty]
        public string Copyrights
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// An array of DirectionsLegs, each of which contains information about the steps of which it is composed.  There will be one leg for each waypoint or destination specified.  So a route with no waypoints will contain one DirectionsLeg and a route with one waypoint will contain two. (This property was formerly known as "routes".)
        /// </summary>
        [IntrinsicProperty]
        public List<DirectionsLeg> Legs
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// An array of LatLngs representing the entire course of this route. The path is simplified in order to make it suitable in contexts where a small number of vertices is required (such as Static Maps API URLs).
        /// </summary>
        [IntrinsicProperty]
        public List<LatLng> Overview_path
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Warnings to be displayed when showing these directions.
        /// </summary>
        [IntrinsicProperty]
        public List<string> Warnings
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If optimizeWaypoints was set to true, this field will contain the re-ordered permutation of the input waypoints. For example, if the input was:   Origin: Los Angeles   Waypoints: Dallas, Bangor, Phoenix   Destination: New York and the optimized output was ordered as follows:   Origin: Los Angeles   Waypoints: Phoenix, Dallas, Bangor   Destination: New York then this field will be an Array containing the values [2, 0, 1].  Note that the numbering of waypoints is zero-based. If any of the input waypoints has stopover set to false, this field will be empty, since route optimization is not available for such queries.
        /// </summary>
        [IntrinsicProperty]
        public List<double> Waypoint_order
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}