using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// A FusionTablesLayer allows you to display data from a Google Fusion Table on a map, as a rendered layer. (See http://code.google.com/apis/fusiontables for more information about Fusion Tables). 
    /// Table data can be queried using the same query language as is used in the Fusion Tables API. This class extends MVCObject.
    /// </summary>
    [Imported]
    public partial class FusionTablesLayer : MVCObject
    {
        /// <summary>
        /// A layer that displays data from a Fusion Table.
        /// </summary>
        public FusionTablesLayer(string tableId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A layer that displays data from a Fusion Table.
        /// </summary>
        public FusionTablesLayer(string tableId, FusionTablesLayerOptions opts)
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

        public string GetQuery()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the ID of the table from which to query data.
        /// </summary>
        public double GetTableId()
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

        public void SetOptions(FusionTablesLayerOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the query to execute on the table specified by the tableId property. The layer will be updated to display the results of the query.
        /// </summary>
        public void SetQuery(string query)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the ID of the table from which to query data. Setting this value will cause the layer to be redrawn.
        /// </summary>
        public void SetTableId(double tableId)
        {
            throw new NotImplementedException();
        }
    }
}