namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    private class ValueTypeRow
    {
        public int IntValue { get; set; }
        public short ShortValue { get; set; }
        public long LongValue { get; set; }
        public DateOnly DateOnlyValue { get; set; }
        public DateTime DateTimeValue { get; set; }
        public decimal DecimalValue { get; set; }
    }

    [Theory]
    [InlineData(nameof(ValueTypeRow.IntValue), "Integer", "Number")]
    [InlineData(nameof(ValueTypeRow.ShortValue), "Integer", "Number")]
    [InlineData(nameof(ValueTypeRow.LongValue), "Int64", "BigInt")]
    [InlineData(nameof(ValueTypeRow.DateOnlyValue), "Date", "Date")]
    [InlineData(nameof(ValueTypeRow.DateTimeValue), "Date", "Date")]
    [InlineData(nameof(ValueTypeRow.DecimalValue), "Decimal", "Number")]
    public void EditorAndFormatter_Are_Determined_For_ValueTypes(string propertyName,
        string? expectedEditorType, string? expectedFormatterType)
    {
        var processor = new BasicPropertyProcessor();
        var item = new PropertyItem();
        var source = new PropertyInfoSource(typeof(ValueTypeRow).GetProperty(propertyName), null);

        processor.Process(source, item);

        Assert.Equal(expectedEditorType, item.EditorType);
        Assert.Equal(expectedFormatterType, item.FormatterType);
    }

    [Fact]
    public void ShortValue_Uses_Integer_Editor_With_Short_Max_Value()
    {
        var processor = new BasicPropertyProcessor();
        var item = new PropertyItem();
        var source = new PropertyInfoSource(
            typeof(ValueTypeRow).GetProperty(nameof(ValueTypeRow.ShortValue)), null);

        processor.Process(source, item);

        Assert.Equal("Integer", item.EditorType);
        Assert.Equal(short.MaxValue, Convert.ToInt32(item.EditorParams["maxValue"]));
    }

    [Fact]
    public void LongValue_Uses_Int64_Editor_And_BigInt_Formatter()
    {
        var processor = new BasicPropertyProcessor();
        var item = new PropertyItem();
        var source = new PropertyInfoSource(
            typeof(ValueTypeRow).GetProperty(nameof(ValueTypeRow.LongValue)), null);

        processor.Process(source, item);

        Assert.Equal("Int64", item.EditorType);
        Assert.Equal("BigInt", item.FormatterType);
    }
}
