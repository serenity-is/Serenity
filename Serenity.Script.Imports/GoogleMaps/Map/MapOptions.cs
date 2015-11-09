using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public partial class MapOptions
    {
        /// <summary>
        /// Color used for the background of the Map div.  This color will be visible when tiles have not yet loaded as the user pans.  This option can only be set when the map is initialized.
        /// </summary>
        [IntrinsicProperty]
        public string BackgroundColor
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The initial Map center. Required.
        /// </summary>
        [IntrinsicProperty]
        public LatLng Center
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Enables/disables all default UI. May be overridden individually.
        /// </summary>
        [IntrinsicProperty]
        public bool DisableDefaultUI
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Enables/disables zoom and center on double click. Enabled by default.
        /// </summary>
        [IntrinsicProperty]
        public bool DisableDoubleClickZoom
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If false, prevents the map from being dragged.  Dragging is enabled by default.
        /// </summary>
        [IntrinsicProperty]
        public bool Draggable
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The name or url of the cursor to display on a draggable object.
        /// </summary>
        [IntrinsicProperty]
        public string DraggableCursor
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The name or url of the cursor to display when an object is dragging.
        /// </summary>
        [IntrinsicProperty]
        public string DraggingCursor
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The heading for aerial imagery in degrees measured clockwise from cardinal direction North. Headings are snapped to the nearest available angle for which imagery is available.
        /// </summary>
        [IntrinsicProperty]
        public double Heading
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If false, prevents the map from being controlled by the keyboard. Keyboard shortcuts are enabled by default.
        /// </summary>
        [IntrinsicProperty]
        public bool KeyboardShortcuts
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The initial enabled/disabled state of the Map type control.
        /// </summary>
        [IntrinsicProperty]
        public bool MapTypeControl
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The initial display options for the Map type control.
        /// </summary>
        [IntrinsicProperty]
        public MapTypeControlOptions MapTypeControlOptions
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The initial Map mapTypeId. Required.
        /// </summary>
        [IntrinsicProperty]
        public MapTypeId MapTypeId
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The maximum zoom level which will be displayed on the map. If omitted, or set to null, the maximum zoom from the current map type is used instead.
        /// </summary>
        [IntrinsicProperty]
        public double MaxZoom
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The minimum zoom level which will be displayed on the map. If omitted, or set to null, the minimum zoom from the current map type is used instead.
        /// </summary>
        [IntrinsicProperty]
        public double MinZoom
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If true, do not clear the contents of the Map div.
        /// </summary>
        [IntrinsicProperty]
        public bool NoClear
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The enabled/disabled state of the Overview Map control.
        /// </summary>
        [IntrinsicProperty]
        public bool OverviewMapControl
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The display options for the Overview Map control.
        /// </summary>
        [IntrinsicProperty]
        public OverviewMapControlOptions OverviewMapControlOptions
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The enabled/disabled state of the Pan control.
        /// </summary>
        [IntrinsicProperty]
        public bool PanControl
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The display options for the Pan control.
        /// </summary>
        [IntrinsicProperty]
        public PanControlOptions PanControlOptions
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The enabled/disabled state of the Rotate control.
        /// </summary>
        [IntrinsicProperty]
        public bool RotateControl
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The display options for the Rotate control.
        /// </summary>
        [IntrinsicProperty]
        public RotateControlOptions RotateControlOptions
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The initial enabled/disabled state of the Scale control.
        /// </summary>
        [IntrinsicProperty]
        public bool ScaleControl
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The initial display options for the Scale control.
        /// </summary>
        [IntrinsicProperty]
        public ScaleControlOptions ScaleControlOptions
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If false, disables scrollwheel zooming on the map.  The scrollwheel is enabled by default.
        /// </summary>
        [IntrinsicProperty]
        public bool Scrollwheel
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// A StreetViewPanorama to display when the Street View pegman is dropped on the map.  If no panorama is specified, a default StreetViewPanorama will be displayed in the map's div when the pegman is dropped.
        /// </summary>
        [IntrinsicProperty]
        public StreetViewPanorama StreetView
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The initial enabled/disabled state of the Street View Pegman control.
        /// </summary>
        [IntrinsicProperty]
        public bool StreetViewControl
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The initial display options for the Street View Pegman control.
        /// </summary>
        [IntrinsicProperty]
        public StreetViewControlOptions StreetViewControlOptions
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The angle of incidence of the map as measured in degrees from the viewport plane to the map plane.  The only currently supported values are 0, indicating no angle of incidence (no tilt), and 45, indicating a tilt of 45deg;.  45deg; imagery is only available for SATELLITE and HYBRID map types, within some locations, and at some zoom levels.
        /// </summary>
        [IntrinsicProperty]
        public double Tilt
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The initial Map zoom level. Required.
        /// </summary>
        [IntrinsicProperty]
        public double Zoom
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The enabled/disabled state of the Zoom control.
        /// </summary>
        [IntrinsicProperty]
        public bool ZoomControl
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The display options for the Zoom control.
        /// </summary>
        [IntrinsicProperty]
        public ZoomControlOptions ZoomControlOptions
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}