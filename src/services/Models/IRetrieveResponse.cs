using System.Collections;

namespace Serenity.Services;

/// <summary>
/// Interface for a <see cref="RetrieveResponse{T}"/>.
/// As the RetrieveResponse itself is generic, this allows easier
/// access to its members.
/// </summary>
public interface IRetrieveResponse
{
    /// <summary>
    /// The entity
    /// </summary>
    object Entity { get; }

    /// <summary>
    /// Dictionary containing localizations if requested.
    /// </summary>
    IDictionary Localizations { get; set; }
}