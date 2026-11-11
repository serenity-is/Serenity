namespace Serenity.Abstractions;

/// <summary>
/// Options for listing roles with <see cref="IRoleListService"/>.
/// </summary>
public class RoleListOptions
{
    /// <summary>
    /// Gets or sets a text that the name or the key of the roles should contain, which is compared the way the
    /// database compares text, so it is case insensitive with the usual collations. All roles match when it is blank.
    /// </summary>
    public string? ContainsText { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of roles to list, ordered by name. All roles are listed when it is zero or less.
    /// </summary>
    public int Take { get; set; }
}
