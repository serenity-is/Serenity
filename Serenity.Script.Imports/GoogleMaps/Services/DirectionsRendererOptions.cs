using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class DirectionsRendererOptions
    {
        /// <summary>
        /// The directions to display on the map and/or in a &lt;div&gt; panel, retrieved as a DirectionsResult object from DirectionsService.
        /// </summary>
        [IntrinsicProperty]
        public DirectionsResult Directions
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If true, allows the user to drag and modify the paths of routes rendered by this DirectionsRenderer.
        /// </summary>
        [IntrinsicProperty]
        public bool Draggable
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// This property indicates whether the renderer should provide UI to select amongst alternative routes.  By default, this flag is false and a user-selectable list of routes will be shown in the directions' associated panel.  To hide that list, set hideRouteList to true.
        /// </summary>
        [IntrinsicProperty]
        public bool HideRouteList
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The InfoWindow in which to render text information when a marker is clicked. Existing info window content will be overwritten and its position moved. If no info window is specified, the DirectionsRenderer will create and use its own info window. This property will be ignored if suppressInfoWindows is set to true.
        /// </summary>
        [IntrinsicProperty]
        public InfoWindow InfoWindow
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Map on which to display the directions.
        /// </summary>
        [IntrinsicProperty]
        public Map Map
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Options for the markers.  All markers rendered by the DirectionsRenderer will use these options.
        /// </summary>
        [IntrinsicProperty]
        public MarkerOptions MarkerOptions
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The &lt;div&gt; in which to display the directions steps.
        /// </summary>
        [IntrinsicProperty]
        public System.Html.Element Panel
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Options for the polylines.  All polylines rendered by the DirectionsRenderer will use these options.
        /// </summary>
        [IntrinsicProperty]
        public PolylineOptions PolylineOptions
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// By default, the input map is centered and zoomed to the bounding box of this set of directions. If this option is set to true, the viewport is left unchanged, unless the map's center and zoom were never set.
        /// </summary>
        [IntrinsicProperty]
        public bool PreserveViewport
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The index of the route within the DirectionsResult object.  The default value is 0.
        /// </summary>
        [IntrinsicProperty]
        public double RouteIndex
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Suppress the rendering of the BicyclingLayer when bicycling directions are requested.
        /// </summary>
        [IntrinsicProperty]
        public bool SuppressBicyclingLayer
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Suppress the rendering of info windows.
        /// </summary>
        [IntrinsicProperty]
        public bool SuppressInfoWindows
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Suppress the rendering of markers.
        /// </summary>
        [IntrinsicProperty]
        public bool SuppressMarkers
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Suppress the rendering of polylines.
        /// </summary>
        [IntrinsicProperty]
        public bool SuppressPolylines
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}