// DistanceMatrixResponseElement.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// A single element of a response to a DistanceMatrixService request, which contains the duration and distance from one origin to one destination.
    /// </summary>
    [IgnoreNamespace]
    [Imported]
    [ScriptName("Object")]
    public sealed partial class DistanceMatrixResponseElement : Record
    {
        /// <summary>
        /// The distance for this origin-destination pairing. This property may be undefined as the distance may be unknown.
        /// </summary>
        [IntrinsicProperty]
        public Distance Distance
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The duration for this origin-destination pairing. This property may be undefined as the duration may be unknown.
        /// </summary>
        [IntrinsicProperty]
        public Duration Duration
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The status of this particular origin-destination pairing.
        /// </summary>
        [IntrinsicProperty]
        public DistanceMatrixElementStatus Status
        {
            get { throw new NotImplementedException(); }
            set { }
        }

    }
}
