namespace Serenity.Data;

/// <summary>
/// Field object abstraction for SQL query
/// </summary>
public interface IField
{
    /// <summary>
    /// Column name</summary>
    string Name { get; }

    /// <summary>
    /// The expression (can be equal to name if no expression)</summary>
    string Expression { get; }

    /// <summary>
    /// Select as column alias. Can be equal to property name or name.</summary>
    string ColumnAlias { get; }
}
