namespace Serenity.ComponentModel;

/// <summary>
/// Base class for service lookup based editor types.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="ServiceLookupEditorBaseAttribute"/> class.
/// </remarks>
/// <param name="editorType">Type of the editor.</param>
public abstract class ServiceLookupEditorBaseAttribute(string editorType) : CustomEditorAttribute(editorType)
{

    /// <summary>
    /// List service URL, e.g. Northwind/Customer/List.
    /// It can be an absolute or relative URL to ~/Services.
    /// </summary>
    public string? Service
    {
        get { return GetOption<string>("service"); }
        set { SetOption("service", value); }
    }

    /// <summary>
    /// Name of the ID field.
    /// </summary>
    public string? IdField
    {
        get { return GetOption<string>("idField"); }
        set { SetOption("idField", value); }
    }

    /// <summary>
    /// Name of the text field.
    /// </summary>
    public string? TextField
    {
        get { return GetOption<string>("textField"); }
        set { SetOption("textField", value); }
    }

    /// <summary>
    /// Name of the icon field.
    /// </summary>
    public string? IconField
    {
        get { return GetOption<string>("iconField"); }
        set { SetOption("iconField", value); }
    }


    /// <summary>
    /// Page size used while loading records, default 100. A value of zero or less leaves the page size to the editor,
    /// removing one that was set before, as a nullable property is not a valid argument of an attribute.
    /// </summary>
    public int PageSize
    {
        get { return GetOption<int>("pageSize"); }
        set
        {
            if (value > 0)
                SetOption("pageSize", value);
            else
                RemoveOption("pageSize");
        }
    }

    /// <summary>
    /// Sort order for records, defaults to the text field.
    /// </summary>
    [EditorType("SelectEditor"), EditorOption("multiple", true), EditorOption("allowClear", true), EditorOption("delimited", false), EditorOption("autoComplete", true)]
    public string[]? Sort
    {
        get { return GetOption<string[]>("sort"); }
        set { SetOption("sort", value); }
    }

    /// <summary>
    /// Column selection, defaults to KeyOnly.
    /// </summary>
    public ColumnSelection ColumnSelection
    {
        get { return GetOption<ColumnSelection>("columnSelection"); }
        set { SetOption("columnSelection", value); }
    }

    /// <summary>
    /// Include columns list, defaults to ID, text, and [LookupInclude] columns.
    /// </summary>
    [EditorType("SelectEditor"), EditorOption("multiple", true), EditorOption("allowClear", true), EditorOption("delimited", false), EditorOption("autoComplete", true)]
    public string[]? IncludeColumns
    {
        get { return GetOption<string[]>("includeColumns"); }
        set { SetOption("includeColumns", value); }
    }

    /// <summary>
    /// Exclude columns list.
    /// </summary>
    [EditorType("SelectEditor"), EditorOption("multiple", true), EditorOption("allowClear", true), EditorOption("delimited", false), EditorOption("autoComplete", true)]
    public string[]? ExcludeColumns
    {
        get { return GetOption<string[]>("excludeColumns"); }
        set { SetOption("excludeColumns", value); }
    }

    /// <summary>
    /// Quick search fields list.
    /// </summary>
    [EditorType("SelectEditor"), EditorOption("multiple", true), EditorOption("allowClear", true), EditorOption("delimited", false), EditorOption("autoComplete", true)]
    public string[]? QuickSearchFields
    {
        get { return GetOption<string[]>("quickSearchFields"); }
        set { SetOption("quickSearchFields", value); }
    }

    /// <summary>
    /// Include deleted, defaults to false. Only effective when the current
    /// user has the undelete permission for the target entity.
    /// </summary>
    public bool IncludeDeleted
    {
        get { return GetOption<bool>("includeDeleted"); }
        set { SetOption("includeDeleted", value); }
    }

    /// <summary>
    /// Allows dynamically creating new options from text input by the user in the search box.
    /// This option should only be used for free text inputs, not ID / Text pairs.
    /// When this option is enabled, InplaceAdd cannot be used.
    /// A newly created option will have the same ID / Text as the user-entered text.
    /// </summary>
    public bool AutoComplete
    {
        get { return GetOption<bool>("autoComplete"); }
        set { SetOption("autoComplete", value); }
    }

