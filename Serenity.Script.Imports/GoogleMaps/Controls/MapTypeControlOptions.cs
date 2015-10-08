using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// Options for the rendering of the map type control. 
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class MapTypeControlOptions
    {
        /// <summary>
        /// IDs of map types to show in the control.
        /// </summary>
        [IntrinsicProperty]
        public List<MapTypeId> MapTypeIds
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Position id. Used to specify the position of the control on the map. The default position is TOP_RIGHT.
        /// </summary>
        [IntrinsicProperty]
        public ControlPosition Position
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Style id. Used to select what style of map type control to display.
        /// </summary>
        [IntrinsicProperty]
        public MapTypeControlStyle Style
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}