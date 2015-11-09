using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public partial class Point
    {
        /// <summary>
        /// A point on a two-dimensional plane.
        /// </summary>
        public Point(double x, double y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Compares two Points
        /// </summary>
        public bool Equals(Point other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string representation of this Point.
        /// </summary>
        public override string ToString()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The X coordinate
        /// </summary>
        [IntrinsicProperty]
        public double X
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// The Y coordinate
        /// </summary>
        [IntrinsicProperty]
        public double Y
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}