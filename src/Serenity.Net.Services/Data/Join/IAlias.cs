
namespace Serenity.Data;

/// <summary>
///   Interface for table aliases.</summary>
public interface IAlias
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    string Name { get; }

    /// <summary>
    /// Gets the name plus dot.
    /// </summary>
    /// <value>
    /// The name dot.
    /// </value>
    string NameDot { get; }

    /// <summary>
    /// Gets the table.
    /// </summary>
    /// <value>
    /// The table.
    /// </value>
    string Table { get; }
}
