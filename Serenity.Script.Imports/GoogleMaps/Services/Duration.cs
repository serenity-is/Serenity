// Duration.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [IgnoreNamespace]
    [Imported]
    [ScriptName("Object")]
    public sealed partial class Duration
    {
        /// <summary>
        /// A string representation of the duration value.
        /// </summary>
        [IntrinsicProperty]
        public string Text
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The duration in seconds.
        /// </summary>
        [IntrinsicProperty]
        public double Value
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}
