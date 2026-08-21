
namespace Serenity.Data;

/// <summary>
/// An interface for entities with a Table property.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Gets the table name.
    /// </summary>
    string Table { get; }
}
