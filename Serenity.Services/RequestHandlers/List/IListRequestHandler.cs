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

        /// <summary>
        /// Returns true if field is allowed to be selected, based on permissions and SelectLevel.Never.
        /// </summary>
        /// <param name="field">Field</param>
        /// <returns>True if field should be selected</returns>
        bool AllowSelectField(Field field);

        /// <summary>
        /// Returns true if field should be selected based on ColumnSelection flags.
        /// </summary>
        /// <param name="field">Field</param>
        /// <returns>True if field should be selected</returns>
        bool ShouldSelectField(Field field);
    }
}