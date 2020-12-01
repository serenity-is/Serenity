using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    /// Connection string options
    /// </summary>
    public class ConnectionStringOptions : Dictionary<string, ConnectionStringEntry>, 
        IOptions<ConnectionStringOptions>
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public ConnectionStringOptions()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// Returns this
        /// </summary>
        public ConnectionStringOptions Value => this;
    }
}