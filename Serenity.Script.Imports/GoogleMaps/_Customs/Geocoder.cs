using System;
using System.Collections.Generic;

namespace google.maps
{
    public partial class Geocoder
    {
        /// <summary>
        /// Geocode a request.
        /// </summary>
        public void Geocode(GeocoderRequest request, GeocoderDelegate callback)
        {
            throw new NotImplementedException();
        }

        public delegate void GeocoderDelegate(List<GeocoderResult> results, GeocoderStatus status);
    }
}