namespace Serenity.Data;

public class SpecialFieldsTests
{
    private class TestRowField : RowField<IdNameRow>
    {
        public TestRowField() : base(null, "F")
        {
        }

        public int Compare(IdNameRow? a, IdNameRow? b) => CompareValues(a!, b!);

        public IdNameRow? CloneValue(IdNameRow? value) => Clone(value);
    }

    private class TestRowListField : RowListField<IdNameRow>
    {
        public TestRowListField() : base(null, "F")
        {
        }

        public int Compare(List<IdNameRow> a, List<IdNameRow> b) => CompareValues(a, b);

        public List<IdNameRow>? CloneValue(List<IdNameRow>? value) => Clone(value);
    }

    [Fact]
    public void RowField_CompareValues_Handles_Nulls_And_Order()
    {
        var field = new TestRowField();

        Assert.Equal(0, field.Compare(null, null));
        Assert.Equal(-1, field.Compare(null, new IdNameRow { ID = 1 }));
        Assert.Equal(1, field.Compare(new IdNameRow { ID = 1 }, null));
        Assert.Equal(0, field.Compare(new IdNameRow { ID = 1 }, new IdNameRow { ID = 1 }));
        Assert.NotEqual(0, field.Compare(new IdNameRow { ID = 1 }, new IdNameRow { ID = 2 }));
    }

    [Fact]
    public void RowField_Clone_Clones_Value()
    {
        var field = new TestRowField();
        var row = new IdNameRow { ID = 1, Name = "A" };

        Assert.Null(field.CloneValue(null));
        var clone = field.CloneValue(row);
        Assert.NotNull(clone);
        Assert.NotSame(row, clone);
        Assert.Equal("A", clone.Name);
    }

    [Fact]
    public void RowListField_CompareValues_Handles_Nulls_And_Order()
    {
        var field = new TestRowListField();
        var row1 = new IdNameRow { ID = 1 };
        var row2 = new IdNameRow { ID = 2 };

        Assert.Equal(-1, field.Compare([null!], [row1]));
        Assert.Equal(1, field.Compare([row1], [null!]));
        Assert.Equal(-1, field.Compare([null!, null!], [null!, row1]));
        Assert.Equal(0, field.Compare([row1], [row1]));
        Assert.Equal(-1, field.Compare([row1], [row1, row2]));
        Assert.Equal(1, field.Compare([row1, row2], [row1]));
        Assert.NotEqual(0, field.Compare([row1], [row2]));
    }

    [Fact]
    public void RowListField_Clone_Clones_Items()
    {
        var field = new TestRowListField();
        var list = new List<IdNameRow> { new() { ID = 1 }, new() { ID = 2 } };

        Assert.Null(field.CloneValue(null));
        var clone = field.CloneValue(list);
        Assert.NotNull(clone);
        Assert.Equal(2, clone.Count);
        Assert.NotSame(list[0], clone[0]);
    }
}
