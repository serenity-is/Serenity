using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public partial class DistanceMatrixService
    {
        /// <summary>
        /// Creates a new instance of a DistanceMatrixService that sends distance matrix queries to Google servers.
        /// </summary>
        public DistanceMatrixService()
        {
            throw new NotImplementedException();
        }
        public void GetDistanceMatrix(DistanceMatrixRequest request,DistanceMatrixServiceCallback callback)
        {
            throw new NotImplementedException();
        }

       
    }
    public delegate void DistanceMatrixServiceCallback(DistanceMatrixResponse dr, DistanceMatrixStatus status);
}
