using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// A KmlLayer adds geographic markup to the map from a KML, KMZ or GeoRSS file that is hosted on a publicly accessible web server. A KmlFeatureData object is provided for each feature when clicked. This class extends MVCObject.
    /// </summary>
    [Imported]
    public partial class KmlLayer : MVCObject
    {
        /// <summary>
        /// Creates a KmlLayer which renders the contents of the specified KML/KMZ file (http://code.google.com/apis/kml/documentation/kmlreference.html) or GeoRSS file (www.georss.org).
        /// </summary>
        public KmlLayer(string url)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a KmlLayer which renders the contents of the specified KML/KMZ file (http://code.google.com/apis/kml/documentation/kmlreference.html) or GeoRSS file (www.georss.org).
        /// </summary>
        public KmlLayer(string url, KmlLayerOptions opts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the default viewport for the layer being displayed.
        /// </summary>
        public LatLngBounds GetDefaultViewport()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the map on which the KML Layer is being rendered.
        /// </summary>
        public Map GetMap()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the metadata associated with this layer, as specified in the layer markup.
        /// </summary>
        public KmlLayerMetadata GetMetadata()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the URL of the geographic markup which is being displayed.
        /// </summary>
        public string GetUrl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Renders the KML Layer on the specified map. If map is set to null, the layer is removed.
        /// </summary>
        public void SetMap(Map map)
        {
            throw new NotImplementedException();
        }
    }
}