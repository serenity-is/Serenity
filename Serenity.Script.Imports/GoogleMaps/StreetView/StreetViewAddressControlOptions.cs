using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// Options for the rendering of the Street View address control. 
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class StreetViewAddressControlOptions
    {
        /// <summary>
        /// Position id. This id is used to specify the position of the control on the map. The default position is TOP_LEFT.
        /// </summary>
        [IntrinsicProperty]
        public ControlPosition Position
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// CSS styles to apply to the Street View address control.  For example, {backgroundColor: 'red'}.
        /// </summary>
        [IntrinsicProperty]
        public object Style
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}