namespace Serenity;

/// <summary>
/// An exception that should not be logged.
/// </summary>
public interface INotLoggedException
{
    /// <summary>
    /// Gets a value indicating whether to not log exception.
    /// </summary>
    /// <value>
    ///   <c>true</c> if not logged exception; otherwise, <c>false</c>.
    /// </value>
    bool NotLoggedException { get; }
}
