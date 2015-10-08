using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class Projection
    {
        /// <summary>
        /// Translates from the LatLng cylinder to the Point plane. This interface specifies a function which implements translation from given LatLng values to world coordinates on the map projection. The Maps API calls this method when it needs to plot locations on screen. Projection objects must implement this method.
        /// </summary>
        public Point FromLatLngToPoint(LatLng latLng)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translates from the LatLng cylinder to the Point plane. This interface specifies a function which implements translation from given LatLng values to world coordinates on the map projection. The Maps API calls this method when it needs to plot locations on screen. Projection objects must implement this method.
        /// </summary>
        public Point FromLatLngToPoint(LatLng latLng, Point point)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This interface specifies a function which implements translation from world coordinates on a map projection to LatLng values. The Maps API calls this method when it needs to translate actions on screen to positions on the map. Projection objects must implement this method.
        /// </summary>
        public LatLng FromPointToLatLng(Point pixel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This interface specifies a function which implements translation from world coordinates on a map projection to LatLng values. The Maps API calls this method when it needs to translate actions on screen to positions on the map. Projection objects must implement this method.
        /// </summary>
        public LatLng FromPointToLatLng(Point pixel, bool nowrap)
        {
            throw new NotImplementedException();
        }
    }
}