namespace Serenity.TestUtils;

[ConnectionKey("Default")]
public class MockUserRow : Row<MockUserRow.RowFields>, IIdRow, IEmailRow, IPasswordRow,
    IUpdateDateRow, IInsertDateRow, IDisplayNameRow, INameRow
{
    [IdProperty]
    public int? UserId { get => fields.UserId[this]; set => fields.UserId[this] = value; }

    [NameProperty]
    public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

    public string? Email { get => fields.Email[this]; set => fields.Email[this] = value; }
    public string? PasswordHash { get => fields.PasswordHash[this]; set => fields.PasswordHash[this] = value; }
    public string? PasswordSalt { get => fields.PasswordSalt[this]; set => fields.PasswordSalt[this] = value; }
    public DateTime? UpdateDate { get => fields.UpdateDate[this]; set => fields.UpdateDate[this] = value; }
    public DateTime? InsertDate { get => fields.InsertDate[this]; set => fields.InsertDate[this] = value; }
    public string? DisplayName { get => fields.DisplayName[this]; set => fields.DisplayName[this] = value; }

    StringField IEmailRow.EmailField => fields.Email;
    StringField IPasswordRow.PasswordHashField => fields.PasswordHash;
    StringField IPasswordRow.PasswordSaltField => fields.PasswordSalt;
    DateTimeField IUpdateDateRow.UpdateDateField => fields.UpdateDate;
    DateTimeField IInsertDateRow.InsertDateField => fields.InsertDate;
    StringField IDisplayNameRow.DisplayNameField => fields.DisplayName;

    public class RowFields : RowFieldsBase
    {
        public Int32Field UserId = null!;
        public StringField Name = null!;
        public StringField Email = null!;
        public StringField PasswordHash = null!;
        public StringField PasswordSalt = null!;
        public DateTimeField UpdateDate = null!;
        public DateTimeField InsertDate = null!;
        public StringField DisplayName = null!;
    }
}
