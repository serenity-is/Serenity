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
}