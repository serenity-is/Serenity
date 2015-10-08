using System;
using System.Collections.Generic;

namespace google.maps
{
    public partial class MaxZoomService
    {
        /// <summary>
        /// Geocode a request.
        /// </summary>
        /// /// <summary>
        /// Returns the maximum zoom level available at a particular LatLng for the Satellite map type. As this request is asynchronous, you must pass a callback function which will be executed upon completion of the request, being passed a MaxZoomResult.
        /// </summary>    
        public void GetMaxZoomAtLatLng(LatLng latlng, Callback callback)
        {
            throw new NotImplementedException();
        }

        public delegate void Callback(MaxZoomResult result);
    }
}