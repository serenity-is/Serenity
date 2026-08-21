namespace Serenity.Abstractions;

/// <summary>
/// Indicates whether a user account has a password set.
/// </summary>
/// <remarks>
/// When a user definition does not implement this interface, the system
/// assumes that the user has a password.
/// </remarks>
public interface IHasPassword
{
    /// <summary>
    /// Gets a value indicating whether the user has a password.
    /// </summary>
    public bool HasPassword { get; }
}