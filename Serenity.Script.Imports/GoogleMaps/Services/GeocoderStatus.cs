using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public enum GeocoderStatus
    {
        /// <summary>
        /// There was a problem contacting the Google servers.
        /// </summary>
        ERROR,
        /// <summary>
        /// This GeocoderRequest was invalid.
        /// </summary>
        INVALID_REQUEST,
        /// <summary>
        /// The response contains a valid GeocoderResponse.
        /// </summary>
        OK,
        /// <summary>
        /// The webpage has gone over the requests limit in too short a period of time.
        /// </summary>
        OVER_QUERY_LIMIT,
        /// <summary>
        /// The webpage is not allowed to use the geocoder.
        /// </summary>
        REQUEST_DENIED,
        /// <summary>
        /// A geocoding request could not be processed due to a server error. The request may succeed if you try again.
        /// </summary>
        UNKNOWN_ERROR,
        /// <summary>
        /// No result was found for this GeocoderRequest.
        /// </summary>
        ZERO_RESULTS,
    }
}