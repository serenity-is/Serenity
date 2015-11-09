using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class MouseEvent
    {
        /// <summary>
        /// The latitude/longitude that was below the cursor when the event occurred.
        /// </summary>
        [IntrinsicProperty]
        public LatLng LatLng
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}