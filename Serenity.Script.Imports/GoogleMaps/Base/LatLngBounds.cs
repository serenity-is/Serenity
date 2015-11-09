using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public partial class LatLngBounds
    {
        /// <summary>
        /// A LatLngBounds instance represents a rectangle in geographical coordinates, including one that crosses the 180 degrees longitudinal meridian.
        /// </summary>
        public LatLngBounds() { }
        /// <summary>
        /// A LatLngBounds instance represents a rectangle in geographical coordinates, including one that crosses the 180 degrees longitudinal meridian.
        /// </summary>
        public LatLngBounds(LatLng sw) { }
        /// <summary>
        /// A LatLngBounds instance represents a rectangle in geographical coordinates, including one that crosses the 180 degrees longitudinal meridian.
        /// </summary>
        public LatLngBounds(LatLng sw, LatLng ne) { }

        /// <summary>
        /// Returns true if the given lat/lng is in this bounds.
        /// </summary>
        public bool Contains(LatLng latlng) { throw new NotImplementedException(); }

        /// <summary>
        /// Returns true if this bounds approximately equals the given bounds.
        /// </summary>
        public bool Equals(LatLngBounds other) { throw new NotImplementedException(); }

        /// <summary>
        /// Extends this bounds to contain the given point.
        /// </summary>
        public LatLngBounds Extend(LatLng point) { throw new NotImplementedException(); }

        /// <summary>
        /// Computes the center of this LatLngBounds
        /// </summary>
        public LatLng GetCenter() { throw new NotImplementedException(); }

        /// <summary>
        /// Returns the north-east corner of this bounds.
        /// </summary>
        public LatLng GetNorthEast() { throw new NotImplementedException(); }

        /// <summary>
        /// Returns the south-west corner of this bounds.
        /// </summary>
        public LatLng GetSouthWest() { throw new NotImplementedException(); }

        /// <summary>
        /// Returns true if this bounds shares any points with this bounds.
        /// </summary>
        public bool Intersects(LatLngBounds other) { throw new NotImplementedException(); }

        /// <summary>
        /// Returns if the bounds are empty.
        /// </summary>
        public bool IsEmpty() { throw new NotImplementedException(); }

        /// <summary>
        /// Converts the given map bounds to a lat/lng span.
        /// </summary>
        public LatLng ToSpan() { throw new NotImplementedException(); }

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