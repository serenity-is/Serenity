namespace Serenity.Data;

/// <summary>
/// Role permission row interface
/// </summary>
public interface IRolePermissionRow : IRow
{
    /// <summary>
    /// Role key or role name field
    /// </summary>
    StringField RoleKeyOrNameField { get; }

    /// <summary>
    /// Permission key field
    /// </summary>
    StringField PermissionKeyField { get; }
}