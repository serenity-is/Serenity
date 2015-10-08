using System;

namespace google.maps
{
    public partial class DirectionsService
    {
        /// <summary>
        /// Issue a directions search request.
        /// </summary>
        public void Route(DirectionsRequest request, DirectionsServiceCallback callback)
        {
            throw new NotImplementedException();
        }

        public delegate void DirectionsServiceCallback(DirectionsResult dr, DirectionsStatus status);
    }
}