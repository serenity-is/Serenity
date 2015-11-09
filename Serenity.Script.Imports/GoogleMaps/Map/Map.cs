using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// Converted to Saltarelle from Script# import library sources at https://gmapsharp.codeplex.com/
    /// Many thanks to @djsolid
    /// </summary>
    [Imported]
    public partial class Map : MVCObject
    {
        /// <summary>
        /// Creates a new map inside of the given HTML container, which is typically a DIV element.
        /// </summary>
        public Map(System.Html.Element mapDiv)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new map inside of the given HTML container, which is typically a DIV element.
        /// </summary>
        public Map(System.Html.Element mapDiv, MapOptions opts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the viewport to contain the given bounds.
        /// </summary>
        public void FitBounds(LatLngBounds bounds)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the lat/lng bounds of the current viewport. If the map is not yet initialized (i.e. the mapType is still null), or center and zoom have not been set then the result is null or undefined.
        /// </summary>
        public LatLngBounds GetBounds()
        {
            throw new NotImplementedException();
        }

        public LatLng GetCenter()
        {
            throw new NotImplementedException();
        }

        public System.Html.Element GetDiv()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the compass heading of aerial imagery.  The heading value is measured in degrees (clockwise) from cardinal direction North.
        /// </summary>
        public double GetHeading()
        {
            throw new NotImplementedException();
        }

        public MapTypeId GetMapTypeId()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the current Projection.  If the map is not yet initialized (i.e. the mapType is still null) then the result is null. Listen to projection_changed and check its value to ensure it is not null.
        /// </summary>
        public Projection GetProjection()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the default StreetViewPanorama bound to the map, which may be a default panorama embedded within the map, or the panorama set using setStreetView(). Changes to the map's streetViewControl will be reflected in the display of such a bound panorama.
        /// </summary>
        public StreetViewPanorama GetStreetView()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the angle of incidence for aerial imagery (available for SATELLITE and HYBRID map types) measured in degrees from the viewport plane to the map plane.  A value of 0 indicates no angle of incidence (no tilt) while 45' imagery will return a value of 45.
        /// </summary>
        public double GetTilt()
        {
            throw new NotImplementedException();
        }

        public double GetZoom()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Changes the center of the map by the given distance in pixels.  If the distance is less than both the width and height of the map, the transition will be smoothly animated.  Note that the map coordinate system increases from west to east (for x values) and north to south (for y values).
        /// </summary>
        public void PanBy(double x, double y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Changes the center of the map to the given LatLng.  If the change is less than both the width and height of the map, the transition will be smoothly animated.
        /// </summary>
        public void PanTo(LatLng latLng)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Pans the map by the minimum amount necessary to contain the given LatLngBounds.  It makes no guarantee where on the map the bounds will be, except that as much of the bounds as possible will be visible.  The bounds will be positioned inside the area bounded by the map type and navigation (pan, zoom, and Street View) controls, if they are present on the map.  If the bounds is larger than the map, the map will be shifted to include the northwest corner of the bounds.  If the change in the map's position is less than both the width and height of the map, the transition will be smoothly animated.
        /// </summary>
        public void PanToBounds(LatLngBounds latLngBounds)
        {
            throw new NotImplementedException();
        }

        public void SetCenter(LatLng latlng)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the compass heading for aerial imagery measured in degrees from cardinal direction North.
        /// </summary>
        public void SetHeading(double heading)
        {
            throw new NotImplementedException();
        }

        public void SetMapTypeId(MapTypeId mapTypeId)
        {
            throw new NotImplementedException();
        }

        public void SetOptions(MapOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Binds a StreetViewPanorama to the map. This panorama overrides the default StreetViewPanorama, allowing the map to bind to an external panorama outside of the map. Setting the panorama to null binds the default embedded panorama back to the map.
        /// </summary>
        public void SetStreetView(StreetViewPanorama panorama)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the angle of incidence for aerial imagery (available for SATELLITE and HYBRID map types) measured in degrees from the viewport plane to the map plane.  The only supported values are 0, indicating no angle of incidence (no tilt), and 45 indicating a tilt of 45deg;.
        /// </summary>
        public void SetTilt(double tilt)
        {
            throw new NotImplementedException();
        }

        public void SetZoom(double zoom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Additional controls to attach to the map. To add a control to the map, add the control's &lt;div&gt; to the MVCArray corresponding to the ControlPosition where it should be rendered.
        /// </summary>
        [IntrinsicProperty]
        public List<System.Html.Element> Controls
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// A registry of MapType instances by string ID.
        /// </summary>
        [IntrinsicProperty]
        public MapTypeRegistry MapTypes
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Additional map types to overlay.
        /// </summary>
        [IntrinsicProperty]
        public MVCArray OverlayMapTypes
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}