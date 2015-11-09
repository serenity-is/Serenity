using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class GroundOverlayOptions
    {
        /// <summary>
        /// If true, the ground overlay can receive click events.
        /// </summary>
        [IntrinsicProperty]
        public bool Clickable
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The map on which to display the overlay.
        /// </summary>
        [IntrinsicProperty]
        public Map Map
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}