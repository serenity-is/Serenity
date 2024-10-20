namespace Serene.Administration;

[ConnectionKey("Default"), Module("Administration"), TableName("UserPermissions")]
[DisplayName("UserPermissions"), InstanceName("UserPermissions")]
[ReadPermission(PermissionKeys.Security)]
[ModifyPermission(PermissionKeys.Security)]
public sealed class UserPermissionRow : Row<UserPermissionRow.RowFields>, IIdRow, INameRow, IUserPermissionRow
{
    [DisplayName("User Permission Id"), Identity, IdProperty]
    public long? UserPermissionId { get => fields.UserPermissionId[this]; set => fields.UserPermissionId[this] = value; }

    [DisplayName("User Id"), NotNull, ForeignKey("Users", "UserId"), LeftJoin("jUser")]
    public int? UserId { get => fields.UserId[this]; set => fields.UserId[this] = value; }

    [DisplayName("Permission Key"), Size(100), NotNull, QuickSearch, NameProperty]
    public string PermissionKey { get => fields.PermissionKey[this]; set => fields.PermissionKey[this] = value; }

    [DisplayName("Grant")]
    public bool? Granted { get => fields.Granted[this]; set => fields.Granted[this] = value; }

    [DisplayName("User Username"), Expression("jUser.[Username]")]
    public string Username { get => fields.Username[this]; set => fields.Username[this] = value; }

    [DisplayName("User Display Name"), Expression("jUser.[DisplayName]")]
    public string User { get => fields.User[this]; set => fields.User[this] = value; }

    Field IUserPermissionRow.UserIdField => fields.UserId;
    StringField IUserPermissionRow.PermissionKeyField => fields.PermissionKey;
    BooleanField IUserPermissionRow.GrantedField => fields.Granted;

    public class RowFields : RowFieldsBase
    {
        public Int64Field UserPermissionId;
        public Int32Field UserId;
        public StringField PermissionKey;
        public BooleanField Granted;

        public StringField Username;
        public StringField User;
    }
}