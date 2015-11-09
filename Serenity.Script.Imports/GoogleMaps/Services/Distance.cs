// Distance.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [IgnoreNamespace]
    [Imported]
    [ScriptName("Object")]
    public sealed partial class Distance 
    {
        /// <summary>
        /// A string representation of the distance value, using the DirectionsUnitSystem specified in the request.
        /// </summary>
        [IntrinsicProperty]
        public string Text
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The distance in meters.
        /// </summary>
        [IntrinsicProperty]
        public double Value
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}
