using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    /// <summary>
    /// Contains details of the author of a KML document or feature.
    /// </summary>
    [ScriptName("Object")]
    [IgnoreNamespace]
    [Imported]
    public sealed partial class KmlAuthor
    {
        /// <summary>
        /// The author's e-mail address, or an empty string if not specified.
        /// </summary>
        [IntrinsicProperty]
        public string Email
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The author's name, or an empty string if not specified.
        /// </summary>
        [IntrinsicProperty]
        public string Name
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        /// The author's home page, or an empty string if not specified.
        /// </summary>
        [IntrinsicProperty]
        public string Uri
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}