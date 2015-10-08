using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Html;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class InfoWindowOptions
    {
        /// <summary>
        /// Content to display in the InfoWindow.  This can be an HTML element, a plain-text string, or a string containing HTML.  The InfoWindow will be sized according to the content.  To set an explicit size for the content, set content to be a HTML element with that size.
        /// </summary>
        [IntrinsicProperty]
        public string Content
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Content to display in the InfoWindow.  This can be an HTML element, a plain-text string, or a string containing HTML.  The InfoWindow will be sized according to the content.  To set an explicit size for the content, set content to be a HTML element with that size.
        /// </summary>
        [IntrinsicProperty]
        [ScriptName("content")]
        public Element Content_Element
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Disable auto-pan on open.  By default, the info window will pan the map so that it is fully visible when it opens.
        /// </summary>
        [IntrinsicProperty]
        public bool DisableAutoPan
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Maximum width of the infowindow, regardless of content's width.  This value is only considered if it is set before a call to open.  To change the maximum width when changing content, call close, setOptions, and then open.
        /// </summary>
        [IntrinsicProperty]
        public double MaxWidth
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The offset, in pixels, of the tip of the info window from the point on the map at whose geographical coordinates the info window is anchored.  If an InfoWindow is opened with an anchor, the pixelOffset will be calculated from the top-center of the anchor's bounds.
        /// </summary>
        [IntrinsicProperty]
        public Size PixelOffset
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The LatLng at which to display this InfoWindow.  If the InfoWindow is opened with an anchor, the anchor's position will be used instead.
        /// </summary>
        [IntrinsicProperty]
        public LatLng Position
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// All InfoWindows are displayed on the map in order of their zIndex, with higher values displaying in front of InfoWindows with lower values. By default, InfoWinodws are displayed according to their latitude, with InfoWindows of lower latitudes appearing in front of InfoWindows at higher latitudes. InfoWindows are always displayed in front of markers.
        /// </summary>
        [IntrinsicProperty]
        public double ZIndex
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}