
namespace Serenity.ComponentModel;

/// <summary>
/// Corresponds to a property item (e.g. a field in property grid
/// with a label and editor, or a column specification in a grid)
/// </summary>
public class PropertyItem
{
    /// <summary>
    /// Gets or sets the property name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    [Newtonsoft.Json.JsonProperty("name")]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the title, e.g. label of a form field, 
    /// or caption of a grid column.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    [Newtonsoft.Json.JsonProperty("title")]
    [JsonPropertyName("title")]
    public string? Title { get; set; }


    /// <summary>
    /// Gets or sets the hint. This is only used for forms and 
    /// it is shown when the label is hovered.
    /// </summary>
    /// <value>
    /// The hint.
    /// </value>
    [Newtonsoft.Json.JsonProperty("hint")]
    [JsonPropertyName("hint")]
    public string? Hint { get; set; }


    /// <summary>
    /// Gets or sets the placeholder for the editor
    /// when it has an empty value. Only useful for forms.
    /// </summary>
    /// <value>
    /// The placeholder.
    /// </value>
    [Newtonsoft.Json.JsonProperty("placeholder")]
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }


    /// <summary>
    /// Gets or sets the type of the editor used in form.
    /// </summary>
    /// <value>
    /// The type of the editor.
    /// </value>
    [Newtonsoft.Json.JsonProperty("editorType")]
    [JsonPropertyName("editorType")]
    public string? EditorType { get; set; }


    /// <summary>
    /// Gets or sets the editor parameters 
    /// that should be passed to the editor while it
    /// is being created.
    /// </summary>
    /// <value>
    /// The editor parameters.
    /// </value>
    [Newtonsoft.Json.JsonProperty("editorParams")]
    [JsonPropertyName("editorParams")]
    public Dictionary<string, object>? EditorParams { get; set; }

    /// <summary>
    /// Gets or sets the category of the property in form.
    /// Only meaningful for forms.
    /// </summary>
    /// <value>
    /// The category.
    /// </value>
    [Newtonsoft.Json.JsonProperty("category")]
    [JsonPropertyName("category")]
    public string? Category { get; set; }


    /// <summary>
    /// Gets or sets the collapsible flag for the category this property is in.
    /// Should only be set for the first field in the category.
    /// </summary>
    /// <value>
    /// The collapsible.
    /// </value>
    [Newtonsoft.Json.JsonProperty("collapsible")]
    [JsonPropertyName("collapsible")]
    public bool? Collapsible { get; set; }

    /// <summary>
    /// Gets or sets the initial collapsed status of the category this property is in.
    /// </summary>
    /// <value>
    /// The collapsed.
    /// </value>
    [Newtonsoft.Json.JsonProperty("collapsed")]
    [JsonPropertyName("collapsed")]
    public bool? Collapsed { get; set; }

    /// <summary>
    /// Gets or sets the tab for the property. Used only in forms.
    /// </summary>
    /// <value>
    /// The tab.
    /// </value>
    [Newtonsoft.Json.JsonProperty("tab")]
    [JsonPropertyName("tab")]
    public string? Tab { get; set; }

    /// <summary>
    /// Gets or sets the CSS class that should be applied to the grid column, 
    /// e.g. data cells for this property. Only used in grids.
    /// </summary>
    /// <value>
    /// The CSS class.
    /// </value>
    [Newtonsoft.Json.JsonProperty("cssClass")]
    [JsonPropertyName("cssClass")]
    public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets the header CSS class for the grid column.
    /// Only used in grids.
    /// </summary>
    /// <value>
    /// The header CSS class.
    /// </value>
    [Newtonsoft.Json.JsonProperty("headerCssClass")]
    [JsonPropertyName("headerCssClass")]
    public string? HeaderCssClass { get; set; }

    /// <summary>
    /// Gets or sets the form CSS class for the property.
    /// This class is applied to containing "div.field" not the editor.
    /// Only used in forms.
    /// </summary>
    /// <value>
    /// The form CSS class.
    /// </value>
    [Newtonsoft.Json.JsonProperty("formCssClass")]
    [JsonPropertyName("formCssClass")]
    public string? FormCssClass { get; set; }

    /// <summary>
    /// Gets or sets the maximum length of the editor in a form.
    /// </summary>
    /// <value>
    /// The maximum length.
    /// </value>
    [Newtonsoft.Json.JsonProperty("maxLength")]
    [JsonPropertyName("maxLength")]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets the required flag of the property in forms.
    /// </summary>
    /// <value>
    /// The required.
    /// </value>
    [Newtonsoft.Json.JsonProperty("required")]
    [JsonPropertyName("required")]
    public bool? Required { get; set; }

    /// <summary>
    /// Gets or sets the read only flag of the property in forms.
    /// </summary>
    /// <value>
    /// The read only.
    /// </value>
    [Newtonsoft.Json.JsonProperty("readOnly")]
    [JsonPropertyName("readOnly")]
    public bool? ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the read permission. If user doesn't
    /// have this permission, they won't see it in grid / form.
    /// </summary>
    /// <value>
    /// The read permission.
    /// </value>
    [Newtonsoft.Json.JsonProperty("readPermission")]
    [JsonPropertyName("readPermission")]
    public string? ReadPermission { get; set; }

    /// <summary>
    /// Gets or sets the insertable flag of the property.
    /// If this is false, the property won't be serialized
    /// back in new record forms. Does not apply to grids.
    /// </summary>
    /// <value>
    /// The insertable.
    /// </value>
    [Newtonsoft.Json.JsonProperty("insertable")]
    [JsonPropertyName("insertable")]
    public bool? Insertable { get; set; }

    /// <summary>
    /// Gets or sets the insert permission. If user doesn't
    /// have this permission, field won't be serialized back in new
    /// record forms and it will be readonly.
    /// </summary>
    /// <value>
    /// The insert permission.
    /// </value>
    [Newtonsoft.Json.JsonProperty("insertPermission")]
    [JsonPropertyName("insertPermission")]
    public string? InsertPermission { get; set; }

    /// <summary>
    /// Gets or sets the hide on insert flag which controls visibility of the property
    /// in new record forms.
    /// </summary>
    /// <value>
    /// The hide on insert flag.
    /// </value>
    [Newtonsoft.Json.JsonProperty("hideOnInsert")]
    [JsonPropertyName("hideOnInsert")]
    public bool? HideOnInsert { get; set; }


    /// <summary>
    /// Gets or sets the updatable flag of the property.
    /// If it is false, property won't be serialized back
    /// in edit forms and it will be readonly.
    /// </summary>
    /// <value>
    /// The updatable.
    /// </value>
    [Newtonsoft.Json.JsonProperty("updatable")]
    [JsonPropertyName("updatable")]
    public bool? Updatable { get; set; }

    /// <summary>
    /// Gets or sets the update permission. If user doesn't have
    /// this permission, field won't be serialized back in edit
    /// record forms and it will be readonly.
    /// </summary>
    /// <value>
    /// The update permission.
    /// </value>
    [Newtonsoft.Json.JsonProperty("updatePermission")]
    [JsonPropertyName("updatePermission")]
    public string? UpdatePermission { get; set; }

    /// <summary>
    /// Gets or sets the hide on update flag which controls visibility of the property
    /// in edit record forms.
    /// </summary>
    /// <value>
    /// The hide on insert flag.
    /// </value>
    [Newtonsoft.Json.JsonProperty("hideOnUpdate")]
    [JsonPropertyName("hideOnUpdate")]
    public bool? HideOnUpdate { get; set; }

    /// <summary>
    /// Gets or sets the one way flag, which when true field 
    /// won't be serialized back to entity on save.
    /// </summary>
    /// <value>
    /// The one way.
    /// </value>
    [Newtonsoft.Json.JsonProperty("oneWay")]
    [JsonPropertyName("oneWay")]
    public bool? OneWay { get; set; }

    /// <summary>
    /// Gets or sets the default value of the property in 
    /// new record forms.
    /// </summary>
    /// <value>
    /// The default value.
    /// </value>
    [Newtonsoft.Json.JsonProperty("defaultValue")]
    [JsonPropertyName("defaultValue")]
    public object? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the localizable flag of the property.
    /// Only fields with this flag are shown in localization
    /// tab of entity dialog.
    /// </summary>
    /// <value>
    /// The localizable.
    /// </value>
    [Newtonsoft.Json.JsonProperty("localizable")]
    [JsonPropertyName("localizable")]
    public bool? Localizable { get; set; }


    /// <summary>
    /// Gets or sets the visible flag. Only used for columns.
    /// </summary>
    /// <value>
    /// The visible.
    /// </value>
    [Newtonsoft.Json.JsonProperty("visible")]
    [JsonPropertyName("visible")]
    public bool? Visible { get; set; }


    /// <summary>
    /// Gets or sets the allow hide flag for columns.
    /// </summary>
    /// <value>
    /// The allow hide.
    /// </value>
    [Newtonsoft.Json.JsonProperty("allowHide")]
    [JsonPropertyName("allowHide")]
    public bool? AllowHide { get; set; }

    /// <summary>
    /// Gets or sets the type of the formatter for columns.
    /// </summary>
    /// <value>
    /// The type of the formatter.
    /// </value>
    [Newtonsoft.Json.JsonProperty("formatterType")]
    [JsonPropertyName("formatterType")]
    public string? FormatterType { get; set; }

    /// <summary>
    /// Gets or sets the formatter parameters for columns.
    /// </summary>
    /// <value>
    /// The formatter parameters.
    /// </value>
    [Newtonsoft.Json.JsonProperty("formatterParams")]
    [JsonPropertyName("formatterParams")]
    public Dictionary<string, object>? FormatterParams { get; set; }

    /// <summary>
    /// Gets or sets the display format to be passed to the formatter
    /// of a column. Note that display format has no effect on 
    /// editors.
    /// </summary>
    /// <value>
    /// The display format.
    /// </value>
    [Newtonsoft.Json.JsonProperty("displayFormat")]
    [JsonPropertyName("displayFormat")]
    public string? DisplayFormat { get; set; }

    /// <summary>
    /// Gets or sets the alignment of the column.
    /// Only applies to column, not editors.
    /// </summary>
    /// <value>
    /// The alignment.
    /// </value>
    [Newtonsoft.Json.JsonProperty("alignment")]
    [JsonPropertyName("alignment")]
    public string? Alignment { get; set; }

    /// <summary>
    /// Gets or sets the width of the column.
    /// Does not apply to forms.
    /// </summary>
    /// <value>
    /// The width.
    /// </value>
    [Newtonsoft.Json.JsonProperty("width")]
    [JsonPropertyName("width")]
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the width explicitly set flag.
    /// If property has a [Width] attribute this property
    /// is true. If property column width is automatically
    /// calculated by Serenity based on field metadata, it 
    /// is false.
    /// </summary>
    /// <value>
    /// The width set.
    /// </value>
    [Newtonsoft.Json.JsonProperty("widthSet")]
    [JsonPropertyName("widthSet")]
    public bool? WidthSet { get; set; }

    /// <summary>
    /// Gets or sets the minimum width of a column.
    /// </summary>
    /// <value>
    /// The minimum width.
    /// </value>
    [Newtonsoft.Json.JsonProperty("minWidth")]
    [JsonPropertyName("minWidth")]
    public int? MinWidth { get; set; }

    /// <summary>
    /// Gets or sets the maximum width of a column.
    /// </summary>
    /// <value>
    /// The maximum width.
    /// </value>
    [Newtonsoft.Json.JsonProperty("maxWidth")]
    [JsonPropertyName("maxWidth")]
    public int? MaxWidth { get; set; }

    /// <summary>
    /// Gets or sets the width of the label in forms.
    /// </summary>
    /// <value>
    /// The width of the label.
    /// </value>
    [Newtonsoft.Json.JsonProperty("labelWidth")]
    [JsonPropertyName("labelWidth")]
    public string? LabelWidth { get; set; }

    /// <summary>
    /// Gets or sets the resizable flag of a column.
    /// </summary>
    /// <value>
    /// The resizable.
    /// </value>
    [Newtonsoft.Json.JsonProperty("resizable")]
    [JsonPropertyName("resizable")]
    public bool? Resizable { get; set; }

    /// <summary>
    /// Gets or sets the sortable flag of a column.
    /// </summary>
    /// <value>
    /// The sortable.
    /// </value>
    [Newtonsoft.Json.JsonProperty("sortable")]
    [JsonPropertyName("sortable")]
    public bool? Sortable { get; set; }

    /// <summary>
    /// Gets or sets the sort order of a column, 
    /// e.g. in which position this column is sorted by default in the grid.
    /// Only applies to grids, not forms.
    /// </summary>
    /// <value>
    /// The sort order.
    /// </value>
    [Newtonsoft.Json.JsonProperty("sortOrder")]
    [JsonPropertyName("sortOrder")]
    public int? SortOrder { get; set; }

    /// <summary>
    /// Gets or sets the grouping order of a column, 
    /// e.g. in which position this column is grouped by default in the grid.
    /// Only applies to grids, not forms.
    /// </summary>
    /// <value>
    /// The sort order.
    /// </value>
    [Newtonsoft.Json.JsonProperty("groupOrder")]
    [JsonPropertyName("groupOrder")]
    public int? GroupOrder { get; set; }


    /// <summary>
    /// Gets or sets the type of the summary. Only applies to grids.
    /// </summary>
    /// <value>
    /// The type of the summary.
    /// </value>
    [Newtonsoft.Json.JsonProperty("summaryType")]
    [JsonPropertyName("summaryType")]
    public SummaryType? SummaryType { get; set; }

    /// <summary>
    /// Gets or sets the edit link flag. Determines if column
    /// should have a edit link to open related dialog.
    /// </summary>
    /// <value>
    /// The edit link.
    /// </value>
    [Newtonsoft.Json.JsonProperty("editLink")]
    [JsonPropertyName("editLink")]
    public bool? EditLink { get; set; }

    /// <summary>
    /// Gets or sets the type of the edit link item type
    /// if it is different than the item type of the grid.
    /// </summary>
    /// <value>
    /// The type of the edit link item.
    /// </value>
    [Newtonsoft.Json.JsonProperty("editLinkItemType")]
    [JsonPropertyName("editLinkItemType")]
    public string? EditLinkItemType { get; set; }

    /// <summary>
    /// Gets or sets the edit link identifier (e.g. ID) field.
    /// If edit link is in CompanyName field, EditLinkId field
    /// might be CompanyId.
    /// </summary>
    /// <value>
    /// The edit link identifier field.
    /// </value>
    [Newtonsoft.Json.JsonProperty("editLinkIdField")]
    [JsonPropertyName("editLinkIdField")]
    public string? EditLinkIdField { get; set; }

    /// <summary>
    /// Gets or sets the edit link CSS class.
    /// </summary>
    /// <value>
    /// The edit link CSS class.
    /// </value>
    [Newtonsoft.Json.JsonProperty("editLinkCssClass")]
    [JsonPropertyName("editLinkCssClass")]
    public string? EditLinkCssClass { get; set; }

    /// <summary>
    /// Gets or sets the type of the filtering for the column.
    /// </summary>
    /// <value>
    /// The type of the filtering.
    /// </value>
    [Newtonsoft.Json.JsonProperty("filteringType")]
    [JsonPropertyName("filteringType")]
    public string? FilteringType { get; set; }


    /// <summary>
    /// Gets or sets the filtering parameters.
    /// </summary>
    /// <value>
    /// The filtering parameters.
    /// </value>
    [Newtonsoft.Json.JsonProperty("filteringParams")]
    [JsonPropertyName("filteringParams")]
    public Dictionary<string, object>? FilteringParams { get; set; }

    /// <summary>
    /// Gets or sets the filtering identifier field.
    /// If the column is CompanyName, its FilteringIdField
    /// might be CompanyId.
    /// </summary>
    /// <value>
    /// The filtering identifier field.
    /// </value>
    [Newtonsoft.Json.JsonProperty("filteringIdField")]
    [JsonPropertyName("filteringIdField")]
    public string? FilteringIdField { get; set; }

    /// <summary>
    /// Gets or sets the filter only flag, e.g. this property
    /// should not be used for forms/grids, it is here
    /// only to set filtering options.
    /// </summary>
    /// <value>
    /// The filter only.
    /// </value>
    [Newtonsoft.Json.JsonProperty("filterOnly")]
    [JsonPropertyName("filterOnly")]
    public bool? FilterOnly { get; set; }

    /// <summary>
    /// Gets or sets the not filterable that disables
    /// filtering by the property in advanced filter dialog.
    /// </summary>
    /// <value>
    /// The not filterable.
    /// </value>
    [Newtonsoft.Json.JsonProperty("notFilterable")]
    [JsonPropertyName("notFilterable")]
    public bool? NotFilterable { get; set; }

    /// <summary>
    /// Gets or sets the quick filter flag.
    /// Only used for columns.
    /// </summary>
    /// <value>
    /// The quick filter.
    /// </value>
    [Newtonsoft.Json.JsonProperty("quickFilter")]
    [JsonPropertyName("quickFilter")]
    public bool? QuickFilter { get; set; }

    /// <summary>
    /// Gets or sets the quick filter parameters.
    /// </summary>
    /// <value>
    /// The quick filter parameters.
    /// </value>
    [Newtonsoft.Json.JsonProperty("quickFilterParams")]
    [JsonPropertyName("quickFilterParams")]
    public Dictionary<string, object>? QuickFilterParams { get; set; }

    /// <summary>
    /// Gets or sets the quick filter separator flag.
    /// </summary>
    /// <value>
    /// The quick filter separator.
    /// </value>
    [Newtonsoft.Json.JsonProperty("quickFilterSeparator")]
    [JsonPropertyName("quickFilterSeparator")]
    public bool? QuickFilterSeparator { get; set; }

    /// <summary>
    /// Gets or sets the quick filter CSS class.
    /// </summary>
    /// <value>
    /// The quick filter CSS class.
    /// </value>
    [Newtonsoft.Json.JsonProperty("quickFilterCssClass")]
    [JsonPropertyName("quickFilterCssClass")]
    public string? QuickFilterCssClass { get; set; }

    /// <summary>
    /// Gets or sets the extension data, used for JSON.NET
    /// serialization / deserialization of dynamic properties.
    /// </summary>
    /// <value>
    /// The extension data.
    /// </value>
    [JsonExtensionData]
    [Newtonsoft.Json.JsonExtensionData]
    public Dictionary<string, object>? ExtensionData { get; set; }

    /// <summary>
    /// Should the EditorType property be serialized by JSON.NET, used to reduce JSON size.
    /// </summary>
    public bool ShouldSerializeEditorType()
    {
        return EditorType != null && EditorType != "String";
    }

    /// <summary>
    /// Should the FilteringType property be serialized by JSON.NET, used to reduce JSON size.
    /// </summary>
    public bool ShouldSerializeFilteringType()
    {
        return FilteringType != null && FilteringType != "String";
    }

    /// <summary>
    /// Should the EditorParams property be serialized by JSON.NET, used to reduce JSON size.
    /// </summary>
    public bool ShouldSerializeEditorParams()
    {
        return EditorParams != null && EditorParams.Count > 0;
    }

    /// <summary>
    /// Should the FormatterParams property be serialized by JSON.NET, used to reduce JSON size.
    /// </summary>
    public bool ShouldSerializeFormatterParams()
    {
        return FormatterParams != null && FormatterParams.Count > 0;
    }

    /// <summary>
    /// Should the FilteringParams property be serialized by JSON.NET, used to reduce JSON size.
    /// </summary>
    public bool ShouldSerializeFilteringParams()
    {
        return FilteringParams != null && FilteringParams.Count > 0;
    }

    /// <summary>
    /// Should the QuickFilterParams property be serialized by JSON.NET, used to reduce JSON size.
    /// </summary>
    public bool ShouldSerializeQuickFilterParams()
    {
        return QuickFilterParams != null && QuickFilterParams.Count > 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyItem"/> class.
    /// </summary>
    public PropertyItem()
    {
        EditorParams = [];
        FormatterParams = [];
        FilteringParams = [];
        QuickFilterParams = [];
        ExtensionData = [];
    }
}