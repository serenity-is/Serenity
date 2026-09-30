namespace Serenity.Data;

public class SqlQuery_With_Tests
{
    [Fact]
    public void WithSelfPassesAndReturnsTheQueryItself()
    {
        var query = new SqlQuery();
        var afterWith = query.WithSelf(out var me);
        Assert.Equal(query, me);
        Assert.Equal(query, afterWith);
    }
}