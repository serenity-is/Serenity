using Microsoft.Extensions.Logging;

namespace Serenity.Data;

/// <summary>
/// Interfaces for types that has an Logger 
/// </summary>
public interface IHasLogger
{
    /// <summary>
    /// Gets the logger (that can be used by SqlHelper methods for logging)
    /// </summary>
    ILogger Logger { get; }
}