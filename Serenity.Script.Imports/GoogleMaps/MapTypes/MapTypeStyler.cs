using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// styler affects how a map's elements will be styled. Each MapTypeStyler should contain one and only one key. If more than one key is specified in a single MapTypeStyler, 
    /// all but one will be ignored. For example: var rule = {hue: '#ff0000'}.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class MapTypeStyler
    {
        /// <summary>
        /// Gamma. Modifies the gamma by raising the lightness to the given power. Valid values: Floating point numbers, [0.01, 10], with 1.0 representing no change.
        /// </summary>
        [IntrinsicProperty]
        public double Gamma
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Sets the hue of the feature to match the hue of the color supplied. Note that the saturation and lightness of the feature is conserved, which means that the feature will not match the color supplied exactly.  Valid values: An RGB hex string, i.e. '#ff0000'.
        /// </summary>
        [IntrinsicProperty]
        public string Hue
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Inverts lightness. A value of true will invert the lightness of the feature while preserving the hue and saturation.
        /// </summary>
        [IntrinsicProperty]
        public bool Invert_lightness
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Lightness. Shifts lightness of colors by a percentage of the original value if decreasing and a percentage of the remaining value if increasing. Valid values: [-100, 100].
        /// </summary>
        [IntrinsicProperty]
        public double Lightness
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Saturation. Shifts the saturation of colors by a percentage of the original value if decreasing and a percentage of the remaining value if increasing. Valid values: [-100, 100].
        /// </summary>
        [IntrinsicProperty]
        public double Saturation
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// Visibility: Valid values: 'on', 'off' or 'simplifed'.
        /// </summary>
        [IntrinsicProperty]
        public string Visibility
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}