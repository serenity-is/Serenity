
using System.Collections;

namespace Serenity.Services;

/// <summary>
/// Interface for a <see cref="SaveRequest{TEntity}"/>. 
/// As the SaveRequest itself is generic, this allows
/// easier access to its members.
/// </summary>
public interface ISaveRequest
{
    /// <summary>
    /// The entity ID to update, should only be
    /// passed for Update requests.
    /// </summary>
    object EntityId { get; set; }

    /// <summary>
    /// Entity to insert / update
    /// </summary>
    object Entity { get; set; }

    /// <summary>
    /// Dictionary of translations if required.
    /// </summary>
    IDictionary Localizations { get; set; }
}
