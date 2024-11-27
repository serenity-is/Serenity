
namespace Serenity.Data;

/// <summary>
/// An interface for entities with Table property
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Table name</summary>
    string Table { get; }
}
