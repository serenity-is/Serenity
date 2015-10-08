using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public enum ElevationStatus
    {
        /// <summary>
        /// This request was invalid.
        /// </summary>
        INVALID_REQUEST,
        /// <summary>
        /// The request did not encounter any errors.
        /// </summary>
        OK,
        /// <summary>
        /// The webpage has gone over the requests limit in too short a period of time.
        /// </summary>
        OVER_QUERY_LIMIT,
        /// <summary>
        /// The webpage is not allowed to use the elevation service for some reason.
        /// </summary>
        REQUEST_DENIED,
        /// <summary>
        /// A geocoding, directions or elevation request could not be successfully processed, yet the exact reason for the failure is not known.
        /// </summary>
        UNKNOWN_ERROR,
    }
}