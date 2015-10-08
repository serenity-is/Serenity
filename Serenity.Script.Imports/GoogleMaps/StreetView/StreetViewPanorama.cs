using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// Displays the panorama for a given LatLng or panorama ID. A StreetViewPanorama object provides a Street View "viewer" which can be stand-alone within a separate <div> or bound to a Map. This class extends MVCObject.
    /// </summary>
    [Imported]
    public partial class StreetViewPanorama : MVCObject
    {
        /// <summary>
        /// Creates a panorama with the passed StreetViewPanoramaOptions.
        /// </summary>
        public StreetViewPanorama(System.Html.Element container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a panorama with the passed StreetViewPanoramaOptions.
        /// </summary>
        public StreetViewPanorama(System.Html.Element container, StreetViewPanoramaOptions opts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the set of navigation links for the Street View panorama.
        /// </summary>
        public List<StreetViewLink> GetLinks()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the current panorama ID for the Street View panorama.  This id is stable within the browser's current session only.
        /// </summary>
        public string GetPano()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the current LatLng position for the Street View panorama.
        /// </summary>
        public LatLng GetPosition()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the current point of view for the Street View panorama.
        /// </summary>
        public StreetViewPov GetPov()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the panorama is visible.  It does not specify whether Street View imagery is available at the specified position.
        /// </summary>
        public bool GetVisible()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the current panorama ID for the Street View panorama.
        /// </summary>
        public void SetPano(string pano)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the current LatLng position for the Street View panorama.
        /// </summary>
        public void SetPosition(LatLng latLng)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the point of view for the Street View panorama.
        /// </summary>
        public void SetPov(StreetViewPov pov)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets to true to make the panorama visible.  If set to false, the panorama will be hidden whether it is embedded in the map or in its own &lt;div&gt;.
        /// </summary>
        public void SetVisible(bool flag)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Additional controls to attach to the panorama. To add a control to the panorama, add the control's &lt;div&gt; to the MVCArray corresponding to the ControlPosition where it should be rendered.
        /// </summary>
        [IntrinsicProperty]
        public List<System.Html.Element> Controls
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}