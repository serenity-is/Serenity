namespace Serenity.Data;

public class SqlQuery_WhereEqual_Tests
{
    private class MyField : IField
    {
        public string Name { get; set; } = "Name";
        public string Expression { get; set; } = "T0.Name";
        public string ColumnAlias { get; set; } = "Name";
    }

    [Fact]
    public void WhereEqual_WithValue_AddsEqualityWithParam()
    {
        var query = new SqlQuery().From("T").WhereEqual(new MyField(), "x");

        Assert.Contains("T0.Name = @p1", query.ToString());
        Assert.Equal("x", query.Params!["@p1"]);
    }

    [Fact]
    public void WhereEqual_WithNull_Default_AddsNeverTrueEquality()
    {
        // Intentional: matches SQL three-valued logic and Criteria == semantics.
        var query = new SqlQuery().From("T").WhereEqual(new MyField(), null);

        Assert.Contains("T0.Name = @p1", query.ToString());
        Assert.True(query.Params!.ContainsKey("@p1"));
        Assert.Null(query.Params["@p1"]);
    }

    [Fact]
    public void WhereEqual_WithNull_EmitIsNull_AddsIsNull()
    {
        var query = new SqlQuery().From("T").WhereEqual(new MyField(), null, emitIsNull: true);

        Assert.Contains("T0.Name IS NULL", query.ToString());
        Assert.True(query.Params is null || query.Params.Count == 0);
    }
}
