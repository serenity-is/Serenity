using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps.panoramio
{
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class PanoramioFeature
    {
        /// <summary>
        /// The username of the user who uploaded this photo.
        /// </summary>
        [IntrinsicProperty]
        public string Author
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The unique identifier for this photo, as used in the Panoramio API (see http://www.panoramio.com/api/widget/api.html).
        /// </summary>
        [IntrinsicProperty]
        public string PhotoId
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The title of the photo.
        /// </summary>
        [IntrinsicProperty]
        public string Title
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The URL of the photo.
        /// </summary>
        [IntrinsicProperty]
        public string Url
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The unique identifier for the user who uploaded this photo, as used in the Panoramio API (see http://www.panoramio.com/api/widget/api.html).
        /// </summary>
        [IntrinsicProperty]
        public string UserId
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}