using System;
using System.Collections.Generic;

namespace google.maps
{
    public partial class StreetViewService
    {
        /// <summary>
        /// Retrieves the data for the given pano id and passes it to the provided callback as a StreetViewPanoramaData object.  Pano ids are unique per panorama and stable for the lifetime of a session, but are liable to change between sessions.
        /// </summary>
        public void GetPanoramaById(string pano, Callback callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the StreetViewPanoramaData for a panorama within a given radius of the given LatLng.  The StreetViewPanoramaData is passed to the provided callback. If the radius is less than 50 meters, the nearest panorama will be returned.
        /// </summary>
        public void GetPanoramaByLocation(LatLng latlng, double radius, Callback callback)
        {
            throw new NotImplementedException();
        }

        public delegate void Callback(StreetViewPanoramaData data, StreetViewStatus status);
    }
}