    /// <summary>Gets or sets the <c>containsField</c> option.</summary>
    public string? ContainsField
    {
        get { return GetOption<string?>("containsField"); }
        set { SetOption("containsField", value); }
    }

    /// <summary>
    /// Enables inplace add / edit functionality.
    /// </summary>
    public bool InplaceAdd
    {
        get { return GetOption<bool>("inplaceAdd"); }
        set { SetOption("inplaceAdd", value); }
    }

    /// <summary>
    /// Permission required to use inplace add / edit.
    /// </summary>
    public string? InplaceAddPermission
    {
        get { return GetOption<string>("inplaceAddPermission"); }
        set { SetOption("inplaceAddPermission", value); }
    }

    /// <summary>
    /// This property is meaningful when InplaceAdd is true. By default, the dialog type name
    /// is determined by the service, e.g. if the service is "Northwind/CustomerCity/List",
    /// a dialog class named "Northwind.CustomerCityDialog" is used. If the dialog type is different
    /// than the service, set this to the class name, e.g. "MyModule.MyDialog".
    /// </summary>
    public string? DialogType
    {
        get { return GetOption<string>("dialogType"); }
        set { SetOption("dialogType", value); }
    }

    /// <summary>
    /// ID (can be relative) of the editor that this editor will cascade from, e.g. Country.
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
    /// Cascade filtering value, usually set by the CascadeFrom editor, e.g. the integer value of CountryID.
    /// If null or empty, and CascadeField is set, all items are filtered.
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
    /// The value of <see cref="MinimumResultsForSearch"/> that shows the search field for any number of results, see
    /// <see cref="LookupEditorBaseAttribute.AlwaysShowSearch"/>.
    /// </summary>
    public const int AlwaysShowSearch = LookupEditorBaseAttribute.AlwaysShowSearch;

    /// <summary>
    /// The value of <see cref="MinimumResultsForSearch"/> that hides the search field permanently, see
    /// <see cref="LookupEditorBaseAttribute.NeverShowSearch"/>.
    /// </summary>
    public const int NeverShowSearch = LookupEditorBaseAttribute.NeverShowSearch;

    /// <summary>
    /// The minimum number of results that must be initially (after opening the dropdown for the first time) populated in
    /// order to keep the search field. This is useful for cases where local data is used with just a few results, in
    /// which case the search box is not very useful and wastes screen space. <see cref="AlwaysShowSearch"/> shows it for
    /// any number of results, which is what the editor does when it is not set, unless
    /// ComboboxEditor.defaults.minimumResultsForSearch says otherwise, and <see cref="NeverShowSearch"/>, or any other
    /// negative value, hides it permanently.
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
    /// Opens dialogs as a panel (default value is null, which uses the panel attribute on the dialog class).
    /// </summary>
    public bool OpenDialogAsPanel
    {
        get { return GetOption<bool>("openDialogAsPanel"); }
        set { SetOption("openDialogAsPanel", value); }
    }

    /// <summary>
    /// Gets or sets the item/row type passed to the constructor. 
    /// </summary>
    public Type? ItemType { get; set; }

    /// <summary>
    /// Gets or sets the endpoint type. If this is set, the service URL will be 
    /// determined from the endpoint's route using <see cref="ActionName"/>
    /// method, or List or ListLookup actions if ActionName is not set.
    /// </summary>
    public Type? EndpointType { get; set; }

    /// <summary>
    /// Gets / sets the action name, e.g. List / ListLookup etc. to use.
    /// If this is null, the service will be determined from the endpoint's
    /// ListLookup or List actions, or defaults to "List" method if no endpoint is available.
    /// </summary>
    public string? ActionName { get; set; }

    /// <summary>
    /// Gets or sets the supported ListRequest capabilities. When not explicitly set,
    /// capabilities are inferred from the endpoint when possible.
    /// </summary>
    [EnumEditor(Multiple = true, Flags = true)]
    public ListRequestCapabilities Capabilities
    {
        get { return GetOption<ListRequestCapabilities>("capabilities"); }
        set { SetOption("capabilities", value); }
    }

