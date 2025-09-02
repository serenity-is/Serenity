namespace Serenity;

/// <summary>
/// Exception data extensions
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    ///   Sets custom exception data with given property name and value. Sets the data in base exception.</summary>
    /// <param name="exception">
    ///   Exception to set custom data in.</param>
    /// <param name="property">
    ///   Custom exception data name.</param>
    /// <param name="value">
    ///   Custom exception data value.</param>
    public static void SetData(this Exception exception, string property, object? value)
    {
        exception.GetBaseException().Data[property] = value;
    }

    /// <summary>
    /// Logs the exception if logger instance is not null
    /// </summary>
    /// <param name="exception">Exception</param>
    /// <param name="logger">Logger</param>
    /// <param name="category">Optional category</param>
    [Obsolete("Please log exceptions directly via .NET's ILogger interface")]
    public static void Log(this Exception exception, IExceptionLogger logger, string? category = null)
    {
        try
        {
            logger?.Log(exception, category);
        }
        catch
        {
            // ignore errors while logging
        }
    }
}