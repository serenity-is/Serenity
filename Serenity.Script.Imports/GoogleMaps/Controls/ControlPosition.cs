using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public enum ControlPosition
    {
        /// <summary>
        /// Elements are positioned in the center of the bottom row.
        /// </summary>
        BOTTOM_CENTER,
        /// <summary>
        /// Elements are positioned in the bottom left and flow towards the middle.  Elements are positioned to the right of the Google logo.
        /// </summary>
        BOTTOM_LEFT,
        /// <summary>
        /// Elements are positioned in the bottom right and flow towards the middle.  Elements are positioned to the left of the copyrights.
        /// </summary>
        BOTTOM_RIGHT,
        /// <summary>
        /// Elements are positioned on the left, above bottom-left elements, and flow upwards.
        /// </summary>
        LEFT_BOTTOM,
        /// <summary>
        /// Elements are positioned in the center of the left side.
        /// </summary>
        LEFT_CENTER,
        /// <summary>
        /// Elements are positioned on the left, below top-left elements, and flow downwards.
        /// </summary>
        LEFT_TOP,
        /// <summary>
        /// Elements are positioned on the right, above bottom-right elements, and flow upwards.
        /// </summary>
        RIGHT_BOTTOM,
        /// <summary>
        /// Elements are positioned in the center of the right side.
        /// </summary>
        RIGHT_CENTER,
        /// <summary>
        /// Elements are positioned on the right, below top-right elements, and flow downwards.
        /// </summary>
        RIGHT_TOP,
        /// <summary>
        /// Elements are positioned in the center of the top row.
        /// </summary>
        TOP_CENTER,
        /// <summary>
        /// Elements are positioned in the top left and flow towards the middle.
        /// </summary>
        TOP_LEFT,
        /// <summary>
        /// Elements are positioned in the top right and flow towards the middle.
        /// </summary>
        TOP_RIGHT,
    }
}