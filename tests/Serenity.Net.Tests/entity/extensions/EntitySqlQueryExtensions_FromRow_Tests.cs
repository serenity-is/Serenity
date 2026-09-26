namespace Serenity.Data;

using Serenity.Data.Mapping;

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

    [Fact]
    public void SelectIntoForeignRowUsesCurrentIntoRowForSelfReferences()
    {
        var row = new SelfNavigationRow();
        var query = new SqlQuery().From(row, (fields, query) =>
        {
            query.Select(fields.ID);
            query.SelectIntoForeignRow<SelfNavigationRow.RowFields>(fields.Manager, (foreignFields, foreignQuery) =>
                foreignQuery.Select(foreignFields.ID));
            query.SelectIntoForeignRow<SelfNavigationRow.RowFields>(fields.Mentor, (foreignFields, foreignQuery) =>
                foreignQuery.Select(foreignFields.ID));
            query.Select(fields.ManagerID);
        });

        var extensible = (ISqlQueryExtensible)query;
        Assert.Same(row, extensible.CurrentIntoRow);
        Assert.NotNull(row.Manager);
        Assert.NotNull(row.Mentor);
        Assert.NotSame(row.Manager, row.Mentor);
        Assert.Equal(new[] { 0, 1, 2, 0 }, extensible.Columns.Select(column => column.IntoRowIndex));
        Assert.Contains("jManager", query.ToString(), StringComparison.Ordinal);
        Assert.Contains("jMentor", query.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void SelectIntoForeignRowLoadsValuesIntoNestedRow()
    {
        var row = new SelfNavigationRow();
        var query = new SqlQuery().From(row, (fields, query) =>
        {
            query.Select(fields.ID, "ParentID");
            query.SelectIntoForeignRow<SelfNavigationRow.RowFields>(fields.Manager, (foreignFields, foreignQuery) =>
                foreignQuery.Select(foreignFields.ID, "ManagerID"));
        });

        using var reader = new MockDbDataReader(["ParentID", "ManagerID"], [new { ParentID = 10, ManagerID = 20 }]);
        Assert.True(reader.Read());

        query.GetFromReader(reader);

        Assert.Equal(10, row.ID);
        Assert.NotNull(row.Manager);
        Assert.Equal(20, row.Manager.ID);
    }

    [Fact]
    public void SelectIntoForeignRowRestoresCurrentIntoRowWhenConfigureThrows()
    {
        var row = new SelfNavigationRow();
        var query = new SqlQuery().From(row);

        Assert.Throws<InvalidOperationException>(() => query.SelectIntoForeignRow<SelfNavigationRow.RowFields>(
            SelfNavigationRow.Fields.Manager, (_, _) => throw new InvalidOperationException()));

        Assert.Same(row, ((ISqlQueryExtensible)query).CurrentIntoRow);
    }

    [Fact]
    public void SelectIntoForeignRowRejectsNonRowField()
    {
        var row = new SelfNavigationRow();
        var query = new SqlQuery().From(row);

        Assert.Throws<ArgumentException>(() => query.SelectIntoForeignRow<SelfNavigationRow.RowFields>(
            SelfNavigationRow.Fields.ID, (_, _) => { }));
    }

    private static void AssertSubQueryFromUsesUniqueAliases(QueryWithParams query)
    {
        var aliases = new List<string?>();

        query.SubQueryFrom(ComplexRow.Fields, (fields, _) => aliases.Add(fields.AliasName));
        query.SubQueryFrom(ComplexRow.Fields, (fields, _) => aliases.Add(fields.AliasName));

        Assert.Equal(new string?[] { "T1", "T2" }, aliases);
    }

    [TableName("SelfNavigation")]
    public sealed class SelfNavigationRow : Row<SelfNavigationRow.RowFields>
    {
        private const string jManager = nameof(jManager);
        private const string jMentor = nameof(jMentor);

        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [ForeignKey(typeof(SelfNavigationRow), nameof(ID)), LeftJoin(jManager)]
        public int? ManagerID { get => fields.ManagerID[this]; set => fields.ManagerID[this] = value; }

        [ForeignRow(nameof(ManagerID))]
        public SelfNavigationRow? Manager { get => fields.Manager[this]; set => fields.Manager[this] = value; }

        [ForeignKey(typeof(SelfNavigationRow), nameof(ID)), LeftJoin(jMentor)]
        public int? MentorID { get => fields.MentorID[this]; set => fields.MentorID[this] = value; }

        [ForeignRow(nameof(MentorID))]
        public SelfNavigationRow? Mentor { get => fields.Mentor[this]; set => fields.Mentor[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
            public Int32Field ManagerID = null!;
            public RowField<SelfNavigationRow> Manager = null!;
            public Int32Field MentorID = null!;
            public RowField<SelfNavigationRow> Mentor = null!;
        }
    }
}
