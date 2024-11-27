namespace Serenity.Data;

/// <summary>
/// Interface for a field with join and referenced join alias information
/// </summary>
/// <seealso cref="IField" />
public interface IFieldWithJoinInfo : IField
{
    /// <summary>
    /// List of referenced joins in field expression</summary>
    HashSet<string> ReferencedAliases { get; }

    /// <summary>
    /// List of all joins in fields entity</summary>
    IDictionary<string, Join> Joins { get; }
}
