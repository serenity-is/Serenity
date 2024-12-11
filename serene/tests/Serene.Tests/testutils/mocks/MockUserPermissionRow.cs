namespace Serenity.TestUtils;

public sealed class MockUserPermissionRow : Row<MockUserPermissionRow.RowFields>, IUserPermissionRow
{
    public int? UserId { get => fields.UserId[this]; set => fields.UserId[this] = value; }

    public string PermissionKey { get => fields.PermissionKey[this]; set => fields.PermissionKey[this] = value; }

    public bool? Granted { get => fields.Granted[this]; set => fields.Granted[this] = value; }

    Field IUserPermissionRow.UserIdField => fields.UserId;
    StringField IUserPermissionRow.PermissionKeyField => fields.PermissionKey;
    BooleanField IUserPermissionRow.GrantedField => fields.Granted;

    public class RowFields : RowFieldsBase
    {
        public Int32Field UserId;
        public StringField PermissionKey;
        public BooleanField Granted;
    }
}