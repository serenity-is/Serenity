using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// LatLng is a point in geographical coordinates, latitude and longitude.  
    /// Notice that although usual map projections associate longitude with the x-coordinate of the map, and latitude with the y-coordinate, the latitude coordinate is always written first, followed by the longitude.
    /// Notice also that you cannot modify the coordinates of a LatLng. If you want to compute another point, you have to create a new one.
    /// </summary>
    [Imported]
    public partial class LatLng
    {
        /// <summary>
        /// Notice the ordering of latitude and longitude. If the noWrap flag is true, then the numbers will be used as passed, otherwise latitude will be clamped to lie between -90 degrees and +90 degrees, and longitude will be wrapped to lie between -180 degrees and +180 degrees.
        /// </summary>
        public LatLng(double lat, double lng)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Notice the ordering of latitude and longitude. If the noWrap flag is true, then the numbers will be used as passed, otherwise latitude will be clamped to lie between -90 degrees and +90 degrees, and longitude will be wrapped to lie between -180 degrees and +180 degrees.
        /// </summary>
        public LatLng(double lat, double lng, bool noWrap)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Comparison function.
        /// </summary>
        public bool Equals(LatLng other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the latitude in degrees.
        /// </summary>
        public double Lat()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the longitude in degrees.
        /// </summary>
        public double Lng()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts to string representation.
        /// </summary>
        public override string ToString()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string of the form "lat,lng" for this LatLng. We round the lat/lng values to 6 decimal places by default.
        /// </summary>
        public string ToUrlValue()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string of the form "lat,lng" for this LatLng. We round the lat/lng values to 6 decimal places by default.
        /// </summary>
        public string ToUrlValue(double precision)
        {
            throw new NotImplementedException();
        }
    }
}