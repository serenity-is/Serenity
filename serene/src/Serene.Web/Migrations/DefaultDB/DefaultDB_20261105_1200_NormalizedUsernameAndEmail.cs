using FluentMigrator;

namespace Serene.Migrations.DefaultDB;

[DefaultDB, MigrationKey(20261105_1200)]
public class DefaultDB_20261105_1200_NormalizedUsernameAndEmail(IOptions<UserEntityOptions>? userEntityOptions = null) : Migration
{
    private const string UsernameColumn = "NormalizedUsername";
    private const string EmailColumn = "NormalizedEmail";

    public override void Up()
    {
        var usersTable = userEntityOptions?.Value?.TableName ?? "Users";

        // SQLite can't alter a column later, so it needs NOT NULL with a default from the start
        if (this.IsSqlite())
            Alter.Table(usersTable).AddColumn(UsernameColumn).AsString(100).NotNullable().WithDefaultValue("");
        else
            Alter.Table(usersTable).AddColumn(UsernameColumn).AsString(100).Nullable();

        Alter.Table(usersTable).AddColumn(EmailColumn).AsString(100).Nullable();

        var dialect = this.GetDialect();
        var table = dialect.QuoteIdentifier(usersTable);
        var usernameColumn = dialect.QuoteIdentifier(UsernameColumn);
        var emailColumn = dialect.QuoteIdentifier(EmailColumn);
        Execute.Sql($"UPDATE {table} SET {usernameColumn} = {GetNormalizeExpression(dialect.QuoteIdentifier("Username"), dialect)}");
        Execute.Sql($"UPDATE {table} SET {emailColumn} = {GetNormalizeExpression(dialect.QuoteIdentifier("Email"), dialect)}");

        // FluentMigrator generates an empty statement for this alter on Firebird
        if (this.IsFirebird())
            Execute.Sql($"ALTER TABLE {table} ALTER COLUMN {usernameColumn} SET NOT NULL");
        else if (!this.IsSqlite())
            Alter.Column(UsernameColumn).OnTable(usersTable).AsString(100).NotNullable();

        // this fails if there are users with usernames that only differ by case,
        // which is possible on case sensitive databases, they should be renamed first
        Create.Index("IX_Users_NormalizedUsername")
            .OnTable(usersTable)
            .OnColumn(UsernameColumn).Ascending()
            .WithOptions().Unique();

        // emails are not unique, as nothing enforced that for the existing users
        Create.Index("IX_Users_NormalizedEmail")
            .OnTable(usersTable)
            .OnColumn(EmailColumn).Ascending();
    }

    public override void Down()
    {
    }

    /// <summary>
    /// SQL equivalent of BaseUserRetrieveService.NormalizeUsername and NormalizeEmail. All variants of the letter i are replaced
    /// first, so that UPPER has nothing left to turn into a dotted İ on a Turkish database.
    /// </summary>
    private static string GetNormalizeExpression(string column, ISqlDialect dialect)
    {
        var expression = column;
        foreach (var letter in new[] { "ı", "İ", "i" })
            expression = $"REPLACE({expression}, {dialect.QuoteUnicodeString(letter)}, 'I')";

        return $"UPPER({expression})";
    }
}
