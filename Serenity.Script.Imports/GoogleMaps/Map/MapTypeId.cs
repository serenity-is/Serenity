using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported, NamedValues]
    public enum MapTypeId
    {
        /// <summary>
        /// This map type displays a transparent layer of major streets on satellite images.
        /// </summary>
        hybrid,
        /// <summary>
        /// This map type displays a normal street map.
        /// </summary>
        roadmap,
        /// <summary>
        /// This map type displays satellite images.
        /// </summary>
        satellite,
        /// <summary>
        /// This map type displays maps with physical features such as terrain and vegetation.
        /// </summary>
        terrain,
    }
}