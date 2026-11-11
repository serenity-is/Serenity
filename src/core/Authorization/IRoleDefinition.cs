namespace Serenity.Abstractions;

/// <summary>
/// A role of the application, as <see cref="IRoleListService"/> lists them.
/// </summary>
public interface IRoleDefinition
{
    /// <summary>
    /// Gets the unique role identifier.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the name of the role, which is what to show to the users.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the key of the role, which does not change when the role is renamed, or null if the role has none, so
    /// it is known by its name. Use <see cref="RoleDefinitionExtensions.GetKeyOrName"/> for what the roles of
    /// a user and the permissions of a role are stored with.
    /// </summary>
    string? RoleKey { get; }
}
