using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Html;

namespace google.maps
{
    /// <summary>
    /// An overlay that looks like a bubble and is often connected to a marker. This class extends MVCObject.
    /// </summary>
    [Imported]
    public partial class InfoWindow : MVCObject
    {
        /// <summary>
        /// Creates an info window with the given options.  An InfoWindow can be placed on a map at a particular position or above a marker, depending on what is specified in the options. Unless auto-pan is disabled, an InfoWindow will pan the map to make itself visible when it is opened. After constructing an InfoWindow, you must call open to display it on the map.  The user can  click the close button on the InfoWindow to remove it from the map, or the developer can call close() for the same effect.
        /// </summary>
        public InfoWindow()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an info window with the given options.  An InfoWindow can be placed on a map at a particular position or above a marker, depending on what is specified in the options. Unless auto-pan is disabled, an InfoWindow will pan the map to make itself visible when it is opened. After constructing an InfoWindow, you must call open to display it on the map.  The user can  click the close button on the InfoWindow to remove it from the map, or the developer can call close() for the same effect.
        /// </summary>
        public InfoWindow(InfoWindowOptions opts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Closes this InfoWindow by removing it from the DOM structure.
        /// </summary>
        public void Close()
        {
            throw new NotImplementedException();
        }

        public string GetContent()
        {
            throw new NotImplementedException();
        }

        [ScriptName("getContent")]
        public Element GetContentElement()
        {
            throw new NotImplementedException();
        }

        public LatLng GetPosition()
        {
            throw new NotImplementedException();
        }

        public double GetZIndex()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens this InfoWindow on the given map.  Optionally, an InfoWindow can be associated with an anchor.  In the core API, the only anchor is the Marker class.  However, an anchor can be any MVCObject that exposes the position property and optionally anchorPoint for calculating the pixelOffset (see InfoWindowOptions).  The anchorPoint is the offset from the anchor's position to the tip of the InfoWindow.
        /// </summary>
        public void Open()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens this InfoWindow on the given map.  Optionally, an InfoWindow can be associated with an anchor.  In the core API, the only anchor is the Marker class.  However, an anchor can be any MVCObject that exposes the position property and optionally anchorPoint for calculating the pixelOffset (see InfoWindowOptions).  The anchorPoint is the offset from the anchor's position to the tip of the InfoWindow.
        /// </summary>
        public void Open(Map map)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens this InfoWindow on the given map.  Optionally, an InfoWindow can be associated with an anchor.  In the core API, the only anchor is the Marker class.  However, an anchor can be any MVCObject that exposes the position property and optionally anchorPoint for calculating the pixelOffset (see InfoWindowOptions).  The anchorPoint is the offset from the anchor's position to the tip of the InfoWindow.
        /// </summary>
        public void Open(Map map, MVCObject anchor)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens this InfoWindow on the given map.  Optionally, an InfoWindow can be associated with an anchor.  In the core API, the only anchor is the Marker class.  However, an anchor can be any MVCObject that exposes the position property and optionally anchorPoint for calculating the pixelOffset (see InfoWindowOptions).  The anchorPoint is the offset from the anchor's position to the tip of the InfoWindow.
        /// </summary>
        public void Open(StreetViewPanorama map)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens this InfoWindow on the given map.  Optionally, an InfoWindow can be associated with an anchor.  In the core API, the only anchor is the Marker class.  However, an anchor can be any MVCObject that exposes the position property and optionally anchorPoint for calculating the pixelOffset (see InfoWindowOptions).  The anchorPoint is the offset from the anchor's position to the tip of the InfoWindow.
        /// </summary>
        public void Open(StreetViewPanorama map, MVCObject anchor)
        {
            throw new NotImplementedException();
        }

        public void SetContent(string content)
        {
            throw new NotImplementedException();
        }

        public void SetContent(Element element)
        {
            throw new NotImplementedException();
        }

        public void SetOptions(InfoWindowOptions options)
        {
            throw new NotImplementedException();
        }

        public void SetPosition(LatLng position)
        {
            throw new NotImplementedException();
        }

        public void SetZIndex(double zIndex)
        {
            throw new NotImplementedException();
        }
    }
}