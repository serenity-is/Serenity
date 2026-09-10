namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    [LocalTextPrefix("MoreBased")]
    private class MoreBasedRow : Row<MoreBasedRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotNull, Size(40)]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        [DisplayName("RowTitle")]
        public string? Titled { get => fields.Titled[this]; set => fields.Titled[this] = value; }

        [DefaultValue("dv")]
        public string? Defaulted { get => fields.Defaulted[this]; set => fields.Defaulted[this] = value; }

        [ReadPermission("ReadP"), InsertPermission("InsertP"), UpdatePermission("UpdateP")]
        public string? Perms { get => fields.Perms[this]; set => fields.Perms[this] = value; }

        [Insertable(false), Updatable(false)]
        public string? Locked { get => fields.Locked[this]; set => fields.Locked[this] = value; }

        [NotMapped]
        public string? Computed { get => fields.Computed[this]; set => fields.Computed[this] = value; }

        public int? IntField { get => fields.IntField[this]; set => fields.IntField[this] = value; }
        public long? LongField { get => fields.LongField[this]; set => fields.LongField[this] = value; }

        [Size(10), Scale(2)]
        public decimal? DecimalField { get => fields.DecimalField[this]; set => fields.DecimalField[this] = value; }

        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? DateTimeField { get => fields.DateTimeField[this]; set => fields.DateTimeField[this] = value; }

        [DistinctValuesEditor]
        public string? DistinctField { get => fields.DistinctField[this]; set => fields.DistinctField[this] = value; }

        [DistinctValuesEditor(typeof(MoreServiceRow), "Name")]
        public string? DistinctRtField { get => fields.DistinctRtField[this]; set => fields.DistinctRtField[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public StringField Name;
            public StringField Titled;
            public StringField Defaulted;
            public StringField Perms;
            public StringField Locked;
            public StringField Computed;
            public Int32Field IntField;
            public Int64Field LongField;
            public DecimalField DecimalField;
            public DateTimeField DateTimeField;
            public StringField DistinctField;
            public StringField DistinctRtField;
#pragma warning restore CS0649
        }
    }

    [LocalTextPrefix("MoreService")]
    private class MoreServiceRow : Row<MoreServiceRow.RowFields>, IIdRow, INameRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public StringField Name;
