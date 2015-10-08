using System;
using System.Collections.Generic;

namespace google.maps
{
    public partial class MVCArray
    {
        /// <summary>
        /// Iterate over each element, calling the provided callback. The callback is called for each element like: callback(element, index).
        /// </summary>
        public void ForEach(Callback callback)
        {
            throw new NotImplementedException();
        }

        public delegate void Callback(object element, double number);
    }
}