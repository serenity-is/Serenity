using System.Collections.Generic;

namespace Serenity.Data
{
    public interface IEntityWithJoins : IEntity
    {
        /// <summary>
        /// List of all joins in entity</summary>
        IDictionary<string, Join> Joins { get; }
    }
}
