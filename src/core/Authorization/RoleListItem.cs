namespace Serenity.Abstractions;

/// <summary>
/// A role in the list of <see cref="IRoleListService"/>.
/// </summary>
public class RoleListItem : IRoleDefinition
{
    /// <inheritdoc/>
    public string Id { get; set; } = "";

    /// <inheritdoc/>
    public string Name { get; set; } = "";

    /// <inheritdoc/>
    public string? RoleKey { get; set; }
}
