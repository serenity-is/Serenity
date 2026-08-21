namespace Serenity;

/// <summary>
/// Represents the core identity information for a user.
/// </summary>
/// <remarks>
/// Applications typically implement this interface with an application-specific
/// class (for example, <c>UserDefinition</c>) that adds additional properties.
/// </remarks>
public interface IUserDefinition
{
    /// <summary>
    /// Gets the unique user identifier.
    /// </summary>
    string Id { get; }
    /// <summary>
    /// Gets the login name of the user.
    /// </summary>
    string Username { get; }
    /// <summary>
    /// Gets the display name of the user, which may be the same as <see cref="Username"/>.
    /// </summary>
    string DisplayName { get; }
    /// <summary>
    /// Gets the email address of the user.
    /// </summary>
    string Email { get; }
    /// <summary>
    /// Gets a value indicating whether the user is active (1 = active, 0 = disabled, -1 = deleted).
    /// </summary>
    short IsActive { get; }
}