using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps.adsense
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class AdUnitOptions
    {
        /// <summary>
        /// The AdSense For Content channel number for tracking the performance of this AdUnit. It must be stored as a string as it will typically be a large UINT64. (Optional)
        /// </summary>
        [IntrinsicProperty]
        public string ChannelNumber
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// the Format of the AdUnit. See https://google.com/adsense/adformats (Optional)
        /// </summary>
        [IntrinsicProperty]
        public AdFormat Format
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The map associated with this AdUnit
        /// </summary>
        [IntrinsicProperty]
        public Map Map
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The position of the AdUnit.  If specified, the AdUnit will be displayed at this position.  Otherwise, it will not be added to the map. (Optional)
        /// </summary>
        [IntrinsicProperty]
        public ControlPosition Position
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Your AdSense for Content publisher ID.  Required and must be set at the time of initialization. (Required)
        /// </summary>
        [IntrinsicProperty]
        public string PublisherId
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}