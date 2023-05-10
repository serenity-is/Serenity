namespace Serenity.ComponentModel;

/// <summary>
/// Base class for lookup based editor types
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public abstract class LookupEditorBaseAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LookupEditorBaseAttribute"/> class.
    /// </summary>
    /// <param name="editorType">Type of the editor.</param>
    protected LookupEditorBaseAttribute(string editorType)
        : base(editorType)
    {
    }

    /// <summary>
    /// Lookup key, e.g. Northwind.CustomerCity
    /// </summary>
    public string? LookupKey
    {
        get { return GetOption<string>("lookupKey"); }
        set { SetOption("lookupKey", value); }
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
    /// is determined by LookupKey, e.g. if lookup key is "Northwind.CustomerCity", 
    /// a dialog class named "Northwind.CustomerCityDialog" is used. If dialog type is different
    /// than lookup key, set this to classname, e.g. "MyModule.MyDialog"
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
    /// Make sure you have [LookupInclude] attribute on this field of lookup row,
    /// otherwise you'll have empty results as this field won't be available client side.
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
        get { return GetOption<string>("cascadeValue"); }
        set { SetOption("cascadeValue", value); }
    }

    /// <summary>
    /// Optional filtering field (items will be filtered on this key, e.g. GroupID)
    /// Make sure you have [LookupInclude] attribute on this field of lookup row,
    /// otherwise you'll have empty results as this field won't be available client side.
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
        get { return GetOption<object?>("filterValue"); }
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
    /// Load lookup in async mode, default false
    /// </summary>
    public bool Async
    {
        get { return GetOption<bool>("async"); }
        set { SetOption("async", value); }
    }
}