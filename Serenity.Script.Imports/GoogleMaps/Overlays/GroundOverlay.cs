using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public partial class GroundOverlay : MVCObject
    {
        /// <summary>
        /// Creates a ground overlay from the provided image URL and its LatLngBounds. The image is scaled to fit the current bounds, and projected using the current map projection.
        /// </summary>
        public GroundOverlay(string url, LatLngBounds bounds)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a ground overlay from the provided image URL and its LatLngBounds. The image is scaled to fit the current bounds, and projected using the current map projection.
        /// </summary>
        public GroundOverlay(string url, LatLngBounds bounds, GroundOverlayOptions opts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the LatLngBounds of this overlay.
        /// </summary>
        public LatLngBounds GetBounds()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the map on which this ground overlay is displayed.
        /// </summary>
        public Map GetMap()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the url of the projected image.
        /// </summary>
        public string GetUrl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Renders the ground overlay on the specified map. If map is set to null, the overlay is removed.
        /// </summary>
        public void SetMap(Map map)
        {
            throw new NotImplementedException();
        }
    }
}