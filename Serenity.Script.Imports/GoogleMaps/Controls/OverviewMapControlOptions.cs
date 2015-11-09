using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class OverviewMapControlOptions
    {
        /// <summary>
        /// Whether the control should display in opened mode or collapsed (minimized) mode.  By default, the control is closed.
        /// </summary>
        [IntrinsicProperty]
        public bool Opened
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}