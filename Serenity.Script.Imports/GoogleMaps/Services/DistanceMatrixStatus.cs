// DistanceMatrixStatus.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// The top-level status about the request in general returned by the DistanceMatrixService upon completion of a distance matrix request.
    /// </summary>
    [Imported]
    public enum DistanceMatrixStatus
    {
        /// <summary>
        /// The provided request was invalid.
        /// </summary>
        INVALID_REQUEST,
        /// <summary>
        /// The request contains more than 25 origins, or more than 25 destinations.
        /// </summary>
        MAX_DIMENSIONS_EXCEEDED,
        /// <summary>
        /// The product of origins and destinations exceeds the per-query limit.
        /// </summary>
        MAX_ELEMENTS_EXCEEDED,
        /// <summary>
        /// The response contains a valid result.
        /// </summary>
        OK,
        /// <summary>
        /// Too many elements have been requested within the allowed time period. The request should succeed if you try again after a reasonable amount of time.
        /// </summary>
        OVER_QUERY_LIMIT,
        /// <summary>
        /// The service denied use of the Distance Matrix service by your web page.
        /// </summary>
        REQUEST_DENIED,
        /// <summary>
        /// A Distance Matrix request could not be processed due to a server error. The request may succeed if you try again.
        /// </summary>
        UNKNOWN_ERROR
    }
}
