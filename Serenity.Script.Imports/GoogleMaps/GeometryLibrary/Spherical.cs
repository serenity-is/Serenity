using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps.geometry
{
    /// <summary>
    /// Utility functions for computing geodesic angles, distances and areas. The default radius is Earth's radius of 6378137 meters.
    /// </summary>
    [ScriptName("spherical")]
    [Imported]
    public static class Spherical
    {
        /// <summary>
        /// Computes the area of the given loop. Loops must be closed.
        /// </summary>
        public static double ComputeArea(List<LatLng> loop) { throw new NotImplementedException(); }
        /// <summary>
        /// Computes the area of the given loop. Loops must be closed.
        /// </summary>
        public static double ComputeArea(List<LatLng> loop, double radius) { throw new NotImplementedException(); }
        
        /// <summary>
        /// Computes the distance between two LatLngs.
        /// </summary>
        public static double ComputeDistanceBetween(LatLng from, LatLng to) { throw new NotImplementedException(); }
        /// <summary>
        /// Computes the distance between two LatLngs.
        /// </summary>
        public static double ComputeDistanceBetween(LatLng from, LatLng to, double radius) { throw new NotImplementedException(); }

        /// <summary>
        /// Computes the heading from one LatLng to another LatLng.
        /// </summary>
        public static double ComputeHeading(LatLng from, LatLng to) { throw new NotImplementedException(); }

        /// <summary>
        /// Computes the length of the given path.
        /// </summary>
        public static double ComputeLength(List<LatLng> path) { throw new NotImplementedException(); }
        /// <summary>
        /// Computes the length of the given path.
        /// </summary>
        public static double ComputeLength(List<LatLng> path, double radius) { throw new NotImplementedException(); }

        /// <summary>
        /// Computes the LatLng produced by starting from a given LatLng and heading a given distance.
        /// </summary>
        public static LatLng ComputeOffset(LatLng from, double distance, double heading) { throw new NotImplementedException(); }
        /// <summary>
        /// Computes the LatLng produced by starting from a given LatLng and heading a given distance.
        /// </summary>
        public static LatLng ComputeOffset(LatLng from, double distance, double heading, double radius) { throw new NotImplementedException(); }


        /// <summary>
        /// Travels a fraction of the way from one LatLng to another LatLng.
        /// </summary>
        public static LatLng Interpolate(LatLng from, LatLng to, double fraction) { throw new NotImplementedException(); }
    }
}