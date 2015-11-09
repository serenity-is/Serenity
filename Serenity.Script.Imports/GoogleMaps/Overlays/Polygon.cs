using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public partial class Polygon : MVCObject
    {
        /// <summary>
        /// Create a polygon using the passed PolygonOptions, which specify the polygon's path, the stroke style for the polygon's edges, and the fill style for the polygon's interior regions. A polygon may contain one or more paths, where each path consists of an array of LatLngs.  You may pass either an array of LatLngs or an MVCArray of LatLngs when constructing these paths.  Arrays are converted to MVCArrays within the polygon upon instantiation.
        /// </summary>
        public Polygon()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a polygon using the passed PolygonOptions, which specify the polygon's path, the stroke style for the polygon's edges, and the fill style for the polygon's interior regions. A polygon may contain one or more paths, where each path consists of an array of LatLngs.  You may pass either an array of LatLngs or an MVCArray of LatLngs when constructing these paths.  Arrays are converted to MVCArrays within the polygon upon instantiation.
        /// </summary>
        public Polygon(PolygonOptions opts)
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
        /// Retrieves the paths for this Polygon.
        /// </summary>
        public MVCArray GetPaths()
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

        public void SetOptions(PolygonOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the first path.  See PolylineOptions for more details.
        /// </summary>
        public void SetPath(MVCArray path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the path for this Polygon.
        /// </summary>
        public void SetPaths(MVCArray paths)
        {
            throw new NotImplementedException();
        }
    }
}