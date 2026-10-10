using System.Data.Common;

namespace Serenity.Services.SqlErrors;

/// <summary>
/// Shared integration tests that run against a real database to verify that
/// <see cref="DefaultSqlErrorExtractor"/> recognizes primary key, unique, foreign
/// key and not null constraint violations. Tables use randomized names and are
/// dropped after each test.
/// </summary>
public abstract class SqlErrorIntegrationTests_Base
{
    /// <summary>
    /// Creates a new instance with randomized table names.
    /// </summary>
    protected SqlErrorIntegrationTests_Base()
    {
        var prefix = "SIT_" + Guid.NewGuid().ToString("N")[..8] + "_";
        ParentTable = prefix + "Parent";
        ChildTable = prefix + "Child";
    }

    /// <summary>
    /// Randomized parent table name.
    /// </summary>
    protected string ParentTable { get; }

    /// <summary>
    /// Randomized child table name.
    /// </summary>
    protected string ChildTable { get; }

    /// <summary>
    /// Provider name passed to <see cref="SqlIntegrationConnections.CreateConnection(string)"/>.
    /// </summary>
    protected abstract string Provider { get; }

    /// <summary>
    /// Expected server type for the provider.
    /// </summary>
    protected abstract ServerType ExpectedServerType { get; }

    /// <summary>
    /// Creates the parent and child tables.
    /// </summary>
    protected abstract void CreateTables(DbConnection connection);

    /// <summary>
    /// Drops the parent and child tables.
    /// </summary>
    protected abstract void DropTables(DbConnection connection);

    /// <summary>
    /// Returns an insert statement for the parent table.
    /// </summary>
    protected abstract string InsertParent(int id, string code, string? required);

    /// <summary>
    /// Returns an insert statement for the child table.
    /// </summary>
    protected abstract string InsertChild(int id, int parentId);

    /// <summary>
    /// Returns a delete statement for the parent table.
    /// </summary>
    protected abstract string DeleteParent(int id);

    /// <summary>
    /// Executes a SQL statement.
    /// </summary>
    protected static void Execute(DbConnection connection, string sql) =>
        SqlIntegrationConnections.ExecuteSql(connection, sql);

    private SqlErrorInfo RunScenario(Action<DbConnection> setup, string violatingSql)
    {
        using var connection = SqlIntegrationConnections.CreateConnection(Provider);
        connection.Open();
        try
        {
            CreateTables(connection);
            setup(connection);

            var exception = Assert.ThrowsAny<Exception>(() => Execute(connection, violatingSql));
            var info = new DefaultSqlErrorExtractor().Extract(exception);

            Assert.NotNull(info);
            Assert.Equal(ExpectedServerType, info!.ServerType);
            return info;
        }
        finally
        {
            DropTables(connection);
        }
    }

    private static void AssertUniquenessType(SqlErrorInfo info) =>
        Assert.True(info.Type is SqlErrorConstraintType.PrimaryKey or SqlErrorConstraintType.Unique,
            $"Expected PrimaryKey or Unique, but got {info.Type}");

    /// <summary>
    /// Verifies that a unique constraint violation is recognized.
    /// </summary>
    [Fact]
    public void UniqueConstraint_Is_Recognized()
    {
        if (SqlIntegrationConnections.ShouldSkip(Provider))
            return;

        var info = RunScenario(
            connection => Execute(connection, InsertParent(1, "CODE_A", "value")),
            InsertParent(2, "CODE_A", "value"));

        AssertUniquenessType(info);
    }

    /// <summary>
    /// Verifies that a primary key violation is recognized.
    /// </summary>
    [Fact]
    public void PrimaryKeyConstraint_Is_Recognized()
    {
        if (SqlIntegrationConnections.ShouldSkip(Provider))
            return;

        var info = RunScenario(
            connection => Execute(connection, InsertParent(1, "CODE_A", "value")),
            InsertParent(1, "CODE_B", "value"));

        AssertUniquenessType(info);
    }

    /// <summary>
    /// Verifies that a foreign key violation on insert is recognized.
    /// </summary>
    [Fact]
    public void InsertForeignKeyConstraint_Is_Recognized()
    {
        if (SqlIntegrationConnections.ShouldSkip(Provider))
            return;

        var info = RunScenario(
            connection => { },
            InsertChild(1, 987654));

        Assert.Equal(SqlErrorConstraintType.ForeignKey, info.Type);
    }

    /// <summary>
    /// Verifies that a foreign key violation on delete is recognized.
    /// </summary>
    [Fact]
    public void DeleteForeignKeyConstraint_Is_Recognized()
    {
        if (SqlIntegrationConnections.ShouldSkip(Provider))
            return;

        var info = RunScenario(
            connection =>
            {
                Execute(connection, InsertParent(1, "CODE_A", "value"));
                Execute(connection, InsertChild(1, 1));
            },
            DeleteParent(1));

        Assert.Equal(SqlErrorConstraintType.ForeignKey, info.Type);
    }

    /// <summary>
    /// Verifies that a not null constraint violation is recognized.
    /// </summary>
    [Fact]
    public void NotNullConstraint_Is_Recognized()
    {
        if (SqlIntegrationConnections.ShouldSkip(Provider))
            return;

        var info = RunScenario(
            connection => { },
            InsertParent(1, "CODE_A", null));

        Assert.Equal(SqlErrorConstraintType.NotNull, info.Type);
    }
}
