using Serenity.Data;
using System.Collections.Generic;
using System.Data;

namespace Serenity.Services
{
    /// <summary>
    /// Represents a ListRequestHandler. Is used with ListBehavior objects.
    /// </summary>
    public interface IListRequestHandler
    {
        /// <summary>
        /// Loader row
        /// </summary>
        Row Row { get; }

        /// <summary>
        /// List request
        /// </summary>
        ListRequest Request { get; }

        /// <summary>
        /// List response
        /// </summary>
        IListResponse Response { get; }

        /// <summary>
        /// A state bag that can be used as storage within a request handler context
        /// </summary>
        IDictionary<string, object> StateBag { get; }

        /// <summary>
        /// Current connection
        /// </summary>
        IDbConnection Connection { get; }
    }
}