    /// <summary>
    /// Shows a multiple editor as a checklist, instead of a combobox with a choice for each selected item: the results
    /// have check boxes, of which the ones that are selected stay in the list, the first item selects all the results
    /// that are shown, or clears them, and the selection is shown as one summary, e.g. "A, B, C +2". It has no effect on
    /// a single editor.
    /// </summary>
    public bool CheckList
    {
        get { return GetOption<bool>("checkList"); }
        set { SetOption("checkList", value); }
    }

    /// <summary>
    /// The icon of the placeholder, e.g. the icon of the field or of the filter, which the editor shows when nothing is
    /// selected.
    /// </summary>
    public string? PlaceholderIcon
    {
        get { return GetOption<string>("placeholderIcon"); }
        set { SetOption("placeholderIcon", value); }
    }

    /// <summary>
    /// Whether a checklist, see <see cref="CheckList"/>, has an item that selects all the results that are shown, or
    /// clears them when they are all selected. Default is true. Its text tells which: "Select all", "Select all matching"
    /// when something is typed, or "Select shown" when there are more results than the ones loaded, e.g. of an
    /// asynchronous lookup, whose results come in pages.
    /// </summary>
    public bool SelectAll
    {
        get { return GetOption<bool>("selectAll"); }
        set { SetOption("selectAll", value); }
    }

    /// <summary>
    /// The value of <see cref="MaxSummaryItems"/> that names all the selected items, see
    /// <see cref="LookupEditorBaseAttribute.SummaryNamesAll"/>.
    /// </summary>
    public const int SummaryNamesAll = LookupEditorBaseAttribute.SummaryNamesAll;

    /// <summary>
    /// The value of <see cref="MaxSummaryItems"/> that names none of the selected items, and shows only how many are
    /// selected, see <see cref="LookupEditorBaseAttribute.SummaryCountOnly"/>.
    /// </summary>
    public const int SummaryCountOnly = LookupEditorBaseAttribute.SummaryCountOnly;

    /// <summary>
    /// How many of the selected items the summary of a checklist, see <see cref="CheckList"/>, names before it counts the
    /// rest, e.g. 3 for "A, B, C +2". <see cref="SummaryCountOnly"/> names none of them, and shows how many are selected,
    /// e.g. "4 selected", and <see cref="SummaryNamesAll"/> names them all, and shows how many are selected at the end,
    /// e.g. "A, B, C, D (4)", of which the styles cut what does not fit. When it is not set, the editor uses
    /// ComboboxEditor.defaults.maxSummaryItems, which is <see cref="SummaryNamesAll"/> unless the application changes it.
    /// The other negative values are reserved for summaries to come.
    /// </summary>
    public int MaxSummaryItems
    {
        get { return GetOption<int>("maxSummaryItems"); }
        set { SetOption("maxSummaryItems", value); }
    }

    /// <summary>
    /// Whether the selection can be cleared. When it is not set, the editor allows it when it has a placeholder, e.g. an
    /// empty item text, and not otherwise. It is a plain boolean, as a nullable one is not a valid argument of an
    /// attribute, and an option that is not set is told apart by not being in the options.
    /// </summary>
    public bool AllowClear
    {
        get { return GetOption<bool>("allowClear"); }
        set { SetOption("allowClear", value); }
    }

    /// <summary>
    /// The field of an item that holds its local text key, which the editor localizes, or "text" when the text of an
    /// item is its key. When it is not set, the text of an item is shown as it is, as it is data for a lookup.
    /// </summary>
    public string? TextKeyField
    {
        get { return GetOption<string>("textKeyField"); }
        set { SetOption("textKeyField", value); }
    }

    /// <summary>
    /// The field of an item that holds its CSS class, e.g. "text-danger", which the editor adds to the item in its
    /// results.
    /// </summary>
    public string? CssClassField
    {
        get { return GetOption<string>("cssClassField"); }
        set { SetOption("cssClassField", value); }
    }
}
