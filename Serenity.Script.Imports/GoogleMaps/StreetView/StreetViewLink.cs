using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// A collection of references to adjacent Street View panos.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class StreetViewLink
    {
        /// <summary>
        /// A localized string describing the link.
        /// </summary>
        [IntrinsicProperty]
        public string Description
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The heading of the link.
        /// </summary>
        [IntrinsicProperty]
        public double Heading
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// A unique identifier for the panorama. This id is stable within a session but unstable across sessions.
        /// </summary>
        [IntrinsicProperty]
        public string Pano
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Color of the link
        /// </summary>
        [IntrinsicProperty]
        public string RoadColor
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Opacity of the link
        /// </summary>
        [IntrinsicProperty]
        public double RoadOpacity
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}