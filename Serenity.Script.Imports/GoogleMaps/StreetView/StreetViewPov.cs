using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// A point of view object which specifies the camera's orientation at the Street View panorama's position. The point of view is defined as heading, pitch and zoom.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class StreetViewPov
    {
        /// <summary>
        /// The camera heading in degrees relative to true north. True north is 0°,       east is 90°, south is 180°, west is 270°.
        /// </summary>
        [IntrinsicProperty]
        public double Heading
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The camera pitch in degrees, relative to the street view vehicle.       Ranges from 90° (directly upwards) to -90° (directly downwards).
        /// </summary>
        [IntrinsicProperty]
        public double Pitch
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The zoom level. Fully zoomed-out is level 0, zooming in increases the       zoom level.
        /// </summary>
        [IntrinsicProperty]
        public double Zoom
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}