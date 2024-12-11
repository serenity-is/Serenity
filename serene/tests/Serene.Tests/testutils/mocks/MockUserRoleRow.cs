namespace Serenity.TestUtils;

public class MockUserRoleRow : Row<MockUserRoleRow.RowFields>, IUserRoleRow
{
    public int? UserId { get => fields.UserId[this]; set => fields.UserId[this] = value; }
    public string RoleName { get => fields.RoleName[this]; set => fields.RoleName[this] = value; }
    
    public Field UserIdField => fields.UserId;
    public StringField RoleKeyOrNameField => fields.RoleName;

    public class RowFields : RowFieldsBase
    {
        public Int32Field UserId;
        public StringField RoleName;
    }
}