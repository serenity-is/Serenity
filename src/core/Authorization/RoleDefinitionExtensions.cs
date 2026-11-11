namespace Serenity.Abstractions;

/// <summary>
/// Extensions for <see cref="IRoleDefinition"/>.
/// </summary>
public static class RoleDefinitionExtensions
{
    /// <summary>
    /// Gets what identifies the role in the roles of a user and in the permissions of a role, which is its key, or
    /// its name when it has no key. The permission key that the users who have the role hold is <c>Role:</c> followed
    /// by it.
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns>The key of the role, or its name.</returns>
    public static string GetKeyOrName(this IRoleDefinition role)
    {
        if (role is null)
            throw new ArgumentNullException(nameof(role));

        return string.IsNullOrWhiteSpace(role.RoleKey) ? role.Name : role.RoleKey!;
    }
}
