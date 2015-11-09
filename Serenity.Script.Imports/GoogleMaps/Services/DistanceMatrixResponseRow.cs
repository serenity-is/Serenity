// DistanceMatrixResponseRow.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// A row of the response to a DistanceMatrixService request, consisting of a sequence of DistanceMatrixResponseElements, one for each corresponding destination address.
    /// </summary>
    [IgnoreNamespace]
    [Imported]
    [ScriptName("Object")]
    public sealed partial class DistanceMatrixResponseRow : Record
    {
        /// <summary>
        /// The row's elements, corresponding to the destination addresses.
        /// </summary>
        [IntrinsicProperty]
        public List<DistanceMatrixResponseElement> Elements
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}
