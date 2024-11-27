namespace Serene.Administration;

[ConnectionKey("Default"), Module("Administration"), TableName("Roles")]
[DisplayName("Roles"), InstanceName("Role")]
[ReadPermission(PermissionKeys.Security)]
[ModifyPermission(PermissionKeys.Security)]
[LookupScript]
public sealed class RoleRow : Row<RoleRow.RowFields>, IIdRow, INameRow
{
    [DisplayName("Role Id"), Identity, ForeignKey("Roles", "RoleId"), LeftJoin("jRole"), IdProperty]
    public int? RoleId { get => fields.RoleId[this]; set => fields.RoleId[this] = value; }

    [DisplayName("Role Name"), Size(100), NotNull, QuickSearch, NameProperty]
    public string RoleName { get => fields.RoleName[this]; set => fields.RoleName[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field RoleId;
        public StringField RoleName;
    }
}