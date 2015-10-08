// DistanceMatrixRequest.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// A distance matrix query sent by the DistanceMatrixService containing arrays of origin and destination locations, and various options for computing metrics.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class DistanceMatrixRequest
    {
        /// <summary>
        /// If true, instructs the Distance Matrix service to avoid highways where possible. Optional.
        /// </summary>
        [IntrinsicProperty]
        public bool AvoidHighways
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If true, instructs the Distance Matrix service to avoid toll roads where possible. Optional.
        /// </summary>
        [IntrinsicProperty]
        public bool AvoidTolls
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Region code used as a bias for geocoding requests. Optional.
        /// </summary>
        [IntrinsicProperty]
        public string Region
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Type of routing requested. Required.
        /// </summary>
        [IntrinsicProperty]
        public TravelMode TravelMode
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Preferred unit system to use when displaying distance. Optional; defaults to metric.
        /// </summary>
        [IntrinsicProperty]
        public UnitSystem UnitSystem
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// An array containing destination LatLngs, to which to calculate distance and time. Required.
        /// </summary>
        [IntrinsicProperty]
        public List<LatLng> Destinations
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// An array containing destination address strings, to which to calculate distance and time. Required.
        /// </summary>
        [IntrinsicProperty]
        [ScriptName("destinations")]
        public List<string> Destinations_Addresses
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// An array containing origin address LatLngs, from which to calculate distance and time. Required.
        /// </summary>
        [IntrinsicProperty]
        public List<LatLng> Origins
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// An array containing origin address strings, from which to calculate distance and time. Required.
        /// </summary>
        [IntrinsicProperty]
        [ScriptName("origins")]
        public List<string > Origins_Addresses
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}
