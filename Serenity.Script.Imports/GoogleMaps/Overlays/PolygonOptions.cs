using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class PolygonOptions
    {
        /// <summary>
        /// Indicates whether this Polygon handles click events. Defaults to true.
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
        /// Render each edge as a geodesic (a segment of a "great circle"). A geodesic is the shortest path between two points along the surface of the Earth.
        /// </summary>
        [IntrinsicProperty]
        public bool Geodesic
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Map on which to display Polygon.
        /// </summary>
        [IntrinsicProperty]
        public Map Map
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The ordered sequence of coordinates that designates a closed loop. Unlike polylines, a polygon may consist of one or more paths.  As a result, the paths property may specify one or more arrays of LatLng coordinates. Simple polygons may be defined using a single array of LatLngs. More complex polygons may specify an array of arrays.  Any simple arrays are convered into MVCArrays. Inserting or removing LatLngs from the MVCArray will automatically update the polygon on the map.
        /// </summary>
        [IntrinsicProperty]
        public List<LatLng> Paths
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