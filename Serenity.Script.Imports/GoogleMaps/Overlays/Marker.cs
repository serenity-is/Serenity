using System.Runtime.CompilerServices;
using System;

namespace google.maps
{
    [Imported]
    public partial class Marker : MVCObject
    {
        /// <summary>
        /// The maximum default z-index that the API will assign to a marker. You may set a higher z-index to bring a marker to the front.
        /// </summary>
        public static double MAX_ZINDEX;

        /// <summary>
        /// Creates a marker with the options specified. If a map is specified, the marker is added to the map upon construction. Note that the position must be set for the marker to display.
        /// </summary>
        public Marker() { throw new NotImplementedException(); }
        /// <summary>
        /// Creates a marker with the options specified. If a map is specified, the marker is added to the map upon construction. Note that the position must be set for the marker to display.
        /// </summary>
        public Marker(MarkerOptions opts) { throw new NotImplementedException(); }
        public Animation GetAnimation() { throw new NotImplementedException(); }
        public bool GetClickable() { throw new NotImplementedException(); }
        public string GetCursor() { throw new NotImplementedException(); }
        public bool GetDraggable() { throw new NotImplementedException(); }
        public bool GetFlat() { throw new NotImplementedException(); }
        public string GetIcon() { throw new NotImplementedException(); }
        [ScriptName("getIcon")]
        public MarkerImage GetIconMarkerImage() { throw new NotImplementedException(); }
        public Map GetMap() { throw new NotImplementedException(); }
        [ScriptName("getMap")]
        public Map GetMapStreetViewPanorama() { throw new NotImplementedException(); }
        public LatLng GetPosition() { throw new NotImplementedException(); }
        public string GetShadow() { throw new NotImplementedException(); }
        [ScriptName("getShadow")]
        public string GetShadowMarkerImage() { throw new NotImplementedException(); }
        
        public MarkerShape GetShape() { throw new NotImplementedException(); }
        public string GetTitle() { throw new NotImplementedException(); }
        public bool GetVisible() { throw new NotImplementedException(); }
        public double GetZIndex() { throw new NotImplementedException(); }
        /// <summary>
        /// Start an animation. Any ongoing animation will be cancelled. Currently supported animations are: BOUNCE, DROP. Passing in null will cause any animation to stop.
        /// </summary>
        public void SetAnimation(Animation animation) { throw new NotImplementedException(); }
        public void SetClickable(bool flag) { throw new NotImplementedException(); }
        public void SetCursor(string cursor) { throw new NotImplementedException(); }
        public void SetDraggable(bool flag) { throw new NotImplementedException(); }
        public void SetFlat(bool flag) { throw new NotImplementedException(); }
        
        public void SetIcon(string icon) { throw new NotImplementedException(); }
        public void SetIcon(MarkerImage icon) { throw new NotImplementedException(); }
        /// <summary>
        /// Renders the marker on the specified map or panorama.  If map is set to null, the marker will be removed.
        /// </summary>
        public void SetMap(Map map) { throw new NotImplementedException(); }
        /// <summary>
        /// Renders the marker on the specified map or panorama.  If map is set to null, the marker will be removed.
        /// </summary>
        public void SetMap(StreetViewPanorama map) { throw new NotImplementedException(); }

        public void SetOptions(MarkerOptions options) { throw new NotImplementedException(); }
        public void SetPosition(LatLng latlng) { throw new NotImplementedException(); }
        public void SetShadow(string shadow) { throw new NotImplementedException(); }
        public void SetShadow(MarkerImage shadow) { throw new NotImplementedException(); }
        public void SetShape(MarkerShape shape) { throw new NotImplementedException(); }
        public void SetTitle(string title) { throw new NotImplementedException(); }
        public void SetVisible(bool visible) { throw new NotImplementedException(); }
        public void SetZIndex(double zIndex) { throw new NotImplementedException(); }
    }
}