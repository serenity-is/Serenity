using Microsoft.Extensions.Options;

namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    public enum MyEnum
    {
        A,
        B
    }

    private class DecoratedForm
    {
        [AlignRight, AllowHide(false), Collapsible(true, Collapsed = true), Focusable(false),
         Visible(false), ShowSelection(false), Tabbable(false), ReadOnly(true), Required,
         HideOnInsert, HideOnUpdate, Unbound, SkipOnLoad, SkipOnSave, PinColumn(true),
         GroupOrder(3), SortOrder(2), Sortable(false), SummaryType(SummaryType.Sum),
         CssClass("cls"), HeaderCssClass("hdr"), EditorCssClass("ec"),
         FormCssClass("fcc", UntilNext = true), LabelWidth(150), Width(123, Min = 60, Max = 300),
         MaxLength(50)]
        public string? AllFlags { get; set; }

        [Category("MyCat"), Tab("MyTab"), DisplayName("MyTitle"), Hint("MyHint"), Placeholder("MyPh")]
        public string? Texts { get; set; }

        [DefaultValue(42)]
        public int? Defaulted { get; set; }

        [EditLink(true, ItemType = "MyItem", IdField = "MyId", CssClass = "elc")]
        public int? Linked { get; set; }

        [EditLink(false)]
        public int? NotLinked { get; set; }

        [EditorType("MyEditor"), EditorOption("someKey", "someValue")]
        public string? Edited { get; set; }

        [EditorAddon("my.addon")]
        public string? WithAddon { get; set; }

        [MaxLength(80)]
        public string? Capped { get; set; }

        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? UtcDate { get; set; }

        [NotFilterable]
        public string? NotFilter { get; set; }

        [FilterOnly]
        public string? FilterOnlyProp { get; set; }

        [QuickFilter(Separator = true, CssClass = "qfc")]
        public string? QuickFiltered { get; set; }

        [FilteringIdField("SomeId")]
        public int? FilterById { get; set; }

        [FilteringType("MyFiltering"), FilteringOption("someFilterKey", "someFilterValue")]
        public string? CustomFiltered { get; set; }

        [QuickFilterOption("qfKey", "qfValue")]
        public int? QuickFilterOptionProp { get; set; }

        [DisplayFormat("MyFormat")]
        public string? Formatted { get; set; }

        [FormatterType("MyFormatter"), FormatterOption("someFmtKey", "someFmtValue")]
        public string? CustomFormatted { get; set; }

        [InsertPermission("InsertP"), ReadPermission("ReadP"), UpdatePermission("UpdateP")]
        public string? Permissions { get; set; }

        [Insertable(false), Updatable(false)]
        public string? NotInsertable { get; set; }

        [Resizable(false)]
        public string? NotResizable { get; set; }

        [FixedWidth(100)]
        public string? FixedW { get; set; }

        [FormWidth("fw", JustThis = true)]
        public string? FormWidthJustThis { get; set; }

        [FormWidth("fwUntilNext", UntilNext = true)]
        public string? FormWidthUntilNext { get; set; }

        public string? AfterFormWidth { get; set; }

        [LabelWidth(220, JustThis = true)]
        public string? LabelWidthJustThis { get; set; }

        public int? IntProp { get; set; }
        public short? ShortProp { get; set; }
        public long? LongProp { get; set; }
        public bool? BoolProp { get; set; }
        public decimal? DecimalProp { get; set; }
        public double? DoubleProp { get; set; }
        public float? FloatProp { get; set; }
        public DateTime? DateProp { get; set; }
        public TimeSpan? TimeProp { get; set; }
        public MyEnum? EnumProp { get; set; }

        [PrimaryKey]
        public int? PrimaryKeyProp { get; set; }

        public string? Plain { get; set; }
    }

    [ConnectionKey("BasicPropertyProcessor")]
    [TableName("BpServiceRows")]
    private class ServiceRow : Row<ServiceRow.RowFields>, IIdRow, INameRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NameProperty, LookupInclude]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
            public StringField Name = null!;
        }
    }

    [ConnectionKey("BasicPropertyProcessor")]
    [TableName("BpServiceRowsWithoutLookupFields")]
    private class ServiceRowWithoutLookupFields : Row<ServiceRowWithoutLookupFields.RowFields>
    {
        public class RowFields : RowFieldsBase;
    }

    private class ServiceLookupForm
    {
        [ServiceLookupEditor(typeof(ServiceRow))]
        public int? Item { get; set; }
    }

    private class ServiceLookupWithExplicitFieldsForm
    {
        [ServiceLookupEditor(typeof(ServiceRow), IdField = "ID", TextField = "Name")]
        public int? Item { get; set; }
    }

    private class ServiceLookupWithoutRowFieldsForm
    {
        [ServiceLookupEditor(typeof(ServiceRowWithoutLookupFields))]
        public int? Item { get; set; }
    }

    private class ServiceLookupWithLookupColumnSelectionForm
    {
        [ServiceLookupEditor(typeof(ServiceRow), IdField = "ID", TextField = "Name",
            ColumnSelection = ColumnSelection.Lookup)]
        public int? Item { get; set; }
    }

    private class ConnectorTypeOption
    {
        public string Code { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public int SortOrder { get; set; }
    }

    private class QuickSearchLookupOption
    {
        public string Code { get; set; } = null!;
        public string DisplayName { get; set; } = null!;

        [Serenity.Data.Mapping.QuickSearch]
        public string Email { get; set; } = null!;
    }

    [Route("Services/BasicPropertyProcessor/QuickSearchLookup/[action]")]
    private class QuickSearchLookupEndpoint
    {
        [ListRequestCapabilities(Serenity.Services.ListRequestCapabilities.Baseline,
            Exclude = Serenity.Services.ListRequestCapabilities.ContainsText)]
        public ListResponse<QuickSearchLookupOption> List(ListRequest request)
        {
            return new();
        }
    }

    private class QuickSearchLookupAttribute : ServiceLookupEditorBaseAttribute
    {
        public QuickSearchLookupAttribute() : base(ServiceLookupEditorAttribute.Key)
        {
            ItemType = typeof(QuickSearchLookupOption);
            EndpointType = typeof(QuickSearchLookupEndpoint);
        }
    }

    private class QuickSearchLookupForm
    {
        [QuickSearchLookup]
        public string? Item { get; set; }
    }

    [Route("Services/BasicPropertyProcessor/PrimitiveLookup/[action]")]
    private class PrimitiveLookupEndpoint
    {
        public Task<List<string>> GetNames()
        {
            return Task.FromResult(new List<string>());
        }

        public ListResponse<int> GetNumbers()
        {
            return new();
        }
    }

    private class PrimitiveListLookupForm
    {
        [ServiceLookupEditor(typeof(string), EndpointType = typeof(PrimitiveLookupEndpoint),
            ActionName = "GetNames")]
        public string? Item { get; set; }
    }

    private class PrimitiveListResponseLookupForm
    {
        [ServiceLookupEditor(typeof(int), EndpointType = typeof(PrimitiveLookupEndpoint),
            ActionName = "GetNumbers")]
        public int? Item { get; set; }
    }

    [Route("Services/ProDataSources/ConnectorTypes/[action]")]
    private class ConnectorTypeEndpoint
    {
        public void GetTypes()
        {
        }
    }

    private class ConnectorTypeLookupForm
    {
        [ServiceLookupEditor(typeof(ConnectorTypeOption),
            EndpointType = typeof(ConnectorTypeEndpoint), ActionName = "GetTypes")]
        public string? ConnectorType { get; set; }
    }

    private class ConnectorTypeLookupWithNullIdFieldForm
    {
        [ServiceLookupEditor(typeof(ConnectorTypeOption), IdField = null)]
        public string? ConnectorType { get; set; }
    }

    private class ConnectorTypeLookupWithNullTextFieldForm
    {
        [ServiceLookupEditor(typeof(ConnectorTypeOption), TextField = null)]
        public string? ConnectorType { get; set; }
    }

    private class SingleTextOption
    {
        public string Value { get; set; } = null!;
    }

    private class SingleTextLookupForm
    {
        [ServiceLookupEditor(typeof(SingleTextOption))]
        public string? Value { get; set; }
    }

    private class AmbiguousLookupOption
    {
        public string Description { get; set; } = null!;
        public int SortOrder { get; set; }
    }

    private class AmbiguousLookupForm
    {
        [ServiceLookupEditor(typeof(AmbiguousLookupOption))]
        public int? Value { get; set; }
    }

    [ConnectionKey(typeof(ServiceRow))]
    [Route("Services/BasicPropertyProcessor/ServiceRow/[action]")]
    private class ServiceRowEndpoint
    {
        public void List()
        {
        }
    }

    [Route("Services/BasicPropertyProcessor/RestrictedServiceRow/[action]")]
    private class RestrictedServiceRowEndpoint
    {
        [ListRequestCapabilities(Serenity.Services.ListRequestCapabilities.Skip |
            Serenity.Services.ListRequestCapabilities.Take)]
        public ListResponse<ServiceRow> List(ListRequest request)
        {
            return new();
        }

        [ListRequestCapabilities(Serenity.Services.ListRequestCapabilities.Baseline,
            Exclude = Serenity.Services.ListRequestCapabilities.ContainsText |
                Serenity.Services.ListRequestCapabilities.Criteria)]
        public ListResponse<ServiceRow> ListWithoutTextAndCriteria(ListRequest request)
        {
            return new();
        }
    }

    private class LimitedListRequest : ServiceRequest
    {
        public int Skip { get; set; }
        public string? ContainsText { get; set; }
        public string? CustomValue { get; set; }
    }

    [Route("Services/BasicPropertyProcessor/CustomServiceRow/[action]")]
    private class CustomServiceRowEndpoint
    {
        public ListResponse<ServiceRow> Search(LimitedListRequest request)
        {
            return new();
        }
    }

    private class EndpointOnlyServiceLookupAttribute : ServiceLookupEditorBaseAttribute
    {
        public EndpointOnlyServiceLookupAttribute() : base(ServiceLookupEditorAttribute.Key)
        {
            EndpointType = typeof(ServiceRowEndpoint);
        }
    }

    private class RestrictedServiceLookupAttribute : ServiceLookupEditorBaseAttribute
    {
        public RestrictedServiceLookupAttribute() : base(ServiceLookupEditorAttribute.Key)
        {
            ItemType = typeof(ServiceRow);
            EndpointType = typeof(RestrictedServiceRowEndpoint);
        }
    }

    private class ExcludedServiceLookupAttribute : ServiceLookupEditorBaseAttribute
    {
        public ExcludedServiceLookupAttribute() : base(ServiceLookupEditorAttribute.Key)
        {
            ItemType = typeof(ServiceRow);
            EndpointType = typeof(RestrictedServiceRowEndpoint);
            ActionName = "ListWithoutTextAndCriteria";
        }
    }

    private class ExplicitCapabilitiesServiceLookupAttribute : ServiceLookupEditorBaseAttribute
    {
        public ExplicitCapabilitiesServiceLookupAttribute() : base(ServiceLookupEditorAttribute.Key)
        {
            ItemType = typeof(ServiceRow);
            Capabilities = Serenity.Services.ListRequestCapabilities.Skip |
                Serenity.Services.ListRequestCapabilities.Take;
        }
    }

    private class CustomRequestServiceLookupAttribute : ServiceLookupEditorBaseAttribute
    {
        public CustomRequestServiceLookupAttribute() : base(ServiceLookupEditorAttribute.Key)
        {
            ItemType = typeof(ServiceRow);
            EndpointType = typeof(CustomServiceRowEndpoint);
            ActionName = "Search";
        }
    }

    private class ClientManagedLookupEditorAttribute : ServiceLookupEditorBaseAttribute
    {
        public ClientManagedLookupEditorAttribute() : base("ClientManagedLookup")
        {
            Service = "Custom/Items/List";
            ItemType = typeof(AmbiguousLookupOption);
        }
    }

    private class EndpointOnlyServiceLookupForm
    {
        [EndpointOnlyServiceLookup]
        public int? Item { get; set; }
    }

    private class RestrictedServiceLookupForm
    {
        [RestrictedServiceLookup]
        public int? Item { get; set; }
    }

    private class ExcludedServiceLookupForm
    {
        [ExcludedServiceLookup]
        public int? Item { get; set; }
    }

    private class ExplicitCapabilitiesServiceLookupForm
    {
        [ExplicitCapabilitiesServiceLookup]
        public int? Item { get; set; }
    }

    private class CustomRequestServiceLookupForm
    {
        [CustomRequestServiceLookup]
        public int? Item { get; set; }
    }

    private class ClientManagedLookupForm
    {
        [ClientManagedLookupEditor]
        public int? Item { get; set; }
    }

    private class BasedRow : Row<BasedRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [NotNull, Size(40)]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        [Insertable(false), Updatable(false)]
        public string? Locked { get => fields.Locked[this]; set => fields.Locked[this] = value; }

        [NotMapped]
        public string? Computed { get => fields.Computed[this]; set => fields.Computed[this] = value; }

        public int? IntField { get => fields.IntField[this]; set => fields.IntField[this] = value; }
        public short? ShortField { get => fields.ShortField[this]; set => fields.ShortField[this] = value; }
        public DateTime? DateTimeField { get => fields.DateTimeField[this]; set => fields.DateTimeField[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
            public StringField Name = null!;
            public StringField Locked = null!;
            public StringField Computed = null!;
            public Int32Field IntField = null!;
            public Int16Field ShortField = null!;
            public DateTimeField DateTimeField = null!;
        }
    }

    private class BasedForm
    {
        public string? Name { get; set; }
        public string? Locked { get; set; }
        public string? Computed { get; set; }
        public int? IntField { get; set; }
        public short? ShortField { get; set; }
        public DateTime? DateTimeField { get; set; }
        public string? Plain { get; set; }
    }

    private static PropertyItem Process<TForm>(string propName, IRow? row = null,
        BasicPropertyProcessor? processor = null)
        => Process(typeof(TForm), propName, row, processor);

    private static PropertyItem Process(Type formType, string propName, IRow? row = null,
        BasicPropertyProcessor? processor = null)
    {
        var prop = formType.GetProperty(propName)
            ?? throw new InvalidOperationException("Property not found: " + propName);
        var source = new PropertyInfoSource(prop, row);
        var item = new PropertyItem { Name = propName };
        (processor ?? new BasicPropertyProcessor()).Process(source, item);
        return item;
    }

    [Fact]
    public void Sets_Flags_From_Attributes()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.AllFlags));

        Assert.Equal("right", item.Alignment);
        Assert.False(item.AllowHide);
        Assert.True(item.Collapsible);
        Assert.True(item.Collapsed);
        Assert.False(item.Focusable);
        Assert.False(item.Visible);
        Assert.False(item.ShowSelection);
        Assert.False(item.Tabbable);
        Assert.True(item.ReadOnly);
        Assert.True(item.Required);
        Assert.True(item.HideOnInsert);
        Assert.True(item.HideOnUpdate);
        Assert.True(item.Unbound);
        Assert.True(item.SkipOnLoad);
        Assert.True(item.SkipOnSave);
        Assert.Equal("start", item.Pin);
        Assert.Equal(3, item.GroupOrder);
        Assert.Equal(2, item.SortOrder);
        Assert.False(item.Sortable);
        Assert.Equal(SummaryType.Sum, item.SummaryType);
        Assert.Equal("cls", item.CssClass);
        Assert.Equal("hdr", item.HeaderCssClass);
        Assert.Equal("ec", item.EditorCssClass);
        Assert.Equal("fcc", item.FormCssClass);
        Assert.Equal("150px", item.LabelWidth);
        Assert.Equal(123, item.Width);
        Assert.True(item.WidthSet);
        Assert.Equal(60, item.MinWidth);
        Assert.Equal(300, item.MaxWidth);
        Assert.Equal(50, item.MaxLength);
    }

    [Fact]
    public void Sets_Localizable_Texts()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.Texts));

        Assert.Contains("MyCat", item.Category);
        Assert.Contains("MyTab", item.Tab);
        Assert.Contains("MyTitle", item.Title);
        Assert.Contains("MyHint", item.Hint);
        Assert.Contains("MyPh", item.Placeholder);
    }

    [Fact]
    public void Sets_DefaultValue()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.Defaulted));
        Assert.Equal(42, item.DefaultValue);
    }

    [Fact]
    public void Sets_EditLink()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.Linked));

        Assert.True(item.EditLink);
        Assert.Equal("MyItem", item.EditLinkItemType);
        Assert.Equal("MyId", item.EditLinkIdField);
        Assert.Equal("elc", item.EditLinkCssClass);
    }

    [Fact]
    public void Sets_EditLink_WhenFalse()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.NotLinked));
        Assert.NotEqual(true, item.EditLink);
    }

    [Fact]
    public void Sets_EditorType_And_Options()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.Edited));

        Assert.Equal("MyEditor", item.EditorType);
        Assert.Equal("someValue", item.EditorParams["someKey"]);
    }

    [Fact]
    public void Sets_EditorAddons()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.WithAddon));

        var addon = Assert.Single(item.EditorAddons);
        Assert.Equal("my.addon", addon.Type);
    }

    [Fact]
    public void Sets_MaxLength_Via_Attribute()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.Capped));

        Assert.Equal(80, item.MaxLength);
        Assert.Equal(80, item.EditorParams["maxLength"]);
    }

    [Fact]
    public void Sets_DateTimeKind_UseUtc()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.UtcDate));
        Assert.Equal(true, item.EditorParams["useUtc"]);
    }

    [Fact]
    public void Sets_NotFilterable()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.NotFilter));
        Assert.True(item.NotFilterable);
    }

    [Fact]
    public void Sets_FilterOnly()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.FilterOnlyProp));
        Assert.True(item.FilterOnly);
    }

    [Fact]
    public void Sets_QuickFilter()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.QuickFiltered));

        Assert.True(item.QuickFilter);
        Assert.True(item.QuickFilterSeparator);
        Assert.Equal("qfc", item.QuickFilterCssClass);
    }

    [Fact]
    public void Sets_FilteringIdField()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.FilterById));

        Assert.Equal("SomeId", item.FilteringIdField);
        Assert.Equal("SomeId", item.FilteringParams["idField"]);
    }

    [Fact]
    public void Sets_Custom_FilteringType_And_Options()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.CustomFiltered));

        Assert.Equal("MyFiltering", item.FilteringType);
        Assert.Equal("someFilterValue", item.FilteringParams["someFilterKey"]);
    }

    [Fact]
    public void Sets_QuickFilter_Options()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.QuickFilterOptionProp));
        Assert.Equal("qfValue", item.QuickFilterParams["qfKey"]);
    }

    [Fact]
    public void Sets_DisplayFormat()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.Formatted));

        Assert.Equal("MyFormat", item.DisplayFormat);
        Assert.Equal("MyFormat", item.FilteringParams["displayFormat"]);
    }

    [Fact]
    public void Sets_Custom_Formatter_And_Options()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.CustomFormatted));

        Assert.Equal("MyFormatter", item.FormatterType);
        Assert.Equal("someFmtValue", item.FormatterParams["someFmtKey"]);
    }

    [Fact]
    public void Sets_Permissions()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.Permissions));

        Assert.Equal("InsertP", item.InsertPermission);
        Assert.Equal("ReadP", item.ReadPermission);
        Assert.Equal("UpdateP", item.UpdatePermission);
    }

    [Fact]
    public void Sets_Insertable_And_Updatable_False()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.NotInsertable));

        Assert.False(item.Insertable);
        Assert.False(item.Updatable);
    }

    [Fact]
    public void Sets_Resizable_False_Via_Attribute()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.NotResizable));
        Assert.False(item.Resizable);
    }

    [Fact]
    public void Sets_Resizable_False_Via_FixedWidth()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.FixedW));
        Assert.False(item.Resizable);
    }

    [Fact]
    public void FormWidth_JustThis_DoesNot_Affect_Next()
    {
        var processor = new BasicPropertyProcessor();
        var a = Process<DecoratedForm>(nameof(DecoratedForm.FormWidthJustThis), processor: processor);
        var b = Process<DecoratedForm>(nameof(DecoratedForm.AfterFormWidth), processor: processor);

        Assert.Equal("fw", a.FormCssClass);
        Assert.Null(b.FormCssClass);
    }

    [Fact]
    public void FormWidth_UntilNext_Affects_Next()
    {
        var processor = new BasicPropertyProcessor();
        var a = Process<DecoratedForm>(nameof(DecoratedForm.FormWidthUntilNext), processor: processor);
        var b = Process<DecoratedForm>(nameof(DecoratedForm.AfterFormWidth), processor: processor);

        Assert.Equal("fwUntilNext", a.FormCssClass);
        Assert.Equal("fwUntilNext", b.FormCssClass);
    }

    [Fact]
    public void LabelWidth_JustThis_DoesNot_Affect_Next()
    {
        var processor = new BasicPropertyProcessor();
        var a = Process<DecoratedForm>(nameof(DecoratedForm.LabelWidthJustThis), processor: processor);
        var b = Process<DecoratedForm>(nameof(DecoratedForm.AfterFormWidth), processor: processor);

        Assert.Equal("220px", a.LabelWidth);
        Assert.NotEqual("220px", b.LabelWidth);
    }

    [Theory]
    [InlineData(nameof(DecoratedForm.IntProp), "Integer")]
    [InlineData(nameof(DecoratedForm.ShortProp), "Integer")]
    [InlineData(nameof(DecoratedForm.LongProp), "String")]
    [InlineData(nameof(DecoratedForm.BoolProp), "Boolean")]
    [InlineData(nameof(DecoratedForm.DecimalProp), "Decimal")]
    [InlineData(nameof(DecoratedForm.DoubleProp), "Decimal")]
    [InlineData(nameof(DecoratedForm.FloatProp), "Decimal")]
    [InlineData(nameof(DecoratedForm.DateProp), "Date")]
    [InlineData(nameof(DecoratedForm.TimeProp), "TimeSpan")]
    [InlineData(nameof(DecoratedForm.EnumProp), "Enum")]
    [InlineData(nameof(DecoratedForm.Plain), "String")]
    public void AutoDetermines_EditorType(string propName, string expected)
    {
        var item = Process<DecoratedForm>(propName);
        Assert.Equal(expected, item.EditorType);
    }

    [Fact]
    public void ShortEditor_Sets_MaxValue()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.ShortProp));
        Assert.Equal(short.MaxValue, item.EditorParams["maxValue"]);
    }

    [Theory]
    [InlineData(nameof(DecoratedForm.EnumProp), "Enum")]
    [InlineData(nameof(DecoratedForm.DateProp), "Date")]
    [InlineData(nameof(DecoratedForm.BoolProp), "Checkbox")]
    [InlineData(nameof(DecoratedForm.DecimalProp), "Number")]
    [InlineData(nameof(DecoratedForm.IntProp), "Number")]
    public void AutoDetermines_FormatterType(string propName, string expected)
    {
        var item = Process<DecoratedForm>(propName);
        Assert.Equal(expected, item.FormatterType);
    }

    [Theory]
    [InlineData(nameof(DecoratedForm.EnumProp), "Enum")]
    [InlineData(nameof(DecoratedForm.DateProp), "Date")]
    [InlineData(nameof(DecoratedForm.BoolProp), "Boolean")]
    [InlineData(nameof(DecoratedForm.DecimalProp), "Decimal")]
    [InlineData(nameof(DecoratedForm.IntProp), "Integer")]
    [InlineData(nameof(DecoratedForm.LongProp), "Integer")]
    [InlineData(nameof(DecoratedForm.Plain), "String")]
    public void AutoDetermines_FilteringType(string propName, string expected)
    {
        var item = Process<DecoratedForm>(propName);
        Assert.Equal(expected, item.FilteringType);
    }

    [Fact]
    public void SummaryType_Default_For_Numeric()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.IntProp));
        Assert.Equal(SummaryType.Sum, item.SummaryType);
    }

    [Fact]
    public void SummaryType_Uses_Options_Default()
    {
        var processor = new BasicPropertyProcessor(Options.Create(new PropertyProcessorOptions
        {
            DefaultSummaryType = SummaryType.Avg
        }));
        var item = Process<DecoratedForm>(nameof(DecoratedForm.IntProp), processor: processor);
        Assert.Equal(SummaryType.Avg, item.SummaryType);
    }

    [Fact]
    public void SummaryType_Disabled_For_PrimaryKey()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.PrimaryKeyProp));
        Assert.Equal(SummaryType.Disabled, item.SummaryType);
    }

    [Fact]
    public void SummaryType_Disabled_For_String()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.Plain));
        Assert.Equal(SummaryType.Disabled, item.SummaryType);
    }

    [Fact]
    public void Title_Falls_Back_To_Name()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.Plain));
        Assert.Equal(nameof(DecoratedForm.Plain), item.Title);
    }

    [Fact]
    public void Category_And_Tab_Fall_Back_To_Previous_Items()
    {
        var processor = new BasicPropertyProcessor
        {
            Items = [new PropertyItem { Category = "PrevCat", Tab = "PrevTab" }]
        };

        var item = Process<DecoratedForm>(nameof(DecoratedForm.Plain), processor: processor);

        Assert.Equal("PrevCat", item.Category);
        Assert.Equal("PrevTab", item.Tab);
    }

    [Fact]
    public void Width_Falls_Back_To_Default()
    {
        var item = Process<DecoratedForm>(nameof(DecoratedForm.Plain));
        Assert.Equal(80, item.Width);
    }

    [Fact]
    public void ServiceLookupEditor_Determines_Service_And_Fields()
    {
        var item = Process<ServiceLookupForm>(nameof(ServiceLookupForm.Item));

        Assert.Equal("ServiceLookup", item.EditorType);
        Assert.NotNull(item.EditorParams["service"]);
        Assert.Equal("ID", item.EditorParams["idField"]);
        Assert.Equal("Name", item.EditorParams["textField"]);
        Assert.Equal("ServiceLookup", item.FilteringType);
    }

    [Fact]
    public void ServiceLookupEditor_Determines_IncludeColumns_When_Fields_Are_Explicit()
    {
        var item = Process<ServiceLookupWithExplicitFieldsForm>(
            nameof(ServiceLookupWithExplicitFieldsForm.Item));

        Assert.Equal("ID", item.EditorParams["idField"]);
        Assert.Equal("Name", item.EditorParams["textField"]);
        Assert.Equal(new[] { "Name" }, item.EditorParams["includeColumns"]);
    }

    [Fact]
    public void ServiceLookupEditor_DoesNotInfer_IncludeColumns_When_ColumnSelection_Is_Explicit()
    {
        var item = Process<ServiceLookupWithLookupColumnSelectionForm>(
            nameof(ServiceLookupWithLookupColumnSelectionForm.Item));

        Assert.Equal(ColumnSelection.Lookup, item.EditorParams["columnSelection"]);
        Assert.False(item.EditorParams.ContainsKey("includeColumns"));
    }

    [Fact]
    public void ServiceLookupEditor_Uses_EndpointType_When_ItemType_Is_Not_Set()
    {
        var item = Process<EndpointOnlyServiceLookupForm>(
            nameof(EndpointOnlyServiceLookupForm.Item));

        Assert.Equal("BasicPropertyProcessor/ServiceRow/List", item.EditorParams["service"]);
        Assert.Equal("ID", item.EditorParams["idField"]);
        Assert.Equal("Name", item.EditorParams["textField"]);
        Assert.Equal(new[] { "Name" }, item.EditorParams["includeColumns"]);
        Assert.Equal(Serenity.Services.ListRequestCapabilities.None, item.EditorParams["capabilities"]);
    }

    [Fact]
    public void ServiceLookupEditor_Uses_ActionCapabilities_When_Configured()
    {
        var item = Process<RestrictedServiceLookupForm>(
            nameof(RestrictedServiceLookupForm.Item));

        Assert.Equal(Serenity.Services.ListRequestCapabilities.Skip |
            Serenity.Services.ListRequestCapabilities.Take, item.EditorParams["capabilities"]);
    }

    [Fact]
    public void ServiceLookupEditor_Uses_Explicit_Capability_Flags()
    {
        var item = Process<ExplicitCapabilitiesServiceLookupForm>(
            nameof(ExplicitCapabilitiesServiceLookupForm.Item));

        Assert.Equal(Serenity.Services.ListRequestCapabilities.Skip |
            Serenity.Services.ListRequestCapabilities.Take, item.EditorParams["capabilities"]);
    }

    [Fact]
    public void ServiceLookupEditor_Excludes_ActionCapabilities_From_ListRequest_Defaults()
    {
        var item = Process<ExcludedServiceLookupForm>(
            nameof(ExcludedServiceLookupForm.Item));
        var capabilities = Assert.IsType<Serenity.Services.ListRequestCapabilities>(
            item.EditorParams["capabilities"]);

        Assert.Equal(Serenity.Services.ListRequestCapabilities.None,
            capabilities & Serenity.Services.ListRequestCapabilities.ContainsText);
        Assert.Equal(Serenity.Services.ListRequestCapabilities.None,
            capabilities & Serenity.Services.ListRequestCapabilities.Criteria);
        Assert.NotEqual(Serenity.Services.ListRequestCapabilities.None,
            capabilities & Serenity.Services.ListRequestCapabilities.Skip);
        Assert.NotEqual(Serenity.Services.ListRequestCapabilities.None,
            capabilities & Serenity.Services.ListRequestCapabilities.Take);
        Assert.NotEqual(Serenity.Services.ListRequestCapabilities.None,
            capabilities & Serenity.Services.ListRequestCapabilities.IncludeColumns);
    }

    [Fact]
    public void ServiceLookupEditor_Infers_Capabilities_From_Custom_Request_Properties()
    {
        var item = Process<CustomRequestServiceLookupForm>(
            nameof(CustomRequestServiceLookupForm.Item));

        Assert.Equal(Serenity.Services.ListRequestCapabilities.Skip |
            Serenity.Services.ListRequestCapabilities.ContainsText, item.EditorParams["capabilities"]);
    }

    [Fact]
    public void ServiceLookupEditor_Allows_ClientManaged_Fields_Without_ItemType()
    {
        var item = Process<ClientManagedLookupForm>(nameof(ClientManagedLookupForm.Item));

        Assert.Equal("ClientManagedLookup", item.EditorType);
        Assert.Equal("Custom/Items/List", item.EditorParams["service"]);
        Assert.False(item.EditorParams.ContainsKey("idField"));
        Assert.False(item.EditorParams.ContainsKey("textField"));
    }

    [Fact]
    public void ServiceLookupEditor_InferFields_For_NonRowItemType()
    {
        var item = Process<ConnectorTypeLookupForm>(
            nameof(ConnectorTypeLookupForm.ConnectorType));

        Assert.Equal("ProDataSources/ConnectorTypes/GetTypes", item.EditorParams["service"]);
        Assert.Equal("Code", item.EditorParams["idField"]);
        Assert.Equal("DisplayName", item.EditorParams["textField"]);
    }

    [Fact]
    public void ServiceLookupEditor_Infer_QuickSearchFields_When_ContainsText_Is_Unsupported()
    {
        var item = Process<QuickSearchLookupForm>(nameof(QuickSearchLookupForm.Item));

        var capabilities = Assert.IsType<Serenity.Services.ListRequestCapabilities>(
            item.EditorParams["capabilities"]);
        Assert.Equal(Serenity.Services.ListRequestCapabilities.None,
            capabilities & Serenity.Services.ListRequestCapabilities.ContainsText);
        Assert.Equal(new[] { "Email" }, item.EditorParams["quickSearchFields"]);
    }

    [Fact]
    public void ServiceLookupEditor_Infer_Value_Fields_For_Primitive_List_Responses()
    {
        var listItem = Process<PrimitiveListLookupForm>(nameof(PrimitiveListLookupForm.Item));
        var responseItem = Process<PrimitiveListResponseLookupForm>(
            nameof(PrimitiveListResponseLookupForm.Item));

        Assert.Equal("Value", listItem.EditorParams["idField"]);
        Assert.Equal("Value", listItem.EditorParams["textField"]);
        Assert.Equal("Value", responseItem.EditorParams["idField"]);
        Assert.Equal("Value", responseItem.EditorParams["textField"]);
    }

    [Fact]
    public void ServiceLookupEditor_Respects_Null_IdField_And_Infers_TextField()
    {
        var item = Process<ConnectorTypeLookupWithNullIdFieldForm>(
            nameof(ConnectorTypeLookupWithNullIdFieldForm.ConnectorType));

        Assert.True(item.EditorParams.ContainsKey("idField"));
        Assert.Null(item.EditorParams["idField"]);
        Assert.Equal("DisplayName", item.EditorParams["textField"]);
    }

    [Fact]
    public void ServiceLookupEditor_Respects_Null_TextField_And_Infers_IdField()
    {
        var item = Process<ConnectorTypeLookupWithNullTextFieldForm>(
            nameof(ConnectorTypeLookupWithNullTextFieldForm.ConnectorType));

        Assert.Equal("Code", item.EditorParams["idField"]);
        Assert.True(item.EditorParams.ContainsKey("textField"));
        Assert.Null(item.EditorParams["textField"]);
    }

    [Fact]
    public void ServiceLookupEditor_Uses_SingleStringProperty_For_Id_And_Text()
    {
        var item = Process<SingleTextLookupForm>(nameof(SingleTextLookupForm.Value));

        Assert.Equal("Value", item.EditorParams["idField"]);
        Assert.Equal("Value", item.EditorParams["textField"]);
    }

    [Fact]
    public void ServiceLookupEditor_Throws_When_NonRowFields_Cannot_Be_Inferred()
    {
        var error = Assert.Throws<InvalidOperationException>(() =>
            Process<AmbiguousLookupForm>(nameof(AmbiguousLookupForm.Value)));

        Assert.Contains("IdField and TextField", error.Message, StringComparison.Ordinal);
        Assert.Contains(nameof(AmbiguousLookupOption), error.Message, StringComparison.Ordinal);
        Assert.Contains("IdPropertyAttribute and NamePropertyAttribute", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ServiceLookupEditor_Throws_When_Row_Has_No_Id_Or_Name_Field()
    {
        var error = Assert.Throws<InvalidOperationException>(() =>
            Process<ServiceLookupWithoutRowFieldsForm>(
                nameof(ServiceLookupWithoutRowFieldsForm.Item)));

        Assert.Contains("IdField and TextField", error.Message, StringComparison.Ordinal);
        Assert.Contains(nameof(ServiceRowWithoutLookupFields), error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BasedOnField_Required_And_Insertable()
    {
        var row = new BasedRow();
        var required = Process<BasedForm>(nameof(BasedForm.Name), row);
        Assert.True(required.Required);

        var locked = Process<BasedForm>(nameof(BasedForm.Locked), row);
        Assert.False(locked.Insertable);
        Assert.False(locked.Updatable);
    }

    [Fact]
    public void BasedOnField_NotMapped_Is_NotSortable()
    {
        var row = new BasedRow();
        var item = Process<BasedForm>(nameof(BasedForm.Computed), row);
        Assert.False(item.Sortable);
    }

    [Fact]
    public void BasedOnField_Integer_Sets_MaxValue()
    {
        var row = new BasedRow();
        var item = Process<BasedForm>(nameof(BasedForm.IntField), row);
        Assert.Equal(int.MaxValue, item.EditorParams["maxValue"]);
    }

    [Fact]
    public void BasedOnField_Short_Sets_MaxValue()
    {
        var row = new BasedRow();
        var item = Process<BasedForm>(nameof(BasedForm.ShortField), row);
        Assert.Equal(short.MaxValue, item.EditorParams["maxValue"]);
    }

    [Fact]
    public void BasedOnField_String_Sets_MaxLength()
    {
        var row = new BasedRow();
        var item = Process<BasedForm>(nameof(BasedForm.Name), row);
        Assert.Equal(40, item.MaxLength);
        Assert.Equal(40, item.EditorParams["maxLength"]);
    }
}
