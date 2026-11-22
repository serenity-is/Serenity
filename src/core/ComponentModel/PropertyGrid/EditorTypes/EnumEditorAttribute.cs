namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "Enum" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class EnumEditorAttribute : CustomEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "Enum";

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumEditorAttribute"/> class.
    /// </summary>
    public EnumEditorAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumEditorAttribute"/> class for a numeric enum or a
    /// <see cref="StringEnum"/> type, which is needed for a property that is not typed with it, e.g. a string
    /// property that holds the values of a <see cref="StringEnum"/>.
    /// </summary>
    /// <param name="enumType">The enum or string enumeration type.</param>
    /// <exception cref="ArgumentNullException"><paramref name="enumType"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="enumType"/> is neither an enum nor a string enumeration.</exception>
    public EnumEditorAttribute(Type enumType)
        : base(Key)
    {
        if (enumType == null)
            throw new ArgumentNullException(nameof(enumType));

        if (!enumType.IsEnum && !StringEnum.IsStringEnum(enumType))
            throw new ArgumentOutOfRangeException(nameof(enumType),
                $"{enumType.FullName} is neither an enum nor a {nameof(StringEnum)} type!");

        EnumKey = EnumMapper.GetEnumTypeKey(enumType);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumEditorAttribute"/> class for an enum or a
    /// <see cref="StringEnum"/> type by its key, which is needed for a property that is not typed with it, e.g.
    /// a string property that holds the values of a <see cref="StringEnum"/>.
    /// </summary>
    /// <param name="enumKey">The key of the enum or string enumeration.</param>
    /// <exception cref="ArgumentNullException"><paramref name="enumKey"/> is null or empty.</exception>
    public EnumEditorAttribute(string enumKey)
        : base(Key)
    {
        if (string.IsNullOrEmpty(enumKey))
            throw new ArgumentNullException(nameof(enumKey));

        EnumKey = enumKey;
    }

    /// <summary>
    /// Gets or sets the key of the enum or string enumeration, which is determined from the type of the property
    /// when it is typed with one.
    /// </summary>
    public string? EnumKey
    {
        get { return GetOption<string>("enumKey"); }
        set { SetOption("enumKey", value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to allow clearing.
    /// </summary>
    /// <value>
    ///   <c>true</c> if allow clear; otherwise, <c>false</c>.
    /// </value>
    public bool AllowClear
    {
        get { return GetOption<bool>("allowClear"); }
        set { SetOption("allowClear", value); }
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
    /// Indicates whether the enum should be treated as a flags enum, allowing multiple selections.
    /// </summary>
    public bool Flags
    {
        get { return GetOption<bool>("flags"); }
        set { SetOption("flags", value); }
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
    /// Shows a multiple editor as a checklist, see <see cref="LookupEditorBaseAttribute.CheckList"/>.
    /// </summary>
    public bool CheckList
    {
        get { return GetOption<bool>("checkList"); }
        set { SetOption("checkList", value); }
    }

    /// <summary>
    /// The icon of the placeholder, see <see cref="LookupEditorBaseAttribute.PlaceholderIcon"/>.
    /// </summary>
    public string? PlaceholderIcon
    {
        get { return GetOption<string>("placeholderIcon"); }
        set { SetOption("placeholderIcon", value); }
    }

    /// <summary>
    /// Whether a checklist, see <see cref="CheckList"/>, has an item that selects all the members, or clears them when
    /// they are all selected, see <see cref="LookupEditorBaseAttribute.SelectAll"/>. Default is true.
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
    /// How many of the selected members the summary of a checklist, see <see cref="CheckList"/>, names before it counts
    /// the rest, see <see cref="LookupEditorBaseAttribute.MaxSummaryItems"/>, of which <see cref="SummaryCountOnly"/>
    /// names none and shows only how many are selected, and <see cref="SummaryNamesAll"/> names them all, which is what
    /// ComboboxEditor.defaults.maxSummaryItems, that the editor uses when it is not set, is by default. The other negative
    /// values are reserved for summaries to come.
    /// </summary>
    public int MaxSummaryItems
    {
        get { return GetOption<int>("maxSummaryItems"); }
        set { SetOption("maxSummaryItems", value); }
    }
}
