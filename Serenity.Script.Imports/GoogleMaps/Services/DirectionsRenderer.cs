using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public partial class DirectionsRenderer : MVCObject
    {
        /// <summary>
        /// Creates the renderer with the given options.  Directions can be rendered on a map (as visual overlays) or additionally on a &lt;div&gt; panel (as textual instructions).
        /// </summary>
        public DirectionsRenderer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the renderer with the given options.  Directions can be rendered on a map (as visual overlays) or additionally on a &lt;div&gt; panel (as textual instructions).
        /// </summary>
        public DirectionsRenderer(DirectionsRendererOptions opts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the renderer's current set of directions.
        /// </summary>
        public DirectionsResult GetDirections()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the map on which the DirectionsResult is rendered.
        /// </summary>
        public Map GetMap()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the panel &lt;div&gt; in which the DirectionsResult is rendered.
        /// </summary>
        public System.Html.Element GetPanel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the current (zero-based) route index in use by this DirectionsRenderer object.
        /// </summary>
        public double GetRouteIndex()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the renderer to use the result from the DirectionsService. Setting a valid set of directions in this manner will display the directions on the renderer's designated map and panel.
        /// </summary>
        public void SetDirections(DirectionsResult directions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method specifies the map on which directions will be rendered.  Pass null to remove the directions from the map.
        /// </summary>
        public void SetMap(Map map)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Change the options settings of this DirectionsRenderer after initialization.
        /// </summary>
        public void SetOptions(DirectionsRendererOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method renders the directions in a &lt;div&gt;.  Pass null to remove the content from the panel.
        /// </summary>
        public void SetPanel(System.Html.Element panel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the (zero-based) index of the route in the DirectionsResult object to render.  By default, the first route in the array will be rendered.
        /// </summary>
        public void SetRouteIndex(double routeIndex)
        {
            throw new NotImplementedException();
        }
    }
}