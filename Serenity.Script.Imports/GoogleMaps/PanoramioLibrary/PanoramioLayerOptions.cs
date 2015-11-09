using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps.panoramio
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class PanoramioLayerOptions
    {
        /// <summary>
        /// If true, the layer receives mouse events. Default value is true.
        /// </summary>
        [IntrinsicProperty]
        public bool Clickable
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The map on which to display the layer.
        /// </summary>
        [IntrinsicProperty]
        public Map Map
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Suppress the rendering of info windows when layer features are clicked.
        /// </summary>
        [IntrinsicProperty]
        public bool SuppressInfoWindows
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// A panoramio tag used to filter the photos which are displayed. Only photos which have been tagged with the supplied string will be shown.
        /// </summary>
        [IntrinsicProperty]
        public string Tag
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// A Panoramio user ID. If provided, only photos by this user will be displayed on the map. If both a tag and user ID are provided, the tag will take precedence.
        /// </summary>
        [IntrinsicProperty]
        public string UserId
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}