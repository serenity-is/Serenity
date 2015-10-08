using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public enum ZoomControlStyle
    {
        /// <summary>
        /// The default zoom control. The control which DEFAULT maps to will vary according to map size and other factors. It may change in future versions of the API.
        /// </summary>
        DEFAULT,
        /// <summary>
        /// The larger control, with the zoom slider in addition to +/- buttons.
        /// </summary>
        LARGE,
        /// <summary>
        /// A small control with buttons to zoom in and out.
        /// </summary>
        SMALL,
    }
}