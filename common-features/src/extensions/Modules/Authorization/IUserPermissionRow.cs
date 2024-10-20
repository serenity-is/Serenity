namespace Serenity.Data;

/// <summary>
/// User permission row interface
/// </summary>
public interface IUserPermissionRow : IRow
{
    /// <summary>
    /// User ID field
    /// </summary>
    Field UserIdField { get; }

    /// <summary>
    /// Permission key field
    /// </summary>
    StringField PermissionKeyField { get; }

    /// <summary>
    /// Granted field, might be null if not available. Used to optionally 
    /// revoke permissions granted via roles.
    /// </summary>
    BooleanField GrantedField { get; }
}