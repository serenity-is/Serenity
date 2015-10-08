using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class ImageMapTypeOptions
    {
        /// <summary>
        /// Alt text to display when this MapType's button is hovered over in the MapTypeControl.
        /// </summary>
        [IntrinsicProperty]
        public string Alt
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The maximum zoom level for the map when displaying this MapType.
        /// </summary>
        [IntrinsicProperty]
        public double MaxZoom
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The minimum zoom level for the map when displaying this MapType. Optional.
        /// </summary>
        [IntrinsicProperty]
        public double MinZoom
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Name to display in the MapTypeControl.
        /// </summary>
        [IntrinsicProperty]
        public string Name
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The opacity to apply to the tiles.  The opacity should be specified as a float value between 0 and 1.0, where 0 is fully transparent and 1 is fully opaque.
        /// </summary>
        [IntrinsicProperty]
        public double Opacity
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The tile size.
        /// </summary>
        [IntrinsicProperty]
        public Size TileSize
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}