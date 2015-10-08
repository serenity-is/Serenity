using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class ZoomControlOptions
    {
        /// <summary>
        /// Position id. Used to specify the position of the control on the map. The default position is TOP_LEFT.
        /// </summary>
        [IntrinsicProperty]
        public ControlPosition Position
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Style id. Used to select what style of zoom control to display.
        /// </summary>
        [IntrinsicProperty]
        public ZoomControlStyle Style
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}