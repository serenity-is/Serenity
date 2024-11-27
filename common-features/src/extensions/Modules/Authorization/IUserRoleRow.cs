namespace Serenity.Data;

/// <summary>
/// User role row interface
/// </summary>
public interface IUserRoleRow : IRow
{
    /// <summary>
    /// User ID field
    /// </summary>
    Field UserIdField { get; }

    /// <summary>
    /// User role key or the role name field
    /// </summary>
    StringField RoleKeyOrNameField { get; }
}
