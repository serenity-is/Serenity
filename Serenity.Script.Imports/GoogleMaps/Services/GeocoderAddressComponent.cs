using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class GeocoderAddressComponent
    {
        /// <summary>
        /// The full text of the address component
        /// </summary>
        [IntrinsicProperty]
        public string Long_name
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The abbreviated, short text of the given address component
        /// </summary>
        [IntrinsicProperty]
        public string Short_name
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// An array of strings denoting the type of this address component
        /// </summary>
        [IntrinsicProperty]
        public List<string> Types
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}