namespace Serenity.Data;

using Serenity.Data.Mapping;

public class EntitySqlQueryProjection_IntoForeignRowTests
{
    [Fact]
    public void IntoForeignRowUsesCurrentIntoRowForSelfReferences()
    {
        var row = new SelfNavigationRow();
        var query = new SqlQuery().From(row, out var fields);
        query.Select(fields.ID);
        query.IntoForeignRow<SelfNavigationRow.RowFields>(fields.Manager, (foreignFields, foreignQuery) =>
            foreignQuery.Select(foreignFields.ID));
        query.IntoForeignRow<SelfNavigationRow.RowFields>(fields.Mentor, (foreignFields, foreignQuery) =>
            foreignQuery.Select(foreignFields.ID));
        query.Select(fields.ManagerID);

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
    public void IntoForeignRowLoadsValuesIntoNestedRow()
    {
        var row = new SelfNavigationRow();
        var query = new SqlQuery().From(row, out var fields);
        query.Select(fields.ID, "ParentID");
        query.IntoForeignRow<SelfNavigationRow.RowFields>(fields.Manager, (foreignFields, foreignQuery) =>
            foreignQuery.Select(foreignFields.ID, "ManagerID"));

        using var reader = new MockDbDataReader(["ParentID", "ManagerID"], [new { ParentID = 10, ManagerID = 20 }]);
        Assert.True(reader.Read());

        query.GetFromReader(reader);

        Assert.Equal(10, row.ID);
        Assert.NotNull(row.Manager);
        Assert.Equal(20, row.Manager.ID);
    }

    [Fact]
    public void IntoForeignRowRestoresCurrentIntoRowWhenConfigureThrows()
    {
        var row = new SelfNavigationRow();
        var query = new SqlQuery().From(row);

        Assert.Throws<InvalidOperationException>(() => query.IntoForeignRow<SelfNavigationRow.RowFields>(
            SelfNavigationRow.Fields.Manager, (_, _) => throw new InvalidOperationException()));

        Assert.Same(row, ((ISqlQueryExtensible)query).CurrentIntoRow);
    }

    [Fact]
    public void IntoForeignRowOnFrozenQueryDoesNotChangeRow()
    {
        var row = new SelfNavigationRow();
        var query = new SqlQuery().From(row);
        query.Freeze();

        Assert.Throws<InvalidOperationException>(() => query.IntoForeignRow<SelfNavigationRow.RowFields>(
            SelfNavigationRow.Fields.Manager, (_, _) => { }));
        Assert.Null(row.Manager);
    }

    [Fact]
    public void IntoForeignRowRejectsNonRowField()
    {
        var row = new SelfNavigationRow();
        var query = new SqlQuery().From(row);

        Assert.Throws<ArgumentException>(() => query.IntoForeignRow<SelfNavigationRow.RowFields>(
            SelfNavigationRow.Fields.ID, (_, _) => { }));
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