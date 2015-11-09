using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// Each MapTypeStyleElementType distinguishes between the different representations of a feature.
    /// </summary>
    [Imported]
    public enum MapTypeStyleElementType
    {
        /// <summary>
        /// Apply the rule to all elements of the specified feature.
        /// </summary>
        all,
        /// <summary>
        /// Apply the rule to the feature's geometry.
        /// </summary>
        geometry,
        /// <summary>
        /// Apply the rule to the feature's labels.
        /// </summary>
        labels,
    }
}