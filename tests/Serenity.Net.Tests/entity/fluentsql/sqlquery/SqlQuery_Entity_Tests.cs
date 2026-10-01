namespace Serenity.Data;

public partial class SqlQuery_Entity_Tests
{
    private class MyField : IField
    {
        public string Name { get; set; }
        public string Expression { get; set; }
        public string ColumnAlias { get; set; }
    }

    private class MyEntity : IEntity
    {
        public string Table { get; set; }
    }

    [Fact]
    public void FromWithNullOrEmptyArgumentsThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().From((IEntity)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().From((IEntity)null));
    }

    [Fact]
    public void OrderByWithEmptyOrNullArgumentsThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy((MyField)null));
    }

    [Fact]
    public void FirstIntoRowShouldReturnNullIfNoEntityUsedYet()
    {
        Assert.Null(((ISqlQueryExtensible)new SqlQuery()).FirstIntoRow);
    }

    [Fact]
    public void FirstIntoRowShouldReturnFirstRowIfMoreThanOneEntityUsed()
    {
        var first = new MyEntity() { Table = "x" };
        var second = new MyEntity() { Table = "y" };
        var query = new SqlQuery().From(first).From(second.Table, Alias.T1).Into(second);
        Assert.Equal(first, ((ISqlQueryExtensible)query).FirstIntoRow);
    }

    [Fact]
    public void IntoRowIndexCanBeSetToMinusOneWithNull()
    {
        var entity = new MyEntity() { Table = "x" };
        var query = new SqlQuery().From(entity).Select("x1");
        Assert.Equal(0, ((ISqlQueryExtensible)query).Columns.ElementAt(0).IntoRowIndex);
        query.Into(null).Select("x2");
        Assert.Equal(-1, ((ISqlQueryExtensible)query).Columns.ElementAt(1).IntoRowIndex);
        Assert.Single(((ISqlQueryExtensible)query).IntoRows);
        Assert.Equal(entity, ((ISqlQueryExtensible)query).IntoRows[0]);
    }

    [Fact]
    public void IntoRowCanBeChanged()
    {
        var entity1 = new MyEntity() { Table = "x" };
        var query = new SqlQuery().From(entity1).Select("x1");
        Assert.Equal(0, ((ISqlQueryExtensible)query).Columns.ElementAt(0).IntoRowIndex);
        var entity2 = new MyEntity() { Table = "y" };
        query.Into(entity2).Select("y1");
        Assert.Equal(1, ((ISqlQueryExtensible)query).Columns.ElementAt(1).IntoRowIndex);
        Assert.Equal(2, ((ISqlQueryExtensible)query).IntoRows.Count);
        Assert.Equal(entity1, ((ISqlQueryExtensible)query).IntoRows[0]);
        Assert.Equal(entity2, ((ISqlQueryExtensible)query).IntoRows[1]);
    }

    [Fact]
    public void JoinIgnoresExistingJoinsWithSameAliasAndSameExpression()
    {
        var row = new ComplexRow();
        var fld = ComplexRow.Fields;
        var query = new SqlQuery()
            .From(row)
            .Select(fld.CountryName)
            .LeftJoin(new Alias("TheCountryTable", "c"), new Criteria("c", "TheCountryID") == new Criteria(0, "CountryID"));

        Assert.Equal(
            Normalize.Sql(
                "SELECT c.Name AS [CountryName] FROM [ComplexTable] T0 LEFT JOIN [TheCountryTable] c ON (c.[TheCountryID] = T0.[CountryID])"),
            Normalize.Sql(
                query.ToString()));

    }

    [Fact]
    public void JoinThrowsExceptionForJoinsWithSameAliasButDifferentExpression()
    {
        var row = new ComplexRow();
        var fld = ComplexRow.Fields;

        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            var query = new SqlQuery()
                .From(row)
                .Select(fld.CountryName)
                .LeftJoin(new Alias("City", "c"), new Criteria("c", "CityID") == new Criteria(0, "CountryID"));
        });

        Assert.Contains("already has a join 'c'", exception.Message);
        Assert.Contains("LEFT JOIN [TheCountryTable] c ON (c.[TheCountryID] = T0.[CountryID])", exception.Message);
        Assert.Contains("Attempted join expression is 'LEFT JOIN [City] c ON (c.[CityID] = T0.[CountryID])", exception.Message);
    }

    [Fact]
    public void JoinSkipsIdenticalJoinWithSameAlias()
    {
        var query = new SqlQuery()
            .From("T")
            .LeftJoin(new Alias("Other", "o"), new Criteria("o", "Id") == new Criteria("T", "Oid"));

        // same join again is gracefully skipped, not duplicated
        query.LeftJoin(new Alias("Other", "o"), new Criteria("o", "Id") == new Criteria("T", "Oid"));

        Assert.Single(System.Text.RegularExpressions.Regex.Matches(query.ToString(), "LEFT JOIN"));
    }

    [Fact]
    public void JoinSkipsIdenticalJoinWithSameManualParam()
    {
        var query = new SqlQuery()
            .From("T")
            .LeftJoin(new Alias("Other", "o"), new Criteria("o", "Id") == new ParamCriteria("@oid"));

        // manually named params render literally, so equality is verifiable
        query.LeftJoin(new Alias("Other", "o"), new Criteria("o", "Id") == new ParamCriteria("@oid"));

        Assert.Single(System.Text.RegularExpressions.Regex.Matches(query.ToString(), "LEFT JOIN"));
    }

    [Fact]
    public void JoinThrowsForSameAliasWhenOnDiffersOnlyInValues()
    {
        var query = new SqlQuery()
            .From("T")
            .LeftJoin(new Alias("Other", "o"), new Criteria("o", "Id") == 5);

        // both render as "o.[Id] = @p0" when params are ignored, but the
        // values differ, so this must throw instead of silently keeping 5
        var exception = Assert.Throws<InvalidOperationException>(() =>
            query.LeftJoin(new Alias("Other", "o"), new Criteria("o", "Id") == 6));

        Assert.Contains("already has a join 'o'", exception.Message);
        Assert.Contains("cannot be verified", exception.Message);
    }
}