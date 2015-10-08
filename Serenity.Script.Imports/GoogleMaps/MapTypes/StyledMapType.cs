using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public partial class StyledMapType : MVCObject
    {
        /// <summary>
        /// Creates a styled MapType with the specified options. The StyledMapType takes an array of MapTypeStyles, where each MapTypeStyle is applied to the map consecutively. A later MapTypeStyle that applies the same MapTypeStylers to the same selectors as an earlier MapTypeStyle will override the earlier MapTypeStyle.
        /// </summary>
        public StyledMapType(List<MapTypeStyle> style)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a styled MapType with the specified options. The StyledMapType takes an array of MapTypeStyles, where each MapTypeStyle is applied to the map consecutively. A later MapTypeStyle that applies the same MapTypeStylers to the same selectors as an earlier MapTypeStyle will override the earlier MapTypeStyle.
        /// </summary>
        public StyledMapType(List<MapTypeStyle> style, StyledMapTypeOptions options)
        {
            throw new NotImplementedException();
        }
    }
}