using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps.adsense
{
    [Imported]
    public partial class AdUnit : MVCObject
    {
        /// <summary>
        /// Creates an AdSense for Content display ad on the associated map.
        /// </summary>
        public AdUnit(System.Html.Element container, AdUnitOptions opts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the channel number in use by this AdUnit.
        /// </summary>
        public string GetChannelNumber()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the containing element of the AdUnit.
        /// </summary>
        public System.Html.Element GetContainer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the format in use by this AdUnit.
        /// </summary>
        public AdFormat GetFormat()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the map to which this AdUnit's ads are targeted.
        /// </summary>
        public Map GetMap()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the ControlPosition at which this AdUnit is displayed on the map.
        /// </summary>
        public ControlPosition GetPosition()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the specified AdSense For Content publisher ID.
        /// </summary>
        public string GetPublisherId()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Specifies the channel number for this AdUnit.  Channel numbers are optional and can be created for Google AdSense tracking.
        /// </summary>
        public void SetChannelNumber(string channelNumber)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Specifies the display format for this AdUnit.
        /// </summary>
        public void SetFormat(AdFormat format)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Associates this AdUnit with the specified map.  Ads will be targeted to the map's viewport.  The map must be specified in order to display ads.
        /// </summary>
        public void SetMap(Map map)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the ControlPosition at which to display the AdUnit on the map.  If the position is set to null, the AdUnit is removed from the map.
        /// </summary>
        public void SetPosition(ControlPosition position)
        {
            throw new NotImplementedException();
        }
    }
}