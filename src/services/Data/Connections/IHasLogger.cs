using Microsoft.Extensions.Logging;

namespace Serenity.Data;

/// <summary>
/// Interface for types that have a <see cref="Logger"/> property.
/// </summary>
public interface IHasLogger
{
    /// <summary>
    /// Gets the logger (that can be used by SqlHelper methods for logging).
    /// </summary>
    ILogger Logger { get; }
}