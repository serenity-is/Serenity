using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class DirectionsResult
    {
        /// <summary>
        /// An array of DirectionsRoutes, each of which contains information about the legs and steps of which it is composed.  There will only be one route unless the DirectionsRequest was made with provideRouteAlternatives set to true. (This property was formerly known as "trips".)
        /// </summary>
        [IntrinsicProperty]
        public List<DirectionsRoute> Routes
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}