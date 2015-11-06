using Serenity.Data;
using System.Collections.Generic;
using System.Data;

namespace Serenity.Services
{
    /// <summary>
    /// Represents a RetrieveRequestHandler. Is used with RetrieveBehavior objects.
    /// </summary>
    public interface IRetrieveRequestHandler
    {
        /// <summary>
        /// Loader row
        /// </summary>
        Row Row { get; }

        /// <summary>
        /// Retrieve request
        /// </summary>
        RetrieveRequest Request { get; }

        /// <summary>
        /// Retrieve response
        /// </summary>
        IRetrieveResponse Response { get; }

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