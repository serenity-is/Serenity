using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// The representation of a panorama returned from the provider defined using registerPanoProvider.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class StreetViewPanoramaData
    {
        /// <summary>
        /// Specifies the copyright text for this panorama.
        /// </summary>
        [IntrinsicProperty]
        public string Copyright
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Specifies the navigational links to adjacent panoramas.
        /// </summary>
        [IntrinsicProperty]
        public List<StreetViewLink> Links
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Specifies the location meta-data for this panorama.
        /// </summary>
        [IntrinsicProperty]
        public StreetViewLocation Location
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Specifies the custom tiles for this panorama.
        /// </summary>
        [IntrinsicProperty]
        public StreetViewTileData Tiles
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}