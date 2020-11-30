using Microsoft.Extensions.Options;
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
        /// Returns this
        /// </summary>
        public ConnectionStringOptions Value => this;
    }
}