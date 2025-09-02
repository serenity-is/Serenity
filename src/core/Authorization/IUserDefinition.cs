namespace Serenity;

/// <summary>
/// User definition abstraction.
/// </summary>
/// <remarks>Your application might have a class that implements this interface, e.g. UserDefinition, that has these properties plus some more
/// specific to your app.</remarks>
public interface IUserDefinition
{
    /// <summary>
    /// User ID
    /// </summary>
    string Id { get; }
    /// <summary>
    /// User login name
    /// </summary>
    string Username { get; }
    /// <summary>
    /// Display name for user (can be same with Username)
    /// </summary>
    string DisplayName { get; }
    /// <summary>
    /// Email address
    /// </summary>
    string Email { get; }
    /// <summary>
    /// Is user active (1 = active, 0 = disabled, -1 = deleted)
    /// </summary>
    short IsActive { get; }
}