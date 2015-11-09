using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class MapPanes
    {
        /// <summary>
        /// This pane contains the info window. It is above all map overlays. (Pane 6).
        /// </summary>
        [IntrinsicProperty]
        public System.Html.Element FloatPane
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// This pane contains the info window shadow. It is above the overlayImage, so that markers can be in the shadow of the info window. (Pane 4).
        /// </summary>
        [IntrinsicProperty]
        public System.Html.Element FloatShadow
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// This pane is the lowest pane and is above the tiles. It may not receive DOM events. (Pane 0).
        /// </summary>
        [IntrinsicProperty]
        public System.Html.Element MapPane
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// This pane contains the marker foreground images. (Pane 3).
        /// </summary>
        [IntrinsicProperty]
        public System.Html.Element OverlayImage
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// This pane contains polylines, polygons, ground overlays and tile layer overlays. It may not receive DOM events. (Pane 1).
        /// </summary>
        [IntrinsicProperty]
        public System.Html.Element OverlayLayer
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// This pane contains elements that receive DOM mouse events, such as the transparent targets for markers. It is above the floatShadow, so that markers in the shadow of the info window can be clickable. (Pane 5).
        /// </summary>
        [IntrinsicProperty]
        public System.Html.Element OverlayMouseTarget
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// This pane contains the marker shadows. It may not receive DOM events. (Pane 2).
        /// </summary>
        [IntrinsicProperty]
        public System.Html.Element OverlayShadow
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}