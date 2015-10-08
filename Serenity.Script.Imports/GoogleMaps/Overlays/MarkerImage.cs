using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public partial class MarkerImage
    {
        /// <summary>
        /// A structure representing a Marker icon or shadow image.
        /// </summary>
        public MarkerImage(string url)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A structure representing a Marker icon or shadow image.
        /// </summary>
        public MarkerImage(string url, Size size)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A structure representing a Marker icon or shadow image.
        /// </summary>
        public MarkerImage(string url, Size size, Point origin)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A structure representing a Marker icon or shadow image.
        /// </summary>
        public MarkerImage(string url, Size size, Point origin, Point anchor)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A structure representing a Marker icon or shadow image.
        /// </summary>
        public MarkerImage(string url, Size size, Point origin, Point anchor, Size scaledSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The position at which to anchor an image in correspondance to the location of the marker on the map. By default, the anchor is located along the center point of the bottom of the image.
        /// </summary>
        [IntrinsicProperty]
        public Point Anchor
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The position of the image within a sprite, if any. By default, the origin is located at the top left corner of the image (0, 0).
        /// </summary>
        [IntrinsicProperty]
        public Point Origin
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The size of the entire image after scaling, if any.  Use this property to stretch/shrink an image or a sprite.
        /// </summary>
        [IntrinsicProperty]
        public Size ScaledSize
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The display size of the sprite or image. When using sprites, you must specify the sprite size. If the size is not provided, it will be set when the image loads.
        /// </summary>
        [IntrinsicProperty]
        public Size Size
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The URL of the image or sprite sheet.
        /// </summary>
        [IntrinsicProperty]
        public string Url
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}