using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class MapType
    {
        /// <summary>
        /// Returns a tile for the given tile coordinate (x, y) and zoom level. This tile will be appended to the given ownerDocument.
        /// </summary>
        public System.Html.Element GetTile(Point tileCoord, double zoom, System.Html.Element ownerDocument)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Releases the given tile, performing any necessary cleanup. The provided tile will have already been removed from the document. Optional.
        /// </summary>
        public void ReleaseTile(System.Html.Element tile)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Alt text to display when this MapType's button is hovered over in the MapTypeControl. Optional.
        /// </summary>
        [IntrinsicProperty]
        public string Alt
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The maximum zoom level for the map when displaying this MapType. Required for base MapTypes, ignored for overlay MapTypes.
        /// </summary>
        [IntrinsicProperty]
        public double MaxZoom
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The minimum zoom level for the map when displaying this MapType. Optional; defaults to 0.
        /// </summary>
        [IntrinsicProperty]
        public double MinZoom
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Name to display in the MapTypeControl. Optional.
        /// </summary>
        [IntrinsicProperty]
        public string Name
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The Projection used to render this MapType. Optional; defaults to Mercator.
        /// </summary>
        [IntrinsicProperty]
        public Projection Projection
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Radius of the planet for the map, in meters. Optional; defaults to Earth's equatorial radius of 6378137 meters.
        /// </summary>
        [IntrinsicProperty]
        public double Radius
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The dimensions of each tile. Required.
        /// </summary>
        [IntrinsicProperty]
        public Size TileSize
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}