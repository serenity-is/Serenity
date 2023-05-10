namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "CheckLookup" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class CheckLookupEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "CheckLookup";

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckLookupEditorAttribute"/> class.
    /// </summary>
    /// <param name="lookupKey">The lookup key.</param>
    public CheckLookupEditorAttribute(string lookupKey)
        : base(Key)
    {
        SetOption("lookupKey", lookupKey);
    }

    /// <summary>
    /// If you use this constructor, lookupKey will be determined by [LookupScript] attribute
    /// on specified lookup type. If this is a row type, make sure it has [LookupScript] attribute
    /// on it.
    /// </summary>
    public CheckLookupEditorAttribute(Type lookupType)
        : base(Key)
    {
        if (lookupType == null)
            throw new ArgumentNullException("lookupType");

        var attr = lookupType.GetCustomAttribute<LookupScriptAttribute>(false);
        if (attr == null)
        {
            throw new ArgumentException(string.Format(
                "'{0}' type doesn't have a [LookupScript] attribute, so it can't " +
                "be used with a CheckLookupEditor!",
                lookupType.FullName), "lookupType");
        }

        SetOption("lookupKey", attr.Key ??
            LookupScriptAttribute.AutoLookupKeyFor(lookupType));
    }

    /// <summary>
    /// Lookup key, e.g. Northwind.CustomerCity
    /// </summary>
    public string LookupKey
    {
        get { return GetOption<string>("lookupKey")!; }
        set { SetOption("lookupKey", value); }
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
    public object? CascadeField
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
    public object? FilterField
    {
        get { return GetOption<string>("filterField"); }
        set { SetOption("filterField", value); }
    }

    /// <summary>
    /// Optional filtering value, e.g. the integer value of GroupID. If null or empty string no filtering occurs.
    /// </summary>
    public object? FilterValue
    {
        get { return GetOption<string>("filterValue"); }
        set { SetOption("filterValue", value); }
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
    /// Move selected items to top on load
    /// </summary>
    public bool CheckedOnTop
    {
        get { return GetOption<bool>("checkedOnTop"); }
        set { SetOption("checkedOnTop", value); }
    }

    /// <summary>
    /// Show select all button
    /// </summary>
    public bool ShowSelectAll
    {
        get { return GetOption<bool>("showSelectAll"); }
        set { SetOption("showSelectAll", value); }
    }

    /// <summary>
    /// Hide quick search input
    /// </summary>
    public bool HideSearch
    {
        get { return GetOption<bool>("hideSearch"); }
        set { SetOption("hideSearch", value); }
    }
}