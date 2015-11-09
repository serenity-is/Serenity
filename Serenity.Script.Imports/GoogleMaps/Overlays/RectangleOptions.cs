using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class RectangleOptions
    {
        /// <summary>
        /// The bounds.
        /// </summary>
        [IntrinsicProperty]
        public LatLngBounds Bounds
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Indicates whether this Rectangle handles click events. Defaults to true.
        /// </summary>
        [IntrinsicProperty]
        public bool Clickable
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The fill color in HTML hex style, ie. "#00AAFF"
        /// </summary>
        [IntrinsicProperty]
        public string FillColor
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The fill opacity between 0.0 and 1.0
        /// </summary>
        [IntrinsicProperty]
        public double FillOpacity
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Map on which to display Rectangle.
        /// </summary>
        [IntrinsicProperty]
        public Map Map
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The stroke color in HTML hex style, ie. "#FFAA00"
        /// </summary>
        [IntrinsicProperty]
        public string StrokeColor
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The stroke opacity between 0.0 and 1.0
        /// </summary>
        [IntrinsicProperty]
        public double StrokeOpacity
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The stroke width in pixels.
        /// </summary>
        [IntrinsicProperty]
        public double StrokeWeight
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The zIndex compared to other polys.
        /// </summary>
        [IntrinsicProperty]
        public double ZIndex
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}