#pragma warning restore CS0649
        }
    }

    private class MoreBasedForm
    {
        public string? Name { get; set; }
        public string? Titled { get; set; }
        public string? Defaulted { get; set; }
        public string? Perms { get; set; }
        public string? Locked { get; set; }
        public string? Computed { get; set; }
        public int? IntField { get; set; }

        [EditorType("Integer")]
        public long? LongField { get; set; }

        public decimal? DecimalField { get; set; }
        public DateTime? DateTimeField { get; set; }

        [EditorType("Lookup")]
        public string? DistinctField { get; set; }

        [EditorType("Lookup")]
        public string? DistinctRtField { get; set; }
    }

    [Fact]
    public void Title_Uses_Field_Caption_When_Form_Has_No_DisplayName()
    {
        var item = Process<MoreBasedForm>(nameof(MoreBasedForm.Titled), new MoreBasedRow());
        Assert.NotEqual(nameof(MoreBasedForm.Titled), item.Title);
    }

    [Fact]
    public void DefaultValue_Comes_From_Field()
    {
        var item = Process<MoreBasedForm>(nameof(MoreBasedForm.Defaulted), new MoreBasedRow());
        Assert.Equal("dv", item.DefaultValue);
    }

    [Fact]
    public void Permissions_Come_From_Field()
    {
        var item = Process<MoreBasedForm>(nameof(MoreBasedForm.Perms), new MoreBasedRow());

        Assert.Equal("InsertP", item.InsertPermission);
        Assert.Equal("ReadP", item.ReadPermission);
        Assert.Equal("UpdateP", item.UpdatePermission);
    }

    [Fact]
    public void DateTimeKind_From_Field_Sets_UseUtc()
    {
        var item = Process<MoreBasedForm>(nameof(MoreBasedForm.DateTimeField), new MoreBasedRow());
        Assert.Equal(true, item.EditorParams["useUtc"]);
    }

    [Fact]
    public void DateTime_Field_Without_DateOnly_Uses_DateTime_Filtering()
    {
        var item = Process<MoreBasedForm>(nameof(MoreBasedForm.DateTimeField), new MoreBasedRow());
        Assert.Equal("DateTime", item.FilteringType);
        Assert.Equal("DateTime", item.FormatterType);
    }

    [Fact]
    public void Decimal_Field_Sets_Min_And_Max_Value()
    {
        var item = Process<MoreBasedForm>(nameof(MoreBasedForm.DecimalField), new MoreBasedRow());

        Assert.Equal("Decimal", item.EditorType);
        Assert.NotNull(item.EditorParams["minValue"]);
        Assert.NotNull(item.EditorParams["maxValue"]);
    }

    [Fact]
    public void Int64_Field_Sets_Max_Value()
    {
        var item = Process<MoreBasedForm>(nameof(MoreBasedForm.LongField), new MoreBasedRow());
        Assert.Equal(long.MaxValue, item.EditorParams["maxValue"]);
    }

    [Fact]
    public void DistinctValues_From_Field_Sets_LookupKey()
    {
        var item = Process<MoreBasedForm>(nameof(MoreBasedForm.DistinctField), new MoreBasedRow());
        Assert.Equal("Distinct.MoreBased.DistinctField", item.EditorParams["lookupKey"]);
    }

    [Fact]
    public void DistinctValues_With_RowType_Sets_LookupKey()
    {
        var item = Process<MoreBasedForm>(nameof(MoreBasedForm.DistinctRtField), new MoreBasedRow());
        Assert.Equal("Distinct.MoreService.Name", item.EditorParams["lookupKey"]);
    }

    private class LookupFilterForm
    {
        [LookupEditor("SomeLookup")]
        public string? LookupProp { get; set; }

        [FilteringType("Editor")]
        [EditorType("MyEditor")]
        public string? EditorFilter { get; set; }

        [FilteringType("Editor")]
        public int? EditorFilterNoEditor { get; set; }
    }

    [Fact]
    public void Lookup_Editor_Uses_Lookup_Filtering()
    {
        var item = Process<LookupFilterForm>(nameof(LookupFilterForm.LookupProp));

        Assert.Equal("Lookup", item.FilteringType);
        Assert.Equal("SomeLookup", item.FilteringParams["lookupKey"]);
    }

    [Fact]
    public void FilteringType_Editor_Uses_EditorType_And_UseLike()
    {
        var item = Process<LookupFilterForm>(nameof(LookupFilterForm.EditorFilter));

        Assert.Equal("Editor", item.FilteringType);
        Assert.Equal("MyEditor", item.FilteringParams["editorType"]);
        Assert.Equal(true, item.FilteringParams["useLike"]);
    }

    [Fact]
    public void FilteringType_Editor_Without_Editor_Does_Not_Set_EditorType()
    {
        var item = Process<LookupFilterForm>(nameof(LookupFilterForm.EditorFilterNoEditor));

        Assert.Equal("Editor", item.FilteringType);
        Assert.False(item.FilteringParams.ContainsKey("editorType"));
        Assert.False(item.FilteringParams.ContainsKey("useLike"));
    }

    private class WidthRow : Row<WidthRow.RowFields>
    {
        [Size(20)]
        public string? StringSmall { get => fields.StringSmall[this]; set => fields.StringSmall[this] = value; }
        public string? StringNoSize { get => fields.StringNoSize[this]; set => fields.StringNoSize[this] = value; }

        [Size(40)]
        public string? StringLarge { get => fields.StringLarge[this]; set => fields.StringLarge[this] = value; }
        public bool? BoolField { get => fields.BoolField[this]; set => fields.BoolField[this] = value; }
        public DateTime? DateField { get => fields.DateField[this]; set => fields.DateField[this] = value; }
        public DateOnly? DateOnlyField { get => fields.DateOnlyField[this]; set => fields.DateOnlyField[this] = value; }
        public TimeSpan? TimeField { get => fields.TimeField[this]; set => fields.TimeField[this] = value; }
        public short? Int16Field { get => fields.Int16Field[this]; set => fields.Int16Field[this] = value; }
        public int? Int32Field { get => fields.Int32Field[this]; set => fields.Int32Field[this] = value; }
        public float? SingleField { get => fields.SingleField[this]; set => fields.SingleField[this] = value; }
        public double? DoubleField { get => fields.DoubleField[this]; set => fields.DoubleField[this] = value; }
        public decimal? DecimalField { get => fields.DecimalField[this]; set => fields.DecimalField[this] = value; }
        public Guid? GuidField { get => fields.GuidField[this]; set => fields.GuidField[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public StringField StringSmall;
            public StringField StringNoSize;
            public StringField StringLarge;
            public BooleanField BoolField;
            public DateTimeField DateField;
            public DateOnlyField DateOnlyField;
            public TimeSpanField TimeField;
            public Int16Field Int16Field;
            public Int32Field Int32Field;
            public SingleField SingleField;
            public DoubleField DoubleField;
            public DecimalField DecimalField;
            public GuidField GuidField;
#pragma warning restore CS0649
        }
    }

    private class WidthForm
    {
        public string? StringSmall { get; set; }
        public string? StringNoSize { get; set; }
        public string? StringLarge { get; set; }
        public bool? BoolField { get; set; }
        public DateTime? DateField { get; set; }
        public DateOnly? DateOnlyField { get; set; }
        public TimeSpan? TimeField { get; set; }
        public short? Int16Field { get; set; }
        public int? Int32Field { get; set; }
        public float? SingleField { get; set; }
        public double? DoubleField { get; set; }
        public decimal? DecimalField { get; set; }
        public Guid? GuidField { get; set; }
    }

    [Theory]
    [InlineData(nameof(WidthForm.StringSmall), 150)]
    [InlineData(nameof(WidthForm.StringNoSize), 250)]
    [InlineData(nameof(WidthForm.StringLarge), 150)]
    [InlineData(nameof(WidthForm.BoolField), 40)]
    [InlineData(nameof(WidthForm.DateField), 85)]
    [InlineData(nameof(WidthForm.DateOnlyField), 85)]
    [InlineData(nameof(WidthForm.TimeField), 70)]
    [InlineData(nameof(WidthForm.Int16Field), 55)]
    [InlineData(nameof(WidthForm.Int32Field), 65)]
    [InlineData(nameof(WidthForm.SingleField), 85)]
    [InlineData(nameof(WidthForm.DoubleField), 85)]
    [InlineData(nameof(WidthForm.DecimalField), 85)]
    [InlineData(nameof(WidthForm.GuidField), 80)]
    public void AutoWidth_Uses_Field_Type(string propName, int expected)
    {
        var item = Process<WidthForm>(propName, new WidthRow());
        Assert.Equal(expected, item.Width);
    }

    private class LabelWidthForm
    {
        [LabelWidth("300px")]
        public string? A { get; set; }

        public string? B { get; set; }
    }

    [Fact]
    public void LabelWidth_Without_UntilNext_Does_Not_Affect_Next()
    {
        var processor = new BasicPropertyProcessor();
        var a = Process<LabelWidthForm>(nameof(LabelWidthForm.A), processor: processor);
        var b = Process<LabelWidthForm>(nameof(LabelWidthForm.B), processor: processor);

        Assert.Equal("300px", a.LabelWidth);
        Assert.Null(b.LabelWidth);
    }

    private class FormWidthDefaultForm
    {
        [FormWidth("fd")]
        public string? A { get; set; }

        public string? B { get; set; }
    }

    [Fact]
    public void FormWidth_Without_UntilNext_Does_Not_Affect_Next()
    {
        var processor = new BasicPropertyProcessor();
        var a = Process<FormWidthDefaultForm>(nameof(FormWidthDefaultForm.A), processor: processor);
        var b = Process<FormWidthDefaultForm>(nameof(FormWidthDefaultForm.B), processor: processor);

        Assert.Equal("fd", a.FormCssClass);
        Assert.Null(b.FormCssClass);
    }
}
