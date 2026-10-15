namespace Serene.Administration;

[DefaultConnection, AdministrationModule, TableName("RolePermissions")]
[DisplayText("Role Permissions"), InstanceName("Role Permission")]
[ReadPermission(PermissionKeys.Security)]
[ModifyPermission(PermissionKeys.Security)]
public sealed class RolePermissionRow : Row<RolePermissionRow.RowFields>, IIdRow, INameRow, IRolePermissionRow
{
    [DisplayText("Role Permission Id"), Identity, IdProperty]
    public long? RolePermissionId { get => fields.RolePermissionId[this]; set => fields.RolePermissionId[this] = value; }

    [DisplayText("Role Id"), NotNull, ForeignKey("Roles", "RoleId"), LeftJoin("jRole")]
    public int? RoleId { get => fields.RoleId[this]; set => fields.RoleId[this] = value; }

    [DisplayText("Permission Key"), Size(100), NotNull, QuickSearch, NameProperty]
    public string? PermissionKey { get => fields.PermissionKey[this]; set => fields.PermissionKey[this] = value; }

    [DisplayText("Role Role Name"), Expression("jRole.[RoleName]")]
    public string? RoleName { get => fields.RoleName[this]; set => fields.RoleName[this] = value; }

    public StringField RoleKeyOrNameField => fields.RoleName;
    public StringField PermissionKeyField => fields.PermissionKey;

    public class RowFields : RowFieldsBase
    {
        public Int64Field RolePermissionId = null!;
        public Int32Field RoleId = null!;
        public StringField PermissionKey = null!;

        public StringField RoleName = null!;
    }
}