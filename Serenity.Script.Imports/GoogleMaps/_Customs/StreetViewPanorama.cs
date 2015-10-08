using System;
using System.Collections.Generic;

namespace google.maps
{
    public partial class StreetViewPanorama
    {
        /// <summary>
        /// Set the custom panorama provider called on pano change to load custom panoramas.
        /// </summary>
        public void RegisterPanoProvider(Callback callback)
        {
            throw new NotImplementedException();
        }

        public delegate StreetViewPanoramaData Callback(string input);
    }
}