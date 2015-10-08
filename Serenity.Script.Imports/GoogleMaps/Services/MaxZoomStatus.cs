using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public enum MaxZoomStatus
    {
        /// <summary>
        /// There was a problem contacting the Google servers.
        /// </summary>
        ERROR,
        /// <summary>
        /// The response contains a valid MaxZoomResult.
        /// </summary>
        OK,
    }
}