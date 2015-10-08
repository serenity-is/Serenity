using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public enum DirectionsStatus
    {
        /// <summary>
        /// The DirectionsRequest provided was invalid.
        /// </summary>
        INVALID_REQUEST,
        /// <summary>
        /// Too many DirectionsWaypoints were provided in the DirectionsRequest.  The total allowed waypoints is 8, plus the origin and destination.
        /// </summary>
        MAX_WAYPOINTS_EXCEEDED,
        /// <summary>
        /// At least one of the origin, destination, or waypoints could not be geocoded.
        /// </summary>
        NOT_FOUND,
        /// <summary>
        /// The response contains a valid DirectionsResult.
        /// </summary>
        OK,
        /// <summary>
        /// The webpage has gone over the requests limit in too short a period of time.
        /// </summary>
        OVER_QUERY_LIMIT,
        /// <summary>
        /// The webpage is not allowed to use the directions service.
        /// </summary>
        REQUEST_DENIED,
        /// <summary>
        /// A directions request could not be processed due to a server error. The request may succeed if you try again.
        /// </summary>
        UNKNOWN_ERROR,
        /// <summary>
        /// No route could be found between the origin and destination.
        /// </summary>
        ZERO_RESULTS,
    }
}