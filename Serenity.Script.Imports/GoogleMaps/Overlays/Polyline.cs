using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// A polyline is a linear overlay of connected line segments on the map. This class extends MVCObject.
    /// </summary>
    [Imported]
    public partial class Polyline : MVCObject
    {
        /// <summary>
        /// Create a polyline using the passed PolylineOptions, which specify both the path of the polyline and the stroke style to use when drawing the polyline. You may pass either an array of LatLngs or an MVCArray of LatLngs when constructing a polyline, though simple arrays are converted to MVCArrays within the polyline upon instantiation.
        /// </summary>
        public Polyline()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a polyline using the passed PolylineOptions, which specify both the path of the polyline and the stroke style to use when drawing the polyline. You may pass either an array of LatLngs or an MVCArray of LatLngs when constructing a polyline, though simple arrays are converted to MVCArrays within the polyline upon instantiation.
        /// </summary>
        public Polyline(PolylineOptions opts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the map on which this poly is attached.
        /// </summary>
        public Map GetMap()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the first path.
        /// </summary>
        public MVCArray GetPath()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Renders this Polyline or Polygon on the specified map.  If map is set to null, the Poly will be removed.
        /// </summary>
        public void SetMap(Map map)
        {
            throw new NotImplementedException();
        }

        public void SetOptions(PolylineOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the first path.  See PolylineOptions for more details.
        /// </summary>
        public void SetPath(List<LatLng> path)
        {
            throw new NotImplementedException();
        }
    }
}