using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class MapCanvasProjection
    {
        /// <summary>
        /// Computes the geographical coordinates from pixel coordinates in the map's container.
        /// </summary>
        public LatLng FromContainerPixelToLatLng(Point pixel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Computes the geographical coordinates from pixel coordinates in the div that holds the draggable map.
        /// </summary>
        public LatLng FromDivPixelToLatLng(Point pixel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Computes the pixel coordinates of the given geographical location in the DOM element the map's outer container.
        /// </summary>
        public Point FromLatLngToContainerPixel(LatLng latLng)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Computes the pixel coordinates of the given geographical location in the DOM element that holds the draggable map.
        /// </summary>
        public Point FromLatLngToDivPixel(LatLng latLng)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The width of the world in pixels in the current zoom level.
        /// </summary>
        public double GetWorldWidth()
        {
            throw new NotImplementedException();
        }
    }
}