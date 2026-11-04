using Serenity.Extensions;

namespace Serenity.Testing;

[DefaultConnection]
public class MockNormalizedEmailUserRow : Row<MockNormalizedEmailUserRow.RowFields>, IIdRow, IEmailRow, IPasswordRow, INormalizedEmailRow
{
    [IdProperty]
    public int? UserId { get => fields.UserId[this]; set => fields.UserId[this] = value; }

    public string? Email { get => fields.Email[this]; set => fields.Email[this] = value; }
    public string? NormalizedEmail { get => fields.NormalizedEmail[this]; set => fields.NormalizedEmail[this] = value; }
    public string? PasswordHash { get => fields.PasswordHash[this]; set => fields.PasswordHash[this] = value; }
    public string? PasswordSalt { get => fields.PasswordSalt[this]; set => fields.PasswordSalt[this] = value; }

    StringField IEmailRow.EmailField => fields.Email;
    StringField INormalizedEmailRow.NormalizedEmailField => fields.NormalizedEmail;
    StringField IPasswordRow.PasswordHashField => fields.PasswordHash;
    StringField IPasswordRow.PasswordSaltField => fields.PasswordSalt;

    public class RowFields : RowFieldsBase
    {
        public Int32Field UserId = null!;
        public StringField Email = null!;
        public StringField NormalizedEmail = null!;
        public StringField PasswordHash = null!;
        public StringField PasswordSalt = null!;
    }
}
