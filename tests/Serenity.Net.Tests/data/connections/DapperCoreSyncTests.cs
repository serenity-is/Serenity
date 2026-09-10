namespace Serenity.Data;

public class DapperCoreSyncTests
{
    private class TestRow
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private static SqlQuery CreateQuery()
    {
        var query = new SqlQuery()
            .From("Table")
            .Select("Id")
            .Select("Name")
            .Where("Id = @Id");
        query.AddParam("@Id", 1);
        return query;
    }

    [Fact]
    public void Execute_Executes_AndReturnsRowsAffected()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(command => 5);

        var result = connection.Execute("UPDATE [Table] SET [X] = 1");

        Assert.Equal(5, result);
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    public void Query_Returns_DynamicObjects()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command => new MockDbDataReader(new { Id = 1, Name = "Test" }));

        var list = connection.Query("SELECT * FROM [Table]").ToList();

        Assert.Single(list);
        Assert.Equal(1, (int)list[0].Id);
    }

    [Fact]
    public void Query_ISqlQuery_Returns_DynamicObjects()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command => new MockDbDataReader(new { Id = 1, Name = "Test" }));

        var list = connection.Query(CreateQuery()).ToList();

        Assert.Single(list);
        Assert.Equal(1, (int)list[0].Id);
    }

    [Fact]
    public void Query_T_Returns_TypedObjects()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command => new MockDbDataReader(new { Id = 1, Name = "Test" }));

        var list = connection.Query<TestRow>("SELECT * FROM [Table]").ToList();

        Assert.Single(list);
        Assert.Equal(1, list[0].Id);
    }

    [Fact]
    public void Query_T_ISqlQuery_Returns_TypedObjects()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command => new MockDbDataReader(new { Id = 1, Name = "Test" }));

        var list = connection.Query<TestRow>(CreateQuery()).ToList();

        Assert.Single(list);
        Assert.Equal(1, list[0].Id);
    }
}
