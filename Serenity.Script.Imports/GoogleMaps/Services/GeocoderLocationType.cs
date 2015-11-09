using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public enum GeocoderLocationType
    {
        /// <summary>
        /// The returned result is approximate.
        /// </summary>
        APPROXIMATE,
        /// <summary>
        /// The returned result is the geometric center of a result such a line (e.g. street) or polygon (region).
        /// </summary>
        GEOMETRIC_CENTER,
        /// <summary>
        /// The returned result reflects an approximation (usually on a road) interpolated between two precise points (such as intersections). Interpolated results are generally returned when rooftop geocodes are unavilable for a street address.
        /// </summary>
        RANGE_INTERPOLATED,
        /// <summary>
        /// The returned result reflects a precise geocode.
        /// </summary>
        ROOFTOP,
    }
}