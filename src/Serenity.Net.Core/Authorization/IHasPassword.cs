namespace Serenity.Abstractions;

/// <summary>
/// Used to determine if user has a password.
/// If interface not found on user definition it assumes user has a password.
/// </summary>
public interface IHasPassword
{
    /// <summary>
    /// Is user has a password
    /// </summary>
    public bool HasPassword { get; }
}