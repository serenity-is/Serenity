using System.Collections;
using System.Collections.Generic;

namespace Serenity.Data
{
    public interface IFieldWithJoinInfo : IField
    {
        /// <summary>
        /// List of referenced joins in field expression</summary>
        HashSet<string> ReferencedJoins { get; }
        /// <summary>
        /// List of all joins in fields entity</summary>
        IDictionary<string, Join> Joins { get; }
    }
}
