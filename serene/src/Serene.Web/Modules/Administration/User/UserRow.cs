namespace Serene.Administration;

[ConnectionKey("Default"), Module("Administration"), TableName("Users")]
[DisplayName("Users"), InstanceName("User")]
[ReadPermission(PermissionKeys.Security)]
[ModifyPermission(PermissionKeys.Security)]
[LookupScript(Permission = PermissionKeys.Security)]
public sealed class UserRow : Serenity.Extensions.Entities.LoggingRow<UserRow.RowFields>, IIdRow, INameRow, IIsActiveRow, IDisplayNameRow, IEmailRow, IPasswordRow
{
    [DisplayName("User Id"), Identity, IdProperty]
    public int? UserId { get => fields.UserId[this]; set => fields.UserId[this] = value; }

    [DisplayName("Username"), Size(100), NotNull, QuickSearch, LookupInclude, NameProperty]
    public string Username { get => fields.Username[this]; set => fields.Username[this] = value; }

    [DisplayName("Source"), Size(4), NotNull, Insertable(false), Updatable(false), DefaultValue("site")]
    public string Source { get => fields.Source[this]; set => fields.Source[this] = value; }

    [DisplayName("Password Hash"), Size(86), NotNull, Insertable(false), Updatable(false), MinSelectLevel(SelectLevel.Never)]
    public string PasswordHash { get => fields.PasswordHash[this]; set => fields.PasswordHash[this] = value; }

    [DisplayName("Password Salt"), Size(10), NotNull, Insertable(false), Updatable(false), MinSelectLevel(SelectLevel.Never)]
    public string PasswordSalt { get => fields.PasswordSalt[this]; set => fields.PasswordSalt[this] = value; }

    [DisplayName("Display Name"), Size(100), NotNull, LookupInclude]
    public string DisplayName { get => fields.DisplayName[this]; set => fields.DisplayName[this] = value; }

    [DisplayName("Email"), Size(100)]
    public string Email { get => fields.Email[this]; set => fields.Email[this] = value; }

    [DisplayName("User Image"), Size(100)]
    [ImageUploadEditor(FilenameFormat = "UserImage/~", CopyToHistory = true)]
    public string UserImage { get => fields.UserImage[this]; set => fields.UserImage[this] = value; }

    [DisplayName("Password"), Size(50), NotMapped]
    public string Password { get => fields.Password[this]; set => fields.Password[this] = value; }

    [DisplayName("Activated"), NotNull, Insertable(false), Updatable(true)]
    public short? IsActive { get => fields.IsActive[this]; set => fields.IsActive[this] = value; }

    [DisplayName("Confirm Password"), Size(50), NotMapped]
    public string PasswordConfirm { get => fields.PasswordConfirm[this]; set => fields.PasswordConfirm[this] = value; }

    [DisplayName("Last Directory Update"), Insertable(false), Updatable(false)]
    public DateTime? LastDirectoryUpdate { get => fields.LastDirectoryUpdate[this]; set => fields.LastDirectoryUpdate[this] = value; }

    [DisplayName("Roles"), LinkingSetRelation(typeof(UserRoleRow), nameof(UserRoleRow.UserId), nameof(UserRoleRow.RoleId))]
    [AsyncLookupEditor(typeof(RoleRow), Multiple = true)]
    public List<int> Roles { get => fields.Roles[this]; set => fields.Roles[this] = value; }

    StringField IDisplayNameRow.DisplayNameField => fields.DisplayName;
    StringField IEmailRow.EmailField => fields.Email;
    Int16Field IIsActiveRow.IsActiveField => fields.IsActive;
    StringField IPasswordRow.PasswordHashField => fields.PasswordHash;
    StringField IPasswordRow.PasswordSaltField => fields.PasswordSalt;

    public class RowFields : Serenity.Extensions.Entities.LoggingRowFields
    {
        public Int32Field UserId;
        public StringField Username;
        public StringField Source;
        public StringField PasswordHash;
        public StringField PasswordSalt;
        public StringField DisplayName;
        public StringField Email;
        public StringField UserImage;
        public DateTimeField LastDirectoryUpdate;
        public Int16Field IsActive;

        public StringField Password;
        public StringField PasswordConfirm;

        public ListField<int> Roles;
    }
}