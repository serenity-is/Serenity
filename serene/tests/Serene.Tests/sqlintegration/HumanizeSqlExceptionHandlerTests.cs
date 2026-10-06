using System.Data.Common;
using Serenity.Data;

namespace Serenity.Services.SqlErrors;

public class HumanizeSqlExceptionHandlerTests
{
    [TableName("HumanizeIT_Parent")]
    [ReadPermission(SpecialPermissionKeys.Public)]
    private class ParentRow : Row<ParentRow.RowFields>, IIdRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public string? Code { get => fields.Code[this]; set => fields.Code[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public StringField Code = null!;
        }
    }

    private static IRequestContext CreateContext()
    {
        var behaviors = new MockBehaviorProvider((_, _, _) => new object[]
        {
            new HumanizeSqlExceptionBehavior(new DefaultSqlErrorExtractor())
        });
        return new NullRequestContext(behaviors).WithPermissions(_ => true);
    }

    private static DbConnection OpenSqlite()
    {
        var connection = SqlIntegrationConnections.CreateConnection("Sqlite");
        connection.Open();
        SqlIntegrationConnections.ExecuteSql(connection, "PRAGMA foreign_keys = ON");
        SqlIntegrationConnections.ExecuteSql(connection,
            "CREATE TABLE HumanizeIT_Parent (Id INTEGER NOT NULL PRIMARY KEY, Code TEXT UNIQUE)");
        SqlIntegrationConnections.ExecuteSql(connection,
            "CREATE TABLE HumanizeIT_Child (Id INTEGER NOT NULL PRIMARY KEY, ParentId INTEGER NOT NULL, " +
            "FOREIGN KEY (ParentId) REFERENCES HumanizeIT_Parent(Id))");
        return connection;
    }

    [Fact]
    public void Save_Duplicate_Code_Humanizes_Db_Unique_Violation()
    {
        using var actual = OpenSqlite();
        using var connection = new WrappedConnection(actual, SqliteDialect.Instance);
        var context = CreateContext();

        var handler = new SaveRequestHandler<ParentRow>(context);
        handler.Create(new MockUnitOfWork(connection), new() { Entity = new ParentRow { Id = 1, Code = "A" } });

        var error = Assert.Throws<ValidationError>(() => handler.Create(new MockUnitOfWork(connection),
            new() { Entity = new ParentRow { Id = 2, Code = "A" } }));

        Assert.Equal("UniqueViolation", error.ErrorCode);
    }

    [Fact]
    public void Delete_Referenced_Parent_Humanizes_Db_ForeignKey_Violation()
    {
        using var actual = OpenSqlite();
        SqlIntegrationConnections.ExecuteSql(actual, "INSERT INTO HumanizeIT_Parent (Id, Code) VALUES (1, 'A')");
        SqlIntegrationConnections.ExecuteSql(actual, "INSERT INTO HumanizeIT_Child (Id, ParentId) VALUES (1, 1)");
        using var connection = new WrappedConnection(actual, SqliteDialect.Instance);
        var context = CreateContext();

        var handler = new DeleteRequestHandler<ParentRow>(context);

        var error = Assert.Throws<ValidationError>(() => handler.Delete(new MockUnitOfWork(connection),
            new DeleteRequest { EntityId = 1 }));

        Assert.Equal("RelatedRecordExist", error.ErrorCode);
    }
}
