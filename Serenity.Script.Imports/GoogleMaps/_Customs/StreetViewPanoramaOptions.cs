using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    public delegate StreetViewPanoramaData StreetViewPanoramaOptionsDelegate(string input);

    public partial class StreetViewPanoramaOptions
    {
        /// <summary>
        /// Custom panorama provider, which takes a string pano id and returns an object defining the panorama given that id.  This function must be defined to specify custom panorama imagery.
        /// </summary>
        [IntrinsicProperty]
        public StreetViewPanoramaOptionsDelegate PanoProvider
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}