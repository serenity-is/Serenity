using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// Options defining the properties of a StreetViewPanorama object. 
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class StreetViewPanoramaOptions
    {
        /// <summary>
        /// The enabled/disabled state of the address control.
        /// </summary>
        [IntrinsicProperty]
        public bool AddressControl
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The display options for the address control.
        /// </summary>
        [IntrinsicProperty]
        public StreetViewAddressControlOptions AddressControlOptions
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Enables/disables zoom on double click. Enabled by default.
        /// </summary>
        [IntrinsicProperty]
        public bool DisableDoubleClickZoom
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If true, the close button is displayed. Disabled by default.
        /// </summary>
        [IntrinsicProperty]
        public bool EnableCloseButton
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The enabled/disabled state of the links control.
        /// </summary>
        [IntrinsicProperty]
        public bool LinksControl
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The enabled/disabled state of the pan control.
        /// </summary>
        [IntrinsicProperty]
        public bool PanControl
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The display options for the pan control.
        /// </summary>
        [IntrinsicProperty]
        public PanControlOptions PanControlOptions
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The panorama ID, which should be set when specifying a custom panorama.
        /// </summary>
        [IntrinsicProperty]
        public string Pano
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The LatLng position of the Street View panorama.
        /// </summary>
        [IntrinsicProperty]
        public LatLng Position
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The camera orientation, specified as heading, pitch, and zoom, for the panorama.
        /// </summary>
        [IntrinsicProperty]
        public StreetViewPov Pov
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If false, disables scrollwheel zooming in Street View.  The scrollwheel is enabled by default.
        /// </summary>
        [IntrinsicProperty]
        public bool Scrollwheel
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// If true, the Street View panorama is visible on load.
        /// </summary>
        [IntrinsicProperty]
        public bool Visible
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The enabled/disabled state of the zoom control.
        /// </summary>
        [IntrinsicProperty]
        public bool ZoomControl
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The display options for the zoom control.
        /// </summary>
        [IntrinsicProperty]
        public ZoomControlOptions ZoomControlOptions
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}