
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
    object? EntityId { get; set; }

    /// <summary>
    /// Entity to insert / update
    /// </summary>
    object? Entity { get; set; }

    /// <summary>
    /// Dictionary of translations if required.
    /// </summary>
    IDictionary? Localizations { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the concurrency version check should be skipped
    /// for this save request. This is a server side only option and is ignored in client JSON,
    /// intended for internal services that need to update a row regardless of its version.
    /// </summary>
    bool IgnoreConcurrencyVersion { get; set; }
}
