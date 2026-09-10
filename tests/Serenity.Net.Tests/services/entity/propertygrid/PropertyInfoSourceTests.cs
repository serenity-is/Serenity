namespace Serenity.PropertyGrid;

public class PropertyInfoSourceTests
{
    public enum MyEnum
    {
        A,
        B
    }

    [TableName("SrcRows")]
    private class SourceRow : Row<SourceRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        public MyEnum? EnumProp { get => fields.EnumProp[this]; set => fields.EnumProp[this] = value; }

        [Hint("FromRow")]
        public int? EnumMapped
        {
            get => (int?)fields.EnumMapped[this];
            set => fields.EnumMapped[this] = (MyEnum?)value;
        }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public EnumField<MyEnum> EnumProp;
            public EnumField<MyEnum> EnumMapped;
#pragma warning restore CS0649
        }
    }

    private class SourceForm
    {
        [DisplayName("N")]
        public int? EnumMapped { get; set; }

        public MyEnum? EnumProp { get; set; }
    }

    [Fact]
    public void Throws_ForNullProperty()
    {
        Assert.Throws<ArgumentNullException>(() => new PropertyInfoSource(null!, null));
    }

    [Fact]
    public void ValueType_UnwrapsNullable()
    {
        var source = new PropertyInfoSource(typeof(SourceForm).GetProperty(nameof(SourceForm.EnumMapped)), null);
        Assert.Equal(typeof(int), source.ValueType);
        Assert.Null(source.EnumType);
        Assert.Null(source.BasedOnRow);
        Assert.Null(source.BasedOnField);
    }

    [Fact]
    public void EnumType_ComesFromProperty()
    {
        var source = new PropertyInfoSource(typeof(SourceForm).GetProperty(nameof(SourceForm.EnumProp)), null);
        Assert.Equal(typeof(MyEnum), source.EnumType);
    }

    [Fact]
    public void EnumType_ComesFromBasedOnField()
    {
        var source = new PropertyInfoSource(typeof(SourceForm).GetProperty(nameof(SourceForm.EnumMapped)),
            new SourceRow());
        Assert.Equal(typeof(MyEnum), source.EnumType);
    }

    [Fact]
    public void EnumType_IsNull_WhenNotAnEnum()
    {
        var field = SourceRow.Fields.EnumMapped;
        var original = field.EnumType;
        try
        {
            field.EnumType = typeof(int);
            var source = new PropertyInfoSource(typeof(SourceForm).GetProperty(nameof(SourceForm.EnumMapped)),
                new SourceRow());
            Assert.Null(source.EnumType);
        }
        finally
        {
            field.EnumType = original;
        }
    }

    [Fact]
    public void NameAndReflectedType_AreExposed()
    {
        var property = typeof(SourceForm).GetProperty(nameof(SourceForm.EnumMapped));
        var source = new PropertyInfoSource(property, null);
        Assert.Equal("EnumMapped", source.Name);
        Assert.Equal(property.PropertyType, source.PropertyType);
        Assert.Equal(property.ReflectedType, source.ReflectedType);
        Assert.Same(property, source.Property);
    }

    [Fact]
    public void GetAttribute_FallsBackToBasedOnField()
    {
        var source = new PropertyInfoSource(typeof(SourceForm).GetProperty(nameof(SourceForm.EnumMapped)),
            new SourceRow());

        Assert.Null(source.GetAttribute<HintAttribute>(AttributeOrigin.Explicit));
        Assert.NotNull(source.GetAttribute<HintAttribute>());
        Assert.Equal("FromRow", source.GetAttribute<HintAttribute>()!.Hint);
    }

    [Fact]
    public void GetAttribute_PrefersProperty()
    {
        var source = new PropertyInfoSource(typeof(SourceForm).GetProperty(nameof(SourceForm.EnumMapped)), null);
        Assert.Equal("N", source.GetAttribute<DisplayNameAttribute>()!.DisplayName);
    }

    [Fact]
    public void GetAttributes_IncludesBasedOnField()
    {
        var source = new PropertyInfoSource(typeof(SourceForm).GetProperty(nameof(SourceForm.EnumMapped)),
            new SourceRow());

        var hints = source.GetAttributes<HintAttribute>().ToList();
        Assert.Contains(hints, x => x.Hint == "FromRow");
    }
}

