using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class DirectionsLeg
    {
        /// <summary>
        /// The total distance covered by this leg.  This property may be undefined as the distance may be unknown.
        /// </summary>
        [IntrinsicProperty]
        public Distance Distance
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The total duration of this leg.  This property may be undefined as the duration may be unknown.
        /// </summary>
        [IntrinsicProperty]
        public Duration Duration
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The address of the destination of this leg.
        /// </summary>
        [IntrinsicProperty]
        public string End_address
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The DirectionsService calculates directions between locations by using the nearest transportation option (usually a road) at the start and end locations. end_location indicates the actual geocoded destination, which may be different than the end_location of the last step if, for example, the road is not near the destination of this leg.
        /// </summary>
        [IntrinsicProperty]
        public LatLng End_location
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The address of the origin of this leg.
        /// </summary>
        [IntrinsicProperty]
        public string Start_address
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The DirectionsService calculates directions between locations by using the nearest transportation option (usually a road) at the start and end locations. start_location indicates the actual geocoded origin, which may be different than the start_location of the first step if, for example, the road is not near the origin of this leg.
        /// </summary>
        [IntrinsicProperty]
        public LatLng Start_location
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// An array of DirectionsSteps, each of which contains information about the individual steps in this leg.
        /// </summary>
        [IntrinsicProperty]
        public List<DirectionsStep> Steps
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// An array of waypoints along this leg that were not specified in the original request, either as a result of a user dragging the polyline or selecting an alternate route.
        /// </summary>
        [IntrinsicProperty]
        public List<LatLng> Via_waypoints
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}