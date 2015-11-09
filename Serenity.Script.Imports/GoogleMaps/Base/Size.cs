using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public partial class Size
    {
        /// <summary>
        /// Two-dimensonal size, where width is the distance on the x-axis, and height is the distance on the y-axis.
        /// </summary>
        public Size(double width, double height)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Two-dimensonal size, where width is the distance on the x-axis, and height is the distance on the y-axis.
        /// </summary>
        public Size(double width, double height, string widthUnit)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Two-dimensonal size, where width is the distance on the x-axis, and height is the distance on the y-axis.
        /// </summary>
        public Size(double width, double height, string widthUnit, string heightUnit)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Compares two Sizes.
        /// </summary>
        public bool Equals(Size other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string representation of this Size.
        /// </summary>
        public override string ToString()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The height along the y-axis, in pixels.
        /// </summary>
        [IntrinsicProperty]
        public double Height
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The width along the x-axis, in pixels.
        /// </summary>
        [IntrinsicProperty]
        public double Width
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}