namespace Serenity.Tests.Data;

public class SqlQuery_Select_Tests
{
    [Fact]
    public void SelectWithEmptyOrNullArgumentsThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((string)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(String.Empty));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((string)null, "x"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(String.Empty, "y"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select("x", (string)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select("y", String.Empty));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((Alias)null, "x"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("a"), (string)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("a"), String.Empty));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((Alias)null, "x", "y"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("a"), (string)null, "x"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("b"), String.Empty, "y"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("c"), "x", (string)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("c"), "x", String.Empty));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((ISqlQuery)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((ISqlQuery)null, "x"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new SqlQuery(), (string)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new SqlQuery(), String.Empty));
    }
}