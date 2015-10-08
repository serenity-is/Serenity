// DistanceMatrixElementStatus.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// The element-level status about a particular origin-destination pairing returned by the DistanceMatrixService upon completion of a distance matrix request.
    /// </summary>
    [Imported]
    public enum DistanceMatrixElementStatus
    {
        /// <summary>
        /// The origin and/or destination of this pairing could not be geocoded.
        /// </summary>
        NOT_FOUND,
        /// <summary>
        /// The response contains a valid result.
        /// </summary>
        OK,
        /// <summary>
        /// No route could be found between the origin and destination.
        /// </summary>
        ZERO_RESULTS
    }
}
