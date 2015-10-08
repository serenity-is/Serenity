using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class GeocoderRequest
    {
        /// <summary>
        /// Address. Optional.
        /// </summary>
        [IntrinsicProperty]
        public string Address
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// LatLngBounds within which to search.  Optional.
        /// </summary>
        [IntrinsicProperty]
        public LatLngBounds Bounds
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Preferred language for results.  Optional.
        /// </summary>
        [IntrinsicProperty]
        public string Language
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// LatLng about which to search.  Optional.
        /// </summary>
        [IntrinsicProperty]
        public LatLng Location
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Country code top-level domain within which to search.  Optional.
        /// </summary>
        [IntrinsicProperty]
        public string Region
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}