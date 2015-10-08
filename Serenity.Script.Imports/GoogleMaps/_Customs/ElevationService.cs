using System;
using System.Collections.Generic;

namespace google.maps
{
    public partial class ElevationService
    {
        /// <summary>
        /// Makes an elevation request along a path, where the elevation data are returned as distance-based samples along that path.
        /// </summary>
        public void GetElevationAlongPath(PathElevationRequest request, Callback callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Makes an elevation request for a list of discrete locations.
        /// </summary>
        public void GetElevationForLocations(LocationElevationRequest request, Callback callback)
        {
            throw new NotImplementedException();
        }

        public delegate void Callback(List<ElevationResult> results, ElevationStatus status);
    }
}