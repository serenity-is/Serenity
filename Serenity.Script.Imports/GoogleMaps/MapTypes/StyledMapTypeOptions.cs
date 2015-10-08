using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class StyledMapTypeOptions
    {
        /// <summary>
        /// Alt text to display when this MapType's button is hovered over in the map type control.
        /// </summary>
        [IntrinsicProperty]
        public string Alt
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// A StyledMapType whose style should be used as a base for defining a StyledMapType's style.  The MapTypeStyle rules will be appended to the base's styles.
        /// </summary>
        [IntrinsicProperty]
        public StyledMapType BaseMapType
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The maximum zoom level for the map when displaying this MapType. Optional.
        /// </summary>
        [IntrinsicProperty]
        public double MaxZoom
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The minimum zoom level for the map when displaying this MapType. Optional.
        /// </summary>
        [IntrinsicProperty]
        public double MinZoom
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Name to display in the map type control.
        /// </summary>
        [IntrinsicProperty]
        public string Name
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}