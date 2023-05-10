namespace Serenity.ComponentModel;

/// <summary>
/// Base class for service lookup based editor types
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public abstract class ServiceLookupEditorBaseAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLookupEditorBaseAttribute"/> class.
    /// </summary>
    /// <param name="editorType">Type of the editor.</param>
    protected ServiceLookupEditorBaseAttribute(string editorType)
        : base(editorType)
    {
    }

    /// <summary>
    /// List service url, e.g. Northwind/Customer/List.
    /// It can be an absolute or relative URL to ~/Services
    /// </summary>
    public string? Service
    {
        get { return GetOption<string>("service"); }
        set { SetOption("service", value); }
    }

    /// <summary>
    /// Name of the id field
    /// </summary>
    public string? IdField
    {
        get { return GetOption<string>("idField"); }
        set { SetOption("idField", value); }
    }

    /// <summary>
    /// Name of the text field
    /// </summary>
    public string? TextField
    {
        get { return GetOption<string>("idField"); }
        set { SetOption("textField", value); }
    }

    /// <summary>
    /// Page size used while loading records, default 100
    /// </summary>
    public int? PageSize
    {
        get { return GetOption<int>("pageSize"); }
        set { SetOption("pageSize", value); }
    }

    /// <summary>
    /// Sort order for records, defaults to text field
    /// </summary>
    public object? Sort
    {
        get { return GetOption<string[]>("sort"); }
        set { SetOption("sort", value is string s ? new string[] { s } : value); }
    }

    /// <summary>
    /// Column selection, defaults to KeyOnly
    /// </summary>
    public ColumnSelection ColumnSelection
    {
        get { return GetOption<ColumnSelection>("columnSelection"); }
        set { SetOption("columnSelection", value); }
    }

    /// <summary>
    /// Include columns list, defaults to id, text, and [LookupInclude] columns.
    /// </summary>
    public string[]? IncludeColumns
    {
        get { return GetOption<string[]>("includeColumns"); }
        set { SetOption("includeColumns", value); }
    }

    /// <summary>
    /// Exclude columns list
    /// </summary>
    public string[]? ExcludeColumns
    {
        get { return GetOption<string[]>("excludeColumns"); }
        set { SetOption("excludeColumns", value); }
    }

    /// <summary>
    /// Include deleted, defaults to false
    /// </summary>
    public string[]? IncludeDeleted
    {
        get { return GetOption<string[]>("includeDeleted"); }
        set { SetOption("includeDeleted", value); }
    }

    /// <summary>
    /// Allows dynamically creating new options from text input by the user in the search box.
    /// This option should only be used for free text inputs, not ID / Text pairs.
    /// When this option is enabled InplaceAdd cannot be used. 
    /// Newly created option will have same ID / Text which is user entered text.
    /// </summary>
    public bool AutoComplete
    {
        get { return GetOption<bool>("autoComplete"); }
        set { SetOption("autoComplete", value); }
    }

    /// <summary>
    /// Enable inplace add / edit functionality
    /// </summary>
    public bool InplaceAdd
    {
        get { return GetOption<bool>("inplaceAdd"); }
        set { SetOption("inplaceAdd", value); }
    }

    /// <summary>
    /// Permission required to use inplace add / edit
    /// </summary>
    public string? InplaceAddPermission
    {
        get { return GetOption<string>("inplaceAddPermission"); }
        set { SetOption("inplaceAddPermission", value); }
    }

    /// <summary>
    /// This property is meaningful when InplaceAdd is true. By default, dialog type name
    /// is determined by service, e.g. if service is "Northwind/CustomerCity/List", 
    /// a dialog class named "Northwind.CustomerCityDialog" is used. If dialog type is different
    /// than the service, set this to classname, e.g. "MyModule.MyDialog"
    /// </summary>
    public string? DialogType
    {
        get { return GetOption<string>("dialogType"); }
        set { SetOption("dialogType", value); }
    }

    /// <summary>
    /// ID (can be relative) of the editor that this editor will cascade from, e.g. Country
    /// </summary>
    public string? CascadeFrom
    {
        get { return GetOption<string>("cascadeFrom"); }
        set { SetOption("cascadeFrom", value); }
    }

    /// <summary>
    /// Cascade filtering field (items will be filtered on this key, e.g. CountryID)
    /// </summary>
    public string? CascadeField
    {
        get { return GetOption<string>("cascadeField"); }
        set { SetOption("cascadeField", value); }
    }

    /// <summary>
    /// Cascade filtering value, usually set by CascadeFrom editor, e.g. the integer value of CountryID
    /// If null or empty, and CascadeField is set, all items are filtered
    /// </summary>
    public object? CascadeValue
    {
        get { return GetOption<object>("cascadeValue"); }
        set { SetOption("cascadeValue", value); }
    }

    /// <summary>
    /// Optional filtering field (items will be filtered on this key, e.g. GroupID)
    /// </summary>
    public string? FilterField
    {
        get { return GetOption<string>("filterField"); }
        set { SetOption("filterField", value); }
    }

    /// <summary>
    /// Optional filtering value, e.g. the integer value of GroupID. If null or empty string no filtering occurs.
    /// </summary>
    public object? FilterValue
    {
        get { return GetOption<object>("filterValue"); }
        set { SetOption("filterValue", value); }
    }

    /// <summary>
    /// The minimum number of results that must be initially (after opening the dropdown for the first time) populated in order to keep the search field. 
    /// This is useful for cases where local data is used with just a few results, in which case the search box is not very useful and wastes screen space.
    /// The option can be set to a negative value to permanently hide the search field.
    /// </summary>
    public int MinimumResultsForSearch
    {
        get { return GetOption<int>("minimumResultsForSearch"); }
        set { SetOption("minimumResultsForSearch", value); }
    }

    /// <summary>
    /// Allow multiple selection. Make sure your field is a List. 
    /// You may also set CommaSeparated to use a string field.
    /// </summary>
    public bool Multiple
    {
        get { return GetOption<bool>("multiple"); }
        set { SetOption("multiple", value); }
    }

    /// <summary>
    /// Use comma separated string instead of an array to serialize values.
    /// </summary>
    public bool Delimited
    {
        get { return GetOption<bool>("delimited"); }
        set { SetOption("delimited", value); }
    }

    /// <summary>
    /// Open dialogs as panel (default value is null, which uses panel attribute on dialog class)
    /// </summary>
    public bool OpenDialogAsPanel
    {
        get { return GetOption<bool>("openDialogAsPanel"); }
        set { SetOption("openDialogAsPanel", value); }
    }

    /// <summary>
    /// Gets/sets row type related with this service lookup editor
    /// </summary>
    public Type? ItemType { get; set; }
}