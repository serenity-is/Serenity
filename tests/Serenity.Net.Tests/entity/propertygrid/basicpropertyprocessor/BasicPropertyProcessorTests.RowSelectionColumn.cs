namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    [ColumnsScript]
    private class RowSelectionColumns
    {
        [RowSelectionColumn]
        public object? RowSelection { get; set; }

        [RowSelectionColumn]
        public object? WronglyNamed { get; set; }

        public string? Normal { get; set; }
    }

    [Fact]
    public void RowSelectionColumn_Should_Keep_The_Name_Of_The_Column()
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem { Name = nameof(RowSelectionColumns.RowSelection) };
        var source = new PropertyInfoSource(typeof(RowSelectionColumns)
            .GetProperty(nameof(RowSelectionColumns.RowSelection)), null);

        processor.Process(source, item);

        Assert.Equal(RowSelectionColumnAttribute.ColumnName, item.Name);
        Assert.Equal("RowSelection", item.Name);
    }

    [Fact]
    public void RowSelectionColumn_Should_Throw_When_The_Property_Is_Not_Named_As_The_Column()
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var source = new PropertyInfoSource(typeof(RowSelectionColumns)
            .GetProperty(nameof(RowSelectionColumns.WronglyNamed)), null);

        Assert.Throws<InvalidProgramException>(() => processor.Process(source, item));
    }

    [Fact]
    public void RowSelectionColumn_Should_Be_Unbound_And_Fixed_Width()
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var source = new PropertyInfoSource(typeof(RowSelectionColumns)
            .GetProperty(nameof(RowSelectionColumns.RowSelection)), null);

        processor.Process(source, item);

        Assert.Equal(true, item.Unbound);
        Assert.Equal(27, item.Width);
        Assert.Equal(27, item.MinWidth);
        Assert.Equal(27, item.MaxWidth);
        Assert.Equal(false, item.Sortable);
        Assert.Equal(false, item.Focusable);
        Assert.Equal(false, item.ShowSelection);
        Assert.Equal(false, item.AllowHide);
    }
}
