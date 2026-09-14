namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    private class EditLinkRow : Row<EditLinkRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public int? CityId { get => fields.CityId[this]; set => fields.CityId[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null;
            public StringField Name = null;
            public Int32Field CityId = null;
        }

        public EditLinkRow()
        {
        }

        public EditLinkRow(RowFields fields)
            : base(fields)
        {
        }
    }

    private class EditLinkForm
    {
        [EditLink(true, ItemType = "MyItem", CssClass = "elc")]
        public string? Name { get; set; }

        [EditLink(false)]
        public int? ID { get; set; }
    }

    private static string? AutoDetermineIdField(Field? field)
    {
        var method = typeof(BasicPropertyProcessor).GetMethod("AutoDetermineIdField",
            BindingFlags.NonPublic | BindingFlags.Static)!;
        return (string?)method.Invoke(null, [field]);
    }

    private static EditLinkRow NewEditLinkRow()
    {
        var fields = new EditLinkRow.RowFields();
        fields.Initialize(annotations: null, dialect: SqlSettings.DefaultDialect);
        return new EditLinkRow(fields);
    }

    [Fact]
    public void EditLink_Sets_ItemType_CssClass_And_No_IdField()
    {
        var item = Process<EditLinkForm>(nameof(EditLinkForm.Name), NewEditLinkRow());

        Assert.True(item.EditLink);
        Assert.Equal("MyItem", item.EditLinkItemType);
        Assert.Equal("elc", item.EditLinkCssClass);
        Assert.Null(item.EditLinkIdField);
    }

    [Fact]
    public void EditLink_False_Does_Not_Enable()
    {
        var item = Process<EditLinkForm>(nameof(EditLinkForm.ID), NewEditLinkRow());
        Assert.NotEqual(true, item.EditLink);
    }

    [Fact]
    public void AutoDetermineIdField_Returns_Null_For_Null()
    {
        Assert.Null(AutoDetermineIdField(null));
    }

    [Fact]
    public void AutoDetermineIdField_Returns_Null_Without_Single_ReferencedAlias()
    {
        var fields = NewEditLinkRow().GetFields();
        Assert.Null(AutoDetermineIdField(fields.Name));

        fields.Name.ReferencedAliases = ["T0", "T1"];
        Assert.Null(AutoDetermineIdField(fields.Name));
    }

    [Fact]
    public void AutoDetermineIdField_Finds_Field_By_TextualField()
    {
        var fields = NewEditLinkRow().GetFields();
        var nameField = fields.Name;
        nameField.PropertyName = "Name";
        nameField.ReferencedAliases = ["T0"];

        var cityField = fields.CityId;
        cityField.PropertyName = "CityId";
        cityField.TextualField = "Name";
        cityField.ForeignJoinAlias = new LeftJoin("Cities", "City", new Criteria("1=1"));

        Assert.Equal("CityId", AutoDetermineIdField(nameField));
    }

    [Fact]
    public void AutoDetermineIdField_Finds_Field_By_Join_Name()
    {
        var fields = NewEditLinkRow().GetFields();
        var nameField = fields.Name;
        nameField.PropertyName = "Name";

        var join = new LeftJoin("Cities", "City", new Criteria("1=1"));
        nameField.join = join;

        var cityField = fields.CityId;
        cityField.PropertyName = "CityId";
        cityField.ForeignJoinAlias = join;

        Assert.Equal("CityId", AutoDetermineIdField(nameField));
    }
}
