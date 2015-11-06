using Serenity.Data;
using System.Collections.Generic;
using System.Data;

namespace Serenity.Services
{
    /// <summary>
    /// Represents a SaveRequestHandler. Is used with SaveBehavior objects.
    /// </summary>
    public interface IDeleteRequestHandler
    {
        /// <summary>
        /// New row
        /// </summary>
        Row Row { get; }

        /// <summary>
        /// Save request
        /// </summary>
        DeleteRequest Request { get; }

        /// <summary>
        /// Save response
        /// </summary>
        DeleteResponse Response { get; }

        /// <summary>
        /// A state bag that can be used as storage within a request handler context
        /// </summary>
        IDictionary<string, object> StateBag { get; }

        /// <summary>
        /// Current connection
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// Current transaction
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}