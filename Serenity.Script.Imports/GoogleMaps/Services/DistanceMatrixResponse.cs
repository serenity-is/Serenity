// DistanceMatrixResponse.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// The response to a DistanceMatrixService request, consisting of the formatted origin and destination addresses, and a sequence of DistanceMatrixResponseRows, one for each corresponding origin address.
    /// </summary>
    [IgnoreNamespace]
    [Imported]
    [ScriptName("Object")]
    public sealed partial class DistanceMatrixResponse : Record
    {
        /// <summary>
        /// The formatted destination addresses.
        /// </summary>
        [IntrinsicProperty]
        public List<string > DestinationAddresses
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The formatted origin addresses.
        /// </summary>
        [IntrinsicProperty]
        public List<string > OriginAddresses
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The rows of the matrix, corresponding to the origin addresses.
        /// </summary>
        [IntrinsicProperty]
        public List<DistanceMatrixResponseRow> Rows
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}
