using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class GeocoderResult
    {
        /// <summary>
        /// An array of GeocoderAddressComponents
        /// </summary>
        [IntrinsicProperty]
        public List<GeocoderAddressComponent> Address_components
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// A GeocoderGeometry object
        /// </summary>
        [IntrinsicProperty]
        public GeocoderGeometry Geometry
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// An array of strings denoting the type of the returned geocoded element. A type consists of a unique string identifying the geocode result. (For example, "administrative_area_level_1", "country", etc.)
        /// </summary>
        [IntrinsicProperty]
        public List<string> Types
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}