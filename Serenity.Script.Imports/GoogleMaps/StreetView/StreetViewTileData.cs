using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// The properties of the tile set used in a Street View panorama.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class StreetViewTileData
    {
        /// <summary>
        /// Gets the tile image URL for the specified tile. pano is the panorama ID of the Street View tile. tileZoom is the zoom level of the tile. tileX is the x-coordinate of the tile. tileY is the y-coordinate of the tile. Returns the URL for the tile image.
        /// </summary>
        public string GetTileUrl(string pano, double tileZoom, double tileX, double tileY)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The heading (in degrees) at the center of the panoramic tiles.
        /// </summary>
        [IntrinsicProperty]
        public double CenterHeading
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The size (in pixels) at which tiles will be rendered. This may not be the native tile image size.
        /// </summary>
        [IntrinsicProperty]
        public Size TileSize
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The size (in pixels) of the whole panorama's "world".
        /// </summary>
        [IntrinsicProperty]
        public Size WorldSize
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}