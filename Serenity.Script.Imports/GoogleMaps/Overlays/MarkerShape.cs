using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class MarkerShape
    {
        /// <summary>
        /// The format of this attribute depends on the value of the type and follows the w3 AREA coords specification found at  http://www.w3.org/TR/REC-html40/struct/objects.html#adef-coords. The coords attribute is an array of integers that specify the pixel position of the shape relative to the top-left corner of the target image.  The coordinates depend on the value of type as follows:   - circle: coords is [x1,y1,r] where x1,y2 are the coordinates of the center of the circle, and r is the radius of the circle.   - poly: coords is [x1,y1,x2,y2...xn,yn] where each x,y pair contains the coordinates of one vertex of the polygon.   - rect: coords is [x1,y1,x2,y2] where x1,y1 are the coordinates of the upper-left corner of the rectangle and x2,y2 are the coordinates of the lower-right coordinates of the rectangle.
        /// </summary>
        [IntrinsicProperty]
        public List<double> Coords
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Describes the shape's type and can be circle, poly or rect.
        /// </summary>
        [IntrinsicProperty]
        public string Type
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}