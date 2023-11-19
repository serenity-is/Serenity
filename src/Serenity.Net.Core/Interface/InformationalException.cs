namespace Serenity;

/// <summary>
/// An exception that is created purely for informational purposes,
/// e.g. for logging something to exception log
/// </summary>
/// <remarks>
/// Creates a new instance of the exception
/// </remarks>
/// <param name="message">Message</param>
public class InformationalException(string message) : Exception(message)
{
    /// <summary>
    /// EventId for informational errors
    /// </summary>
    public const int EventId = 77701;
}
