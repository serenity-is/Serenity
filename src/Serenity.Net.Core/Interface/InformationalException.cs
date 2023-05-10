namespace Serenity;

/// <summary>
/// An exception that is created purely for informational purposes,
/// e.g. for logging something to exception log
/// </summary>
public class InformationalException : Exception
{
    /// <summary>
    /// EventId for informational errors
    /// </summary>
    public const int EventId = 77701;

    /// <summary>
    /// Creates a new instance of the exception
    /// </summary>
    /// <param name="message">Message</param>
    public InformationalException(string message) 
        : base(message)
    {
    }
}
