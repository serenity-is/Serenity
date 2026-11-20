namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    [ColumnsScript]
    private class RowActionsColumns
    {
        [RowActionsColumn]
        public object? RowActions { get; set; }

        [RowActionsColumn]
        public object? WronglyNamed { get; set; }
    }

    [Fact]
    public void RowActionsColumn_Should_Keep_The_Name_Of_The_Column()
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem { Name = nameof(RowActionsColumns.RowActions) };
        var source = new PropertyInfoSource(typeof(RowActionsColumns)
            .GetProperty(nameof(RowActionsColumns.RowActions)), null);

        processor.Process(source, item);

        Assert.Equal(RowActionsColumnAttribute.ColumnName, item.Name);
    }

    [Fact]
    public void RowActionsColumn_Should_Throw_When_The_Property_Is_Not_Named_As_The_Column()
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var source = new PropertyInfoSource(typeof(RowActionsColumns)
            .GetProperty(nameof(RowActionsColumns.WronglyNamed)), null);

        Assert.Throws<InvalidProgramException>(() => processor.Process(source, item));
    }

    [Fact]
    public void RowActionsColumn_Should_Be_Unbound_And_Fixed_Width()
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var source = new PropertyInfoSource(typeof(RowActionsColumns)
            .GetProperty(nameof(RowActionsColumns.RowActions)), null);

        processor.Process(source, item);

        Assert.Equal(true, item.Unbound);
        Assert.Equal(40, item.Width);
        Assert.Equal(40, item.MinWidth);
        Assert.Equal(40, item.MaxWidth);
        Assert.Equal(false, item.Sortable);
        Assert.Equal(false, item.ShowSelection);
        Assert.Equal(false, item.AllowHide);
    }
}
