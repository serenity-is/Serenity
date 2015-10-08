using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class PolylineOptions
    {
        /// <summary>
        /// Indicates whether this Polyline handles click events. Defaults to true.
        /// </summary>
        [IntrinsicProperty]
        public bool Clickable
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Render each edge as a geodesic (a segment of a "great circle"). A geodesic is the shortest path between two points along the surface of the Earth.
        /// </summary>
        [IntrinsicProperty]
        public bool Geodesic
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Map on which to display Polyline.
        /// </summary>
        [IntrinsicProperty]
        public Map Map
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary> 
        /// The ordered sequence of coordinates of the Polyline.  This path may be specified using either a simple array of LatLngs, or an MVCArray of LatLngs.  Note that if you pass a simple array, it will be converted to an MVCArray Inserting or removing LatLngs in the MVCArray will automatically update the polyline on the map.
        /// </summary>
        [IntrinsicProperty]
        public List<LatLng> Path
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