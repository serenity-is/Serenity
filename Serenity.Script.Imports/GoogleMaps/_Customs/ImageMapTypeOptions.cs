using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    public partial class ImageMapTypeOptions
    {
        /// <summary>
        /// Returns a string (URL) for given tile coordinate (x, y) and zoom level. This function should have a signature of: getTileUrl(Point, number):string
        /// </summary>
        [IntrinsicProperty]
        public ImageMapTypeOptionsDelegate GetTileUrl
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }

    public delegate string ImageMapTypeOptionsDelegate(Point point, double number);
}