namespace Serene.Administration;

[DefaultConnection, AdministrationModule, TableName("UserPermissions")]
[DisplayText("UserPermissions"), InstanceName("UserPermissions")]
[ReadPermission(PermissionKeys.Security)]
[ModifyPermission(PermissionKeys.Security)]
public sealed class UserPermissionRow : Row<UserPermissionRow.RowFields>, IIdRow, INameRow, IUserPermissionRow
{
    [DisplayText("User Permission Id"), Identity, IdProperty]
    public long? UserPermissionId { get => fields.UserPermissionId[this]; set => fields.UserPermissionId[this] = value; }

    [DisplayText("User Id"), NotNull, ForeignKey(typeof(UserRow)), LeftJoin("jUser")]
    public int? UserId { get => fields.UserId[this]; set => fields.UserId[this] = value; }

    [DisplayText("Permission Key"), Size(100), NotNull, QuickSearch, NameProperty]
    public string? PermissionKey { get => fields.PermissionKey[this]; set => fields.PermissionKey[this] = value; }

    [DisplayText("Grant")]
    public bool? Granted { get => fields.Granted[this]; set => fields.Granted[this] = value; }

    [DisplayText("User Username"), Expression("jUser.[Username]")]
    public string? Username { get => fields.Username[this]; set => fields.Username[this] = value; }

    [DisplayText("User Display Name"), Expression("jUser.[DisplayName]")]
    public string? User { get => fields.User[this]; set => fields.User[this] = value; }

    Field IUserPermissionRow.UserIdField => fields.UserId;
    StringField IUserPermissionRow.PermissionKeyField => fields.PermissionKey;
    BooleanField IUserPermissionRow.GrantedField => fields.Granted;

    public class RowFields : RowFieldsBase
    {
        public Int64Field UserPermissionId = null!;
        public Int32Field UserId = null!;
        public StringField PermissionKey = null!;
        public BooleanField Granted = null!;

        public StringField Username = null!;
        public StringField User = null!;
    }
}