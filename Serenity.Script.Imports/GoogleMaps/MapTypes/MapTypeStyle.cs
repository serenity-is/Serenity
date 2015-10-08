using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class MapTypeStyle
    {
        /// <summary>
        /// Selects the element type to which a styler should be applied.  An element type distinguishes between the different representations of a feature.  Optional; if elementType is not specified, the value is assumed to be 'all'.
        /// </summary>
        [IntrinsicProperty]
        public MapTypeStyleElementType ElementType
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Selects the feature, or group of features, to which a styler should be applied.  Optional; if featureType is not specified, the value is assumed to be 'all'.
        /// </summary>
        [IntrinsicProperty]
        public MapTypeStyleFeatureType FeatureType
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The style rules to apply to the selectors.  The rules are applied to the map's elements in the order they are listed in this array.
        /// </summary>
        [IntrinsicProperty]
        public List<MapTypeStyler> Stylers
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}