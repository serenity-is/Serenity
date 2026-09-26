namespace Serenity.Data;

public partial class EntitySqlQueryExtensions_FromRow_Tests
{
    [Fact]
    public void FromAssignsRootAliasT0()
    {
        var query = new SqlQuery().From(new ComplexRow(), (fields, sql) =>
            sql.Select(fields.ID));

        Assert.Equal(Normalize.Sql("SELECT T0.[ComplexID] AS [ID] FROM [ComplexTable] T0"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void FromAndSubQueryFromAssignUniqueAliasesAndConfigureChild()
    {
        var query = new SqlQuery().From(ComplexRow.Fields.As("rp"), (fields, query) => query
            .Select(fields.ID)
                .Where(fields.ID.In(query.SubQueryFrom(ComplexRow.Fields, (subFields, subquery) =>
                    subquery.Select(subFields.ID)))));

        Assert.Equal(Normalize.Sql("SELECT rp.[ComplexID] AS [ID] FROM [ComplexTable] rp WHERE (rp.[ComplexID] IN (SELECT T1.[ComplexID] AS [ID] FROM [ComplexTable] T1))"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void NestedSubQueryFromContinuesAliasSequence()
    {
        var query = new SqlQuery().From(new ComplexRow(), (fields, query) =>
        {
            var subquery = query.SubQueryFrom(ComplexRow.Fields, (subFields, subquery) =>
            {
                var nestedQuery = subquery.SubQueryFrom(ComplexRow.Fields, (nestedFields, nestedQuery) =>
                    nestedQuery.Select(nestedFields.ID));

                subquery.Select(subFields.ID)
                    .Where(subFields.ID.In(nestedQuery));
            });

            query.Select(fields.ID)
                .Where(fields.ID.In(subquery));
        });

        Assert.Equal(Normalize.Sql("SELECT T0.[ComplexID] AS [ID] FROM [ComplexTable] T0 WHERE (T0.[ComplexID] IN (SELECT T1.[ComplexID] AS [ID] FROM [ComplexTable] T1 WHERE (T1.[ComplexID] IN (SELECT T2.[ComplexID] AS [ID] FROM [ComplexTable] T2))))"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void SubQueryCanAllocateAliasBeforeRootSourceIsAdded()
    {
        var query = new SqlQuery();
        var subquery = query.SubQueryFrom(ComplexRow.Fields, (subFields, subquery) =>
            subquery.Select(subFields.ID));

        query.From(new ComplexRow(), (fields, query) =>
            query.Select(fields.ID).Where(fields.ID.In(subquery)));

        Assert.Equal(Normalize.Sql("SELECT T0.[ComplexID] AS [ID] FROM [ComplexTable] T0 WHERE (T0.[ComplexID] IN (SELECT T1.[ComplexID] AS [ID] FROM [ComplexTable] T1))"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void SubQueryFromUsesUniqueAliasesForSqlQuery()
    {
        AssertSubQueryFromUsesUniqueAliases(new SqlQuery());
    }

    [Fact]
    public void SubQueryFromUsesUniqueAliasesForSqlDelete()
    {
        AssertSubQueryFromUsesUniqueAliases(new SqlDelete("ComplexTable"));
    }

    [Fact]
    public void SubQueryFromUsesUniqueAliasesForSqlInsert()
    {
        AssertSubQueryFromUsesUniqueAliases(new SqlInsert("ComplexTable"));
    }

    private static void AssertSubQueryFromUsesUniqueAliases(QueryWithParams query)
    {
        var aliases = new List<string?>();

        query.SubQueryFrom(ComplexRow.Fields, (fields, _) => aliases.Add(fields.AliasName));
        query.SubQueryFrom(ComplexRow.Fields, (fields, _) => aliases.Add(fields.AliasName));

        Assert.Equal(new string?[] { "T1", "T2" }, aliases);
    }
}
