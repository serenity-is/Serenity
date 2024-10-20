namespace Serene.Administration;

[ConnectionKey("Default"), Module("Administration"), TableName("RolePermissions")]
[DisplayName("Role Permissions"), InstanceName("Role Permission")]
[ReadPermission(PermissionKeys.Security)]
[ModifyPermission(PermissionKeys.Security)]
public sealed class RolePermissionRow : Row<RolePermissionRow.RowFields>, IIdRow, INameRow, IRolePermissionRow
{
    [DisplayName("Role Permission Id"), Identity, IdProperty]
    public long? RolePermissionId { get => fields.RolePermissionId[this]; set => fields.RolePermissionId[this] = value; }

    [DisplayName("Role Id"), NotNull, ForeignKey("Roles", "RoleId"), LeftJoin("jRole")]
    public int? RoleId { get => fields.RoleId[this]; set => fields.RoleId[this] = value; }

    [DisplayName("Permission Key"), Size(100), NotNull, QuickSearch, NameProperty]
    public string PermissionKey { get => fields.PermissionKey[this]; set => fields.PermissionKey[this] = value; }

    [DisplayName("Role Role Name"), Expression("jRole.[RoleName]")]
    public string RoleName { get => fields.RoleName[this]; set => fields.RoleName[this] = value; }

    public StringField RoleKeyOrNameField => fields.RoleName;
    public StringField PermissionKeyField => fields.PermissionKey;

    public class RowFields : RowFieldsBase
    {
        public Int64Field RolePermissionId;
        public Int32Field RoleId;
        public StringField PermissionKey;

        public StringField RoleName;
    }
}