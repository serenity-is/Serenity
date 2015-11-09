using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class DirectionsStep
    {
        /// <summary>
        /// The distance covered by this step.  This property may be undefined as the distance may be unknown.
        /// </summary>
        [IntrinsicProperty]
        public Distance Distance
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The typical time required to perform this step in seconds and in text form.  This property may be undefined as the duration may be unknown.
        /// </summary>
        [IntrinsicProperty]
        public Duration Duration
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The ending location of this step.
        /// </summary>
        [IntrinsicProperty]
        public LatLng End_location
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Instructions for this step.
        /// </summary>
        [IntrinsicProperty]
        public string Instructions
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// A sequence of LatLngs describing the course of this step.
        /// </summary>
        [IntrinsicProperty]
        public List<LatLng> Path
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The starting location of this step.
        /// </summary>
        [IntrinsicProperty]
        public LatLng Start_location
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The mode of travel used in this step.
        /// </summary>
        [IntrinsicProperty]
        public TravelMode Travel_mode
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}