using Serenity.ComponentModel;

namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    [FormScript("SelectItemsTestForm")]
    private class SelectItemFormRow
    {
        [SelectItem("low", Text = "Low")]
        [SelectItem("high", Text = "High", TextKey = "Controls.Priority.High")]
        [SelectItem("acme", Text = "Acme Corporation", TextKey = LocalText.Literal)]
        [SelectItem("exotic", TextKey = "Controls.Supplier.Exotic")]
        public string? Priority { get; set; }
    }

    private class SelectItemRow
    {
        [DisplayText("Priority"), SelectItem("low", Text = "Low", Order = 2), SelectItem("high", Text = "High", Order = 1)]
        public string? Priority { get; set; }

        [SelectItem("low", TextKey = "Controls.Priority.Low"), SelectItem("high", TextKey = "Controls.Priority.High")]
        public string? LocalizedPriorities { get; set; }

        [SelectItem("10"), SelectItem("20", Icon = "fa fa-truck", CssClass = "text-danger")]
        public int? PageSize { get; set; }

        [SelectEditor, SelectItem("one")]
        public string? WithEditor { get; set; }

        [EnumEditor(typeof(MultipleOptionEnum)), SelectItem("one")]
        public string? WithAnotherEditor { get; set; }

        [SelectItem("one"), SelectItem("one", Text = "Another")]
        public string? WithDuplicateItems { get; set; }
    }

    [LocalTextPrefix("SelectItemBase")]
    private class SelectItemBaseRow : Row<SelectItemBaseRow.RowFields>
    {
        [SelectItem("low", Text = "Low")]
        public string? Priority { get; set; }

        public class RowFields : RowFieldsBase
        {
            public StringField Priority = null!;
        }
    }

    [FormScript("SelectItemsBasedForm"), BasedOnRow(typeof(SelectItemBaseRow))]
    private class SelectItemBasedForm
    {
        public string? Priority { get; set; }
    }

    [Fact]
    public void SelectItems_Of_The_Row_Field_Take_No_Implied_Key()
    {
        var processor = new BasicPropertyProcessor();
        var item = new PropertyItem();
        processor.Process(new PropertyInfoSource(typeof(SelectItemBasedForm).GetProperty(nameof(SelectItemBasedForm.Priority))!,
            new SelectItemBaseRow()), item);

        // the key that the form would imply is only registered for the items of the form itself, so an item of the row
        // field it is based on is shown as it is
        Assert.Equal("Select", item.EditorType);
        var selectItem = Assert.Single(Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(item.EditorParams["items"]));
        Assert.Equal("Low", selectItem["text"]);
        Assert.False(selectItem.ContainsKey("textKey"));
        Assert.False(item.EditorParams.ContainsKey("textKeyField"));
    }

    [Fact]
    public void SelectItems_Imply_The_Select_Editor()
    {
        var item = ProcessEditor<SelectItemRow>(nameof(SelectItemRow.Priority));

        // the items imply the editor, and the text of an item is passed as it is, without a fallback to its value
        Assert.Equal("Select", item.EditorType);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(item.EditorParams["items"]);
        Assert.Collection(items,
            x =>
            {
                Assert.Equal("high", x["id"]);
                Assert.Equal("High", x["text"]);
            },
            x =>
            {
                Assert.Equal("low", x["id"]);
                Assert.Equal("Low", x["text"]);
            });

        // the items have no text keys, so the editor shows their texts as they are
        Assert.False(item.EditorParams.ContainsKey("textKeyField"));
    }

    [Fact]
    public void SelectItems_Of_A_Form_Take_The_Text_Key_That_It_Implies()
    {
        var item = ProcessEditor<SelectItemFormRow>(nameof(SelectItemFormRow.Priority));

        Assert.Equal("textKey", item.EditorParams["textKeyField"]);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(item.EditorParams["items"]).ToList();
        Assert.Equal(["low", "high", "acme", "exotic"], items.Select(x => x["id"]));
        // an item with a text, and no key of its own, takes the key that its form implies for it, which is registered
        // with the text of the item as its default, an item with a key of its own keeps it, even without a text, and an
        // item with an empty key has none, as it is not localized, e.g. one whose text is a name
        Assert.Equal(["Forms.SelectItemsTestForm.Priority_SelectItem_low", "Controls.Priority.High", null,
            "Controls.Supplier.Exotic"], items.Select(x => x.GetValueOrDefault("textKey")));
        Assert.Equal("Acme Corporation", items[2]["text"]);
    }

    [Fact]
    public void SelectItems_Pass_Their_Text_Keys_And_Let_The_Editor_Fall_Back()
    {
        var item = ProcessEditor<SelectItemRow>(nameof(SelectItemRow.LocalizedPriorities));

        // a property whose items have text keys tells the editor where they are, so that it localizes them
        Assert.Equal("textKey", item.EditorParams["textKeyField"]);
        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(item.EditorParams["items"]).ToList();
        Assert.Equal(["low", "high"], items.Select(x => x["id"]));
        Assert.Equal(["Controls.Priority.Low", "Controls.Priority.High"], items.Select(x => x["textKey"]));
        // no text is passed for an item that has only a key, as what it falls back to is the decision of the editor
        Assert.All(items, x => Assert.False(x.ContainsKey("text")));
    }

    [Fact]
    public void SelectItems_Keep_Only_What_The_Item_Gives()
    {
        var item = ProcessEditor<SelectItemRow>(nameof(SelectItemRow.PageSize));

        // a number property gets a select editor too, without the options of the editor of its type
        Assert.Equal("Select", item.EditorType);
        Assert.False(item.EditorParams.ContainsKey("maxValue"));
        Assert.False(item.EditorParams.ContainsKey("allowNegatives"));

        var items = Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(item.EditorParams["items"]).ToList();
        Assert.Equal(["10", "20"], items.Select(x => x["id"]));
        // an item without a text, and without a text key, shows its value
        Assert.Equal(["10", "20"], items.Select(x => x["text"]));
        Assert.Equal("fa fa-truck", items[1]["icon"]);
        Assert.Equal("text-danger", items[1]["cssClass"]);
    }

    [Fact]
    public void SelectItems_Work_With_An_Explicit_Select_Editor()
    {
        var item = ProcessEditor<SelectItemRow>(nameof(SelectItemRow.WithEditor));
        Assert.Equal("Select", item.EditorType);
        Assert.Single(Assert.IsAssignableFrom<IEnumerable<Dictionary<string, object?>>>(item.EditorParams["items"]));
    }

    [Fact]
    public void SelectItems_On_A_Property_With_Another_Editor_Throw()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            ProcessEditor<SelectItemRow>(nameof(SelectItemRow.WithAnotherEditor)));

        Assert.Contains(nameof(SelectItemRow.WithAnotherEditor), exception.Message);
        Assert.Contains("Enum", exception.Message);
    }

    [Fact]
    public void SelectItems_With_The_Same_Value_Throw()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            ProcessEditor<SelectItemRow>(nameof(SelectItemRow.WithDuplicateItems)));

        Assert.Contains(nameof(SelectItemRow.WithDuplicateItems), exception.Message);
        Assert.Contains("'one'", exception.Message);
    }
}
