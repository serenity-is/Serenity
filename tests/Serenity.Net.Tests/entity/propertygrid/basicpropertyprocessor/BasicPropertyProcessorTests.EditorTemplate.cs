namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    private class TemplateItem
    {
        public string Key { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? IconClass { get; set; }
    }

    private class TemplateItemListRequest : ListRequest
    {
        public int? ParentId { get; set; }
    }

    [Route("Services/BasicPropertyProcessor/TemplateItems/[action]")]
    private class TemplateItemEndpoint
    {
        [ListRequestCapabilities(Serenity.Services.ListRequestCapabilities.SourceAdapter)]
        public ListResponse<TemplateItem> ListItems(TemplateItemListRequest request)
        {
            return new();
        }
    }

    // like an attribute generated for a client side editor, with a template declared on its partial class
    [ServiceLookupEditor(typeof(TemplateItem), EndpointType = typeof(TemplateItemEndpoint),
        ActionName = nameof(TemplateItemEndpoint.ListItems), MinimumResultsForSearch = 10, InplaceAdd = true)]
    private class TemplatedLookupEditorAttribute : ServiceLookupEditorBaseAttribute
    {
        public const string Key = "Test.TemplatedLookupEditor";

        public TemplatedLookupEditorAttribute()
            : base(Key)
        {
        }
    }

    [LookupEditor("Test.Lookup", InplaceAdd = true)]
    private class MismatchedTemplateEditorAttribute : ServiceLookupEditorBaseAttribute
    {
        public MismatchedTemplateEditorAttribute()
            : base("Test.MismatchedTemplateEditor")
        {
        }
    }

    private class TemplatedLookupForm
    {
        [TemplatedLookupEditor]
        public string? Plain { get; set; }

        [TemplatedLookupEditor(MinimumResultsForSearch = 5)]
        public string? Overridden { get; set; }

        [TemplatedLookupEditor(ItemType = typeof(ServiceRow), EndpointType = typeof(RestrictedServiceRowEndpoint))]
        public int? OwnEndpoint { get; set; }

        [MismatchedTemplateEditor]
        public string? Mismatched { get; set; }

        [ServiceLookupEditor(typeof(ServiceRow), EndpointType = typeof(RestrictedServiceRowEndpoint))]
        public int? Regular { get; set; }
    }

    [Fact]
    public void EditorTemplate_Provides_Service_Capabilities_And_Fields_From_Its_Endpoint()
    {
        var item = Process<TemplatedLookupForm>(nameof(TemplatedLookupForm.Plain));

        Assert.Equal(TemplatedLookupEditorAttribute.Key, item.EditorType);
        Assert.Equal("BasicPropertyProcessor/TemplateItems/ListItems", item.EditorParams["service"]);
        Assert.Equal(Serenity.Services.ListRequestCapabilities.SourceAdapter, item.EditorParams["capabilities"]);
        Assert.Equal("Key", item.EditorParams["idField"]);
        Assert.Equal("Name", item.EditorParams["textField"]);
        Assert.Equal("IconClass", item.EditorParams["iconField"]);
    }

    [Fact]
    public void EditorTemplate_Provides_Option_Defaults()
    {
        var item = Process<TemplatedLookupForm>(nameof(TemplatedLookupForm.Plain));

        Assert.Equal(10, item.EditorParams["minimumResultsForSearch"]);
        Assert.Equal(true, item.EditorParams["inplaceAdd"]);
    }

    [Fact]
    public void EditorTemplate_Options_Are_Overridden_By_The_Attribute()
    {
        var item = Process<TemplatedLookupForm>(nameof(TemplatedLookupForm.Overridden));

        Assert.Equal(5, item.EditorParams["minimumResultsForSearch"]);
        Assert.Equal(true, item.EditorParams["inplaceAdd"]);
        Assert.Equal("BasicPropertyProcessor/TemplateItems/ListItems", item.EditorParams["service"]);
    }

    [Fact]
    public void EditorTemplate_Endpoint_Is_Not_Used_When_The_Attribute_Has_Its_Own()
    {
        var item = Process<TemplatedLookupForm>(nameof(TemplatedLookupForm.OwnEndpoint));

        Assert.Equal("BasicPropertyProcessor/RestrictedServiceRow/List", item.EditorParams["service"]);
        Assert.Equal(Serenity.Services.ListRequestCapabilities.Skip |
            Serenity.Services.ListRequestCapabilities.Take, item.EditorParams["capabilities"]);
        Assert.Equal("ID", item.EditorParams["idField"]);
        Assert.Equal(10, item.EditorParams["minimumResultsForSearch"]);
    }

    [Fact]
    public void EditorTemplate_Of_Another_Editor_Family_Is_Ignored()
    {
        var item = Process<TemplatedLookupForm>(nameof(TemplatedLookupForm.Mismatched));

        Assert.Equal("Test.MismatchedTemplateEditor", item.EditorType);
        Assert.False(item.EditorParams.ContainsKey("lookupKey"));
        Assert.False(item.EditorParams.ContainsKey("inplaceAdd"));
        Assert.False(item.EditorParams.ContainsKey("service"));
    }

    [Fact]
    public void EditorTemplate_Provides_Service_Options_To_Editor_Filtering()
    {
        var item = Process<TemplatedLookupForm>(nameof(TemplatedLookupForm.Plain));

        Assert.Equal("Editor", item.FilteringType);
        Assert.Equal(TemplatedLookupEditorAttribute.Key, item.FilteringParams["editorType"]);
        Assert.Equal("BasicPropertyProcessor/TemplateItems/ListItems", item.FilteringParams["service"]);
        Assert.Equal(Serenity.Services.ListRequestCapabilities.SourceAdapter, item.FilteringParams["capabilities"]);
        Assert.Equal("Key", item.FilteringParams["idField"]);
    }

    [Fact]
    public void ServiceLookup_Filtering_Includes_Capabilities()
    {
        var item = Process<TemplatedLookupForm>(nameof(TemplatedLookupForm.Regular));

        Assert.Equal("ServiceLookup", item.FilteringType);
        Assert.Equal(Serenity.Services.ListRequestCapabilities.Skip |
            Serenity.Services.ListRequestCapabilities.Take, item.FilteringParams["capabilities"]);
    }
}
