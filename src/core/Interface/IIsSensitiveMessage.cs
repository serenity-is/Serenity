namespace Serenity.Services;

/// <summary>
/// An interface to determine if the message of an exception might contain sensitive
/// information that should not be shown to the end user.
/// Currently only <see cref="ValidationError"/> implements this and returns false by default
/// unless set explicitly.
/// </summary>
public interface IIsSensitiveMessage
{
    /// <summary>
    /// Gets a value indicating whether the message of this exception can be safely shown
    /// to the end user, e.g. whether it does not contain sensitive information.
    /// </summary>
    bool IsSensitiveMessage { get; }
}