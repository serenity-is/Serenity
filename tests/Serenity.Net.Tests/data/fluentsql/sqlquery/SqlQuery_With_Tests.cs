namespace Serenity.Tests.Data;

public class SqlQuery_With_Tests
{
    [Fact]
    public void WithPassesAndReturnsTheQueryItself()
    {
        var query = new SqlQuery();
        var afterWith = query.With(insideWidth =>
        {
            Assert.Equal(query, insideWidth);
        });

        Assert.Equal(query, afterWith);
    }
}