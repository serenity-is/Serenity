namespace Serenity.Web;

public class RowLookupScriptTests
{
    private class TreeRow : Row<TreeRow.RowFields>, IIdRow, INameRow, IParentIdRow
    {
        [IdProperty, Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public int? ParentId { get => fields.ParentId[this]; set => fields.ParentId[this] = value; }

        public Field ParentIdField => fields.ParentId;

        [LookupInclude]
        public string? Code { get => fields.Code[this]; set => fields.Code[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Name;
            public Int32Field ParentId;
            public StringField Code;
#pragma warning restore CS0649
        }
    }

    private static MockSqlConnections Connections()
    {
        return new MockSqlConnections
        {
            OnNewByKey = _ => new MockDbConnection()
                .InterceptExecuteReader(_ => new MockDbDataReader())
        };
    }

    [Fact]
    public void Constructor_Throws_For_Null_Connections()
    {
        Assert.Throws<ArgumentNullException>(() => new RowLookupScript<IdNameRow>(null!));
    }

    [Fact]
    public void Constructor_Reads_Row_Fields()
    {
        var script = new RowLookupScript<IdNameRow>(Connections());

        Assert.Equal("ID", script.IdField);
        Assert.Equal("Name", script.TextField);
        Assert.Null(script.ParentIdField);
    }

    [Fact]
    public void GetScript_Returns_Empty_Lookup_With_No_Rows()
    {
        var script = new RowLookupScript<IdNameRow>(Connections())
        {
            LookupKey = "Test"
        };

        var result = script.GetScript();

        Assert.Contains("Lookup.Test", result);
        Assert.Contains("new ", result);
    }

    [Fact]
    public void GetScriptData_Returns_Data()
    {
        var script = new RowLookupScript<IdNameRow>(Connections());

        Assert.NotNull(script.GetScriptData());
    }

    [Fact]
    public void Constructor_Sets_ParentIdField_For_Tree_Row()
    {
        var script = new RowLookupScript<TreeRow>(Connections());

        Assert.Equal("ParentId", script.ParentIdField);
        Assert.Equal("Id", script.IdField);
        Assert.Equal("Name", script.TextField);
    }

    [Fact]
    public void GetScript_Selects_LookupInclude_Fields()
    {
        var script = new RowLookupScript<TreeRow>(Connections()) { LookupKey = "Tree" };

        Assert.Contains("Lookup.Tree", script.GetScript());
    }
}
