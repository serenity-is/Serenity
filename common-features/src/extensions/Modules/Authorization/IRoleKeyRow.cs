namespace Serenity.Extensions;

/// <summary>
/// Implemented by role rows that have a key besides their name, which does not change when the role is renamed.
/// <see cref="BaseRoleListService{TRow}"/> lists it as the key of the role.
/// </summary>
public interface IRoleKeyRow
{
    /// <summary>
    /// Gets the role key field.
    /// </summary>
    StringField RoleKeyField { get; }
}
