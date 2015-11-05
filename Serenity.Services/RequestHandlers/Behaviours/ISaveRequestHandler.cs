using Serenity.Data;
using System.Collections.Generic;

namespace Serenity.Services
{
    /// <summary>
    /// Represents a SaveRequestHandler. Is used with SaveBehavior objects.
    /// </summary>
    public interface ISaveRequestHandler
    {
        /// <summary>
        /// Old row, if any, otherwise null
        /// </summary>
        Row Old { get; }

        /// <summary>
        /// New row
        /// </summary>
        Row Row { get; }

        /// <summary>
        /// Save request
        /// </summary>
        ISaveRequest Request { get; }

        /// <summary>
        /// Save response
        /// </summary>
        SaveResponse Response { get; }

        /// <summary>
        /// A state bag that can be used as storage within a request handler context
        /// </summary>
        IDictionary<string, object> StateBag { get; }

        /// <summary>
        /// Current transaction
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}