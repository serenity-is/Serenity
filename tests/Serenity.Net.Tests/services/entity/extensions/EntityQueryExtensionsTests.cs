namespace Serenity.Data;

public class EntityQueryExtensionsTests
{
    [TableName("SelRows")]
    private class SelRow : Row<SelRow.RowFields>, IIdRow, INameRow
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        [NotMapped]
        public string? NotMappedF { get => fields.NotMappedF[this]; set => fields.NotMappedF[this] = value; }

        [SetFieldFlags(FieldFlags.Foreign)]
        public string? ForeignF { get => fields.ForeignF[this]; set => fields.ForeignF[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field Id;
            public StringField Name;
            public StringField NotMappedF;
            public StringField ForeignF;
#pragma warning restore CS0649
        }
    }

    [Fact]
    public void WhereEqual_Throws_For_Null_Row()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().WhereEqual(null!));
    }

    [Fact]
    public void WhereEqual_Throws_When_Not_Tracking_Assignments()
    {
        var row = new SelRow();
        ((IRow)row).TrackAssignments = false;
        Assert.Throws<ArgumentException>(() => new SqlQuery().WhereEqual(row));
    }

    [Fact]
    public void WhereEqual_Adds_Assigned_Fields()
    {
        var row = new SelRow();
        ((IRow)row).TrackAssignments = true;
        row.Name = "x";
        var query = new SqlQuery();

        Assert.Same(query, query.WhereEqual(row));
    }

    [Fact]
    public void Set_Row_Throws_For_Null_And_NonTracking()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlUpdate("T").Set(null!));
        var row = new SelRow();
        ((IRow)row).TrackAssignments = false;
        Assert.Throws<ArgumentException>(() => new SqlUpdate("T").Set(row));
    }

    [Fact]
    public void Set_Row_Adds_Assigned_Fields_And_Excludes()
    {
        var row = new SelRow();
        ((IRow)row).TrackAssignments = true;
        row.Name = "x";
        row.Id = 5;
        var update = new SqlUpdate("T");

        Assert.Same(update, update.Set(row));
        Assert.Same(update, update.Set(row, SelRow.Fields.Name));
    }

    [Fact]
    public void Set_Field_Value_Adds_Parameter()
    {
        var update = new SqlUpdate("T");
        Assert.Same(update, update.Set(SelRow.Fields.Name, "x"));
    }

    [Fact]
    public void SelectTableFields_Throws_For_Nulls()
    {
        Assert.Throws<ArgumentNullException>(() => EntityQueryExtensions.SelectTableFields(null!, new SelRow()));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().SelectTableFields((IRow)null!));
        Assert.Throws<ArgumentNullException>(() => EntityQueryExtensions.SelectTableFields(null!, Array.Empty<Field>()));
    }

    [Fact]
    public void SelectTableFields_Selects_Only_Table_Fields()
    {
        var row = new SelRow();
        var query = new SqlQuery().From(row);

        query.SelectTableFields();
    }

    [Fact]
    public void SelectTableFields_With_Row_And_Exclude()
    {
        var row = new SelRow();
        var query = new SqlQuery().From(row);

        query.SelectTableFields(row, SelRow.Fields.Name);
    }

    [Fact]
    public void SelectForeignFields_Throws_For_Nulls()
    {
        Assert.Throws<ArgumentNullException>(() => EntityQueryExtensions.SelectForeignFields(null!, new SelRow()));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().SelectForeignFields(null!));
    }

    [Fact]
    public void SelectForeignFields_Selects_Foreign_Fields()
    {
        var row = new SelRow();
        var query = new SqlQuery().From(row);

        query.SelectForeignFields(row);
        query.SelectForeignFields(row, SelRow.Fields.ForeignF);
    }

    [Fact]
    public void SelectNonTableFields_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => EntityQueryExtensions.SelectNonTableFields(null!));
    }

    [Fact]
    public void SelectNonTableFields_Selects_Foreign_Fields()
    {
        var row = new SelRow();
        var query = new SqlQuery().From(row);

        query.SelectNonTableFields();
    }

    [Fact]
    public void From_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().From((IEntity)null!));
    }

    [Fact]
    public void From_Row_And_Into()
    {
        var row = new SelRow();
        var query = new SqlQuery();

        Assert.Same(query, query.From(row));
        Assert.Same(query, query.Into(row));
    }

    [Fact]
    public void Select_Field_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((IField)null!));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((IField)null!, "C"));
    }

    [Fact]
    public void Select_Field_And_ColumnName()
    {
        var query = new SqlQuery().From(new SelRow());
        Assert.Same(query, query.Select(SelRow.Fields.Name));
        Assert.Same(query, query.Select(SelRow.Fields.Name, "C"));
    }

    [Fact]
    public void Select_Alias_Field()
    {
        var query = new SqlQuery().From(new SelRow());
        Assert.Same(query, query.Select(Alias.T0, SelRow.Fields.Name));
        Assert.Same(query, query.Select(Alias.T0, SelRow.Fields.Name, "C"));
    }

    [Fact]
    public void Select_Alias_Field_Throws_For_Nulls()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((IAlias)null!, SelRow.Fields.Name));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(Alias.T0, (IField)null!));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(Alias.T0, SelRow.Fields.Name, null!));
    }

    [Fact]
    public void Select_Params_Fields()
    {
        var query = new SqlQuery().From(new SelRow());
        Assert.Same(query, query.Select(SelRow.Fields.Name, SelRow.Fields.Id));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((IField[])null!));
    }

    [Fact]
    public void SelectAs_Adds_Expression()
    {
        var query = new SqlQuery().From(new SelRow());
        Assert.Same(query, query.SelectAs("1", SelRow.Fields.Name));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().SelectAs("", SelRow.Fields.Name));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().SelectAs("1", null!));
    }

    [Fact]
    public void OrderBy_Field_And_Params()
    {
        var query = new SqlQuery().From(new SelRow());
        Assert.Same(query, query.OrderBy(SelRow.Fields.Name));
        Assert.Same(query, query.OrderBy(SelRow.Fields.Name, true));
        Assert.Same(query, query.OrderBy(SelRow.Fields.Name, SelRow.Fields.Id));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy((IField)null!));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy((IField[])null!));
    }

    [Fact]
    public void GroupBy_Field_And_Params()
    {
        var query = new SqlQuery().From(new SelRow());
        Assert.Same(query, query.GroupBy(SelRow.Fields.Name));
        Assert.Same(query, query.GroupBy(SelRow.Fields.Name, SelRow.Fields.Id));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().GroupBy((IField)null!));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().GroupBy((IField[])null!));
    }
}
