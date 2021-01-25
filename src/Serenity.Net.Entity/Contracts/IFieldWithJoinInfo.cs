using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    /// IFieldWithJoinInfo
    /// </summary>
    /// <seealso cref="Serenity.Data.IField" />
    public interface IFieldWithJoinInfo : IField
    {
        /// <summary>
        /// List of referenced joins in field expression</summary>
        HashSet<string> ReferencedAliases { get; }
        /// <summary>
        /// List of all joins in fields entity</summary>
        IDictionary<string, Join> Joins { get; }
    }
}
