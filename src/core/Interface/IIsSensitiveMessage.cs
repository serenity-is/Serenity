namespace Serenity.Services;

/// <summary>
/// An interface to determine if the message of an exception might contain sensitive
/// information that should not be shown to the end user. 
/// Currently only ValidationError implements this and returns false by default
/// unless set explicitly.
/// </summary>
public interface IIsSensitiveMessage
{
    /// <summary>
    /// Gets if the message of this exception can be safely shown to the end user, 
    /// e.g. 
    /// </summary>
    bool IsSensitiveMessage { get; }
}