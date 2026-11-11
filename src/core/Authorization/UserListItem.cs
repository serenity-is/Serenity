namespace Serenity.Abstractions;

/// <summary>
/// A user in the list of <see cref="IUserListService"/>, which has only the properties of
/// <see cref="IUserDefinition"/>, so nothing like the password of a user is loaded to list them.
/// </summary>
public class UserListItem : IUserDefinition
{
    /// <inheritdoc/>
    public string Id { get; set; } = "";

    /// <inheritdoc/>
    public string Username { get; set; } = "";

    /// <inheritdoc/>
    public string DisplayName { get; set; } = "";

    /// <inheritdoc/>
    public string? Email { get; set; }

    /// <inheritdoc/>
    public short IsActive { get; set; } = 1;
}
