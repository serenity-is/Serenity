using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// This object defines the properties that can be set on a FusionTablesLayer object.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class FusionTablesLayerOptions
    {
        /// <summary>
        /// If true, the layer receives mouse events. Default value is true.
        /// </summary>
        [IntrinsicProperty]
        public bool Clickable
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// By default, table data is displayed as geographic features. If true, the layer is used to display a heatmap representing the density of the geographic features returned by querying the selected table.
        /// </summary>
        [IntrinsicProperty]
        public bool Heatmap
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The map on which to display the layer.
        /// </summary>
        [IntrinsicProperty]
        public Map Map
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// A Fusion Tables query to apply when selecting the data to display. Queries should not be URL escaped.
        /// </summary>
        [IntrinsicProperty]
        public string Query
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Suppress the rendering of info windows when layer features are clicked.
        /// </summary>
        [IntrinsicProperty]
        public bool SuppressInfoWindows
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}