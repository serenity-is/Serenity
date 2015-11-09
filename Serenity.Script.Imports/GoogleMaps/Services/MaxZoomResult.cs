using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class MaxZoomResult
    {
        /// <summary>
        /// Status of the request.
        /// </summary>
        [IntrinsicProperty]
        public MaxZoomStatus Status
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The maximum zoom level found at the given LatLng.
        /// </summary>
        [IntrinsicProperty]
        public double Zoom
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}