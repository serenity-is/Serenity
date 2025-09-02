namespace Serenity.Abstractions;

/// <summary>
/// Exception logger abstraction
/// </summary>
[Obsolete("Please log exceptions directly via .NET's ILogger interface")]
public interface IExceptionLogger
{
    /// <summary>
    /// Logs the specified exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="category">Optional category, can be null</param>
    void Log(Exception exception, string? category);
}