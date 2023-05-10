namespace Serenity;

/// <summary>
/// Null exception logger that logs nothing
/// </summary>
[Obsolete("Please log exceptions directly via .NET's ILogger interface")]
public class NullExceptionLogger : IExceptionLogger
{
    private NullExceptionLogger()
    {
    }

    /// <summary>
    /// NullExceptionLogger instance
    /// </summary>
    public static readonly NullExceptionLogger Instance = new();

    /// <summary>
    /// Does nothing
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="category">Optional category, can be null</param>
    public void Log(Exception exception, string? category)
    {
    }
}