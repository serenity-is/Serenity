using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class PathElevationRequest
    {
        /// <summary>
        /// The path along which to collect elevation values.
        /// </summary>
        [IntrinsicProperty]
        public List<LatLng> Path
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Required. The number of equidistant points along the given path for which to retrieve elevation data, including the endpoints. The number of samples must be a value between 2 and 1024.
        /// </summary>
        [IntrinsicProperty]
        public double Samples
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}