using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// A layer showing bike lanes and paths. This class extends MVCObject.
    /// </summary>
    [Imported]
    public partial class BicyclingLayer : MVCObject
    {
        /// <summary>
        /// A layer that displays bike lanes and paths and demotes large roads.
        /// </summary>
        public BicyclingLayer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the map on which this layer is displayed.
        /// </summary>
        public Map GetMap()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Renders the layer on the specified map.  If map is set to null, the layer will be removed.
        /// </summary>
        public void SetMap(Map map)
        {
            throw new NotImplementedException();
        }
    }
}