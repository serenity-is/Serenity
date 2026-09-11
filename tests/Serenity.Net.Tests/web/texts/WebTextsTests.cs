using Serenity.Localization;

namespace Serenity.Web;

public class WebTextsTests
{
    private static readonly Type[] AllTextsTypes =
    [
        typeof(CheckTreeEditorTexts),
        typeof(ColumnPickerDialogTexts),
        typeof(DataGridTexts),
        typeof(DateTimeEditorTexts),
        typeof(DialogTexts),
        typeof(EntityDialogTexts),
        typeof(EntityGridTexts),
        typeof(FilterPanelTexts),
        typeof(FormValidationTexts),
        typeof(HtmlContentEditorTexts),
        typeof(PagerTexts),
        typeof(PropertyGridTexts),
        typeof(QuickSearchTexts),
        typeof(SelectEditorTexts)
    ];

    private static string? PrefixOf(Type type)
    {
        return type.GetCustomAttribute<NestedLocalTextsAttribute>()?.Prefix;
    }

    [Theory]
    [InlineData(typeof(CheckTreeEditorTexts), "Controls.CheckTreeEditor.")]
    [InlineData(typeof(ColumnPickerDialogTexts), "Controls.ColumnPickerDialog.")]
    [InlineData(typeof(DataGridTexts), "Controls.DataGrid.")]
    [InlineData(typeof(DateTimeEditorTexts), "Controls.DateTimeEditor.")]
    [InlineData(typeof(DialogTexts), "Dialogs.")]
    [InlineData(typeof(EntityDialogTexts), "Controls.EntityDialog.")]
    [InlineData(typeof(EntityGridTexts), "Controls.EntityGrid.")]
    [InlineData(typeof(FilterPanelTexts), "Controls.FilterPanel.")]
    [InlineData(typeof(FormValidationTexts), "Validation.")]
    [InlineData(typeof(HtmlContentEditorTexts), "Controls.HtmlContentEditor.")]
    [InlineData(typeof(PagerTexts), "Controls.Pager.")]
    [InlineData(typeof(PropertyGridTexts), "Controls.PropertyGrid.")]
    [InlineData(typeof(QuickSearchTexts), "Controls.QuickSearch.")]
    [InlineData(typeof(SelectEditorTexts), "Controls.SelectEditor.")]
    public void Texts_Class_Has_Expected_Prefix(Type type, string expected)
    {
        Assert.Equal(expected, PrefixOf(type));
    }

    [Fact]
    public void All_Texts_Classes_Can_Be_Registered()
    {
        var registry = new MockLocalTextRegistry();

        registry.AddNestedTexts(new MockTypeSource(AllTextsTypes));

        Assert.NotEmpty(registry.AddedList);
    }
}
