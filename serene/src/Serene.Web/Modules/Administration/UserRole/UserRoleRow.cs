namespace Serene.Administration;

[ConnectionKey("Default"), Module("Administration"), TableName("UserRoles")]
[DisplayName("UserRoles"), InstanceName("UserRoles")]
[ReadPermission(PermissionKeys.Security)]
[ModifyPermission(PermissionKeys.Security)]
public sealed class UserRoleRow : Row<UserRoleRow.RowFields>, IIdRow, IUserRoleRow
{
    const string jRole = nameof(jRole);
    const string jUser = nameof(jUser);

    [DisplayName("User Role Id"), Identity, IdProperty]
    public long? UserRoleId { get => fields.UserRoleId[this]; set => fields.UserRoleId[this] = value; }

    [DisplayName("User Id"), NotNull, ForeignKey(typeof(UserRow)), LeftJoin(jUser)]
    public int? UserId { get => fields.UserId[this]; set => fields.UserId[this] = value; }

    [DisplayName("Role Id"), NotNull, ForeignKey(typeof(RoleRow)), LeftJoin(jRole)]
    public int? RoleId { get => fields.RoleId[this]; set => fields.RoleId[this] = value; }

    [DisplayName("Username"), Expression($"{jUser}.[Username]")]
    public string Username { get => fields.Username[this]; set => fields.Username[this] = value; }

    [DisplayName("User Display Name"), Expression($"{jUser}.[DisplayName]")]
    public string User { get => fields.User[this]; set => fields.User[this] = value; }

    [DisplayName("Role"), Expression($"{jRole}.[RoleName]")]
    public string RoleName { get => fields.RoleName[this]; set => fields.RoleName[this] = value; }

    public Field UserIdField => fields.UserId;

    public StringField RoleKeyOrNameField => fields.RoleName;

    public class RowFields : RowFieldsBase
    {
        public Int64Field UserRoleId;
        public Int32Field UserId;
        public Int32Field RoleId;

        public StringField Username;
        public StringField User;
        public StringField RoleName;
    }
}