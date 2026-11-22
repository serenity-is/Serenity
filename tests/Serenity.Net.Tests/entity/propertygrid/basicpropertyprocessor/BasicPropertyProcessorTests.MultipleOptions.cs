using Serenity.ComponentModel;

namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    private class MultipleOptionRow
    {
        [EnumEditor(CheckList = true)]
        public List<MultipleOptionEnum>? CheckListValues { get; set; }

        [EnumEditor(Delimited = true)]
        public string? DelimitedValues { get; set; }

        [EnumEditor(CheckList = true, Delimited = true)]
        public string? CheckListDelimitedValues { get; set; }

        [EnumEditor(CheckList = true)]
        public MultipleOptionEnum? SingleCheckListValue { get; set; }

        [EnumEditor(Delimited = true)]
        public MultipleOptionEnum? SingleDelimitedValue { get; set; }

        [EnumEditor(CheckList = true)]
        public MultipleOptionFlags? FlagsCheckListValue { get; set; }

        [EnumEditor(Delimited = true)]
        public MultipleOptionFlags? FlagsDelimitedValue { get; set; }

        [EnumEditor(CheckList = true)]
        public string? StringCheckListValue { get; set; }
    }

    [Flags]
    private enum MultipleOptionFlags
    {
        Read = 1,
        Write = 2,
        ReadWrite = Read | Write
    }

    private enum MultipleOptionEnum
    {
        One = 1,
        Two = 2,
        Three = 3
    }

    private static PropertyItem ProcessEditor<TRow>(string propertyName)
    {
        var processor = new BasicPropertyProcessor();
        var item = new PropertyItem();
        processor.Process(new PropertyInfoSource(typeof(TRow).GetProperty(propertyName), null), item);
        return item;
    }

    [Fact]
    public void CheckListAndDelimited_Imply_Multiple()
    {
        var checkList = ProcessEditor<MultipleOptionRow>(nameof(MultipleOptionRow.CheckListValues));
        Assert.Equal("Enum", checkList.EditorType);
        Assert.Equal(true, checkList.EditorParams["multiple"]);
        Assert.Equal(true, checkList.EditorParams["checkList"]);

        var delimited = ProcessEditor<MultipleOptionRow>(nameof(MultipleOptionRow.DelimitedValues));
        Assert.Equal(true, delimited.EditorParams["multiple"]);
        Assert.Equal(true, delimited.EditorParams["delimited"]);

        // a string property can hold more than one value when the editor delimits them
        var both = ProcessEditor<MultipleOptionRow>(nameof(MultipleOptionRow.CheckListDelimitedValues));
        Assert.Equal(true, both.EditorParams["multiple"]);
        Assert.Equal(true, both.EditorParams["checkList"]);
        Assert.Equal(true, both.EditorParams["delimited"]);
    }

    [Fact]
    public void CheckList_On_A_Single_Value_Property_Throws()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            ProcessEditor<MultipleOptionRow>(nameof(MultipleOptionRow.SingleCheckListValue)));

        Assert.Contains(nameof(MultipleOptionRow.SingleCheckListValue), exception.Message);
        Assert.Contains("checkList", exception.Message);
    }

    [Fact]
    public void Delimited_On_A_Single_Value_Property_Throws()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            ProcessEditor<MultipleOptionRow>(nameof(MultipleOptionRow.SingleDelimitedValue)));

        Assert.Contains(nameof(MultipleOptionRow.SingleDelimitedValue), exception.Message);
        Assert.Contains("delimited", exception.Message);
    }

    [Fact]
    public void CheckList_On_A_Flags_Enum_Property_Is_Multiple()
    {
        // the members selected of a set of flags are OR-ed together, so the property holds more than one of them
        var item = ProcessEditor<MultipleOptionRow>(nameof(MultipleOptionRow.FlagsCheckListValue));
        Assert.Equal(true, item.EditorParams["multiple"]);
        Assert.Equal(true, item.EditorParams["checkList"]);
    }

    [Fact]
    public void Delimited_On_A_Flags_Enum_Property_Throws()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            ProcessEditor<MultipleOptionRow>(nameof(MultipleOptionRow.FlagsDelimitedValue)));

        Assert.Contains("delimited", exception.Message);
    }

    [Fact]
    public void CheckList_On_A_String_Property_Without_Delimited_Throws()
    {
        // a string is a list of characters, but it holds more than one value only along with the delimited option
        var exception = Assert.Throws<InvalidOperationException>(() =>
            ProcessEditor<MultipleOptionRow>(nameof(MultipleOptionRow.StringCheckListValue)));

        Assert.Contains(nameof(MultipleOptionRow.StringCheckListValue), exception.Message);
    }
}
