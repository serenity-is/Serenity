using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// The status returned by the StreetViewService on completion of a Street View request.
    /// </summary>
    [Imported]
    public enum StreetViewStatus
    {
        /// <summary>
        /// The request was successful.
        /// </summary>
        OK,
        /// <summary>
        /// The request could not be successfully processed, yet the exact reason for failure is unknown.
        /// </summary>
        UNKNOWN_ERROR,
        /// <summary>
        /// There are no nearby panoramas.
        /// </summary>
        ZERO_RESULTS,
    }
}