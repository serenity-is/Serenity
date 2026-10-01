namespace Serenity.Data;

public class SqlQuery_Select_Tests
{
    private sealed class AliasWithJoins(string table, string name)
        : Alias(table, name), IHaveJoins
    {
        public IDictionary<string, Join> Joins { get; } = new Dictionary<string, Join>();
    }

    [Fact]
    public void SelectWithEmptyOrNullArgumentsThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((string)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(String.Empty));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((string)null, "x"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(String.Empty, "y"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select("x", null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select("y", String.Empty));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((Alias)null, "x"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("a"), null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("a"), String.Empty));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(null, "x", "y"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("a"), null, "x"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("b"), String.Empty, "y"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("c"), "x", null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new Alias("c"), "x", String.Empty));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((ISqlQuery)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((ISqlQuery)null, "x"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new SqlQuery(), null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(new SqlQuery(), String.Empty));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Select_Alias_Overloads_Ensure_Referenced_Joins(int overload)
    {
        var root = new AliasWithJoins("Base", "T0");
        _ = new LeftJoin(root.Joins, "Related", "T1", null);
        var query = new SqlQuery().From(root);

        switch (overload)
        {
            case 0:
                query.Select("T1.Value");
                break;
            case 1:
                query.Select(new Alias("T1"), "Value");
                break;
            case 2:
                query.Select(new Alias("T1"), "Value", "ValueAlias");
                break;
            case 3:
                query.Select("T1.Value", "ValueAlias");
                break;
        }

        Assert.Contains("LEFT JOIN [Related] T1", Normalize.Sql(query.ToString()));
    }
}
