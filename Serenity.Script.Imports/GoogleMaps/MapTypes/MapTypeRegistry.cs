using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public partial class MapTypeRegistry : MVCObject
    {
        /// <summary>
        /// The MapTypeRegistry holds the collection of custom map types available to the map for its use.  The API consults this registry when providing the list of avaiable map types within controls, for example.
        /// </summary>
        public MapTypeRegistry()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the registry to associate the passed string identifier with the passed MapType.
        /// </summary>
        public void Set(string id, MapType mapType)
        {
            throw new NotImplementedException();
        }
    }
}