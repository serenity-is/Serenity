using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class StreetViewControlOptions
    {
        /// <summary>
        /// Position id. Used to specify the position of the control on the map. The default position is embedded within the navigation (zoom and pan) controls. If this position is empty or the same as that specified in the zoomControlOptions or panControlOptions, the Street View control will be displayed as part of the navigation controls. Otherwise, it will be displayed separately.
        /// </summary>
        [IntrinsicProperty]
        public ControlPosition Position
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}