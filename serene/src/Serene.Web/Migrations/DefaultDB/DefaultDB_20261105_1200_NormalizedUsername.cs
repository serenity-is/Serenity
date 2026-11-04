using FluentMigrator;

namespace Serene.Migrations.DefaultDB;

[DefaultDB, MigrationKey(20261105_1200)]
public class DefaultDB_20261105_1200_NormalizedUsername(IOptions<UserEntityOptions>? userEntityOptions = null) : Migration
{
    private const string ColumnName = "NormalizedUsername";

    public override void Up()
    {
        var usersTable = userEntityOptions?.Value?.TableName ?? "Users";

        // SQLite can't alter a column later, so it needs NOT NULL with a default from the start
        if (this.IsSqlite())
            Alter.Table(usersTable).AddColumn(ColumnName).AsString(100).NotNullable().WithDefaultValue("");
        else
            Alter.Table(usersTable).AddColumn(ColumnName).AsString(100).Nullable();

        var dialect = this.GetDialect();
        var table = dialect.QuoteIdentifier(usersTable);
        var column = dialect.QuoteIdentifier(ColumnName);
        var normalized = GetNormalizeExpression(dialect.QuoteIdentifier("Username"), dialect);
        Execute.Sql($"UPDATE {table} SET {column} = {normalized}");

        // FluentMigrator generates an empty statement for this alter on Firebird
        if (this.IsFirebird())
            Execute.Sql($"ALTER TABLE {table} ALTER COLUMN {column} SET NOT NULL");
        else if (!this.IsSqlite())
            Alter.Column(ColumnName).OnTable(usersTable).AsString(100).NotNullable();

        // this fails if there are users with usernames that only differ by case,
        // which is possible on case sensitive databases, they should be renamed first
        Create.Index("IX_Users_NormalizedUsername")
            .OnTable(usersTable)
            .OnColumn(ColumnName).Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
    }

    /// <summary>
    /// SQL equivalent of BaseUserRetrieveService.NormalizeUsername. All variants of the letter i are replaced
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
