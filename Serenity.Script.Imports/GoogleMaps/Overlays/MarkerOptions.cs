using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class MarkerOptions
    {
        /// <summary>
        /// If true, the marker receives mouse and touch events.  Default value is true.
        /// </summary>
        [IntrinsicProperty]
        public bool Clickable
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Mouse cursor to show on hover
        /// </summary>
        [IntrinsicProperty]
        public string Cursor
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If true, the marker can be dragged.  Default value is false.
        /// </summary>
        [IntrinsicProperty]
        public bool Draggable
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If true, the marker shadow will not be displayed.
        /// </summary>
        [IntrinsicProperty]
        public bool Flat
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Icon for the foreground
        /// </summary>
        [IntrinsicProperty]
        public string Icon
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Icon for the foreground
        /// </summary>
        [IntrinsicProperty]
        [ScriptName("icon")]
        public MarkerImage IconMarkerImage
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Map on which to display Marker.
        /// </summary>
        [IntrinsicProperty]
        public Map Map
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Map on which to display Marker.
        /// </summary>
        [IntrinsicProperty]
        [ScriptName("map")]
        public StreetViewPanorama MapStreetViewPanorama
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If false, rendering will not be optimized for this marker.
        /// </summary>
        [IntrinsicProperty]
        public bool Optimized
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Marker position. Required.
        /// </summary>
        [IntrinsicProperty]
        public LatLng Position
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If false, disables raising and lowering the marker on drag.  This option is true by default.
        /// </summary>
        [IntrinsicProperty]
        public bool RaiseOnDrag
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Shadow image
        /// </summary>
        [IntrinsicProperty]
        public string Shadow
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Shadow image
        /// </summary>
        [IntrinsicProperty]
        [ScriptName("shadow")]
        public MarkerImage ShadowMarkerImage
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Image map region definition used for drag/click.
        /// </summary>
        [IntrinsicProperty]
        public MarkerShape Shape
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Rollover text
        /// </summary>
        [IntrinsicProperty]
        public string Title
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If true, the marker is visible
        /// </summary>
        [IntrinsicProperty]
        public bool Visible
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// All markers are displayed on the map in order of their zIndex, with higher values displaying in front of markers with lower values.  By default, markers are displayed according to their vertical position on screen, with lower markers appearing in front of markers further up the screen.
        /// </summary>
        [IntrinsicProperty]
        public double ZIndex
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        [IntrinsicProperty]
        public Animation Animation { get; set; }
    }
}