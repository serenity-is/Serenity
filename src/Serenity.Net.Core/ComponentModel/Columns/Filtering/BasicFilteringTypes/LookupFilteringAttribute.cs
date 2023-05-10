namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that property should use lookup editor type of filtering
/// </summary>
/// <seealso cref="CustomFilteringAttribute" />
public partial class LookupFilteringAttribute : CustomFilteringAttribute
{
    /// <summary>
    /// Filtering type key
    /// </summary>
    public const string Key = "Lookup";

    /// <summary>
    /// Initializes a new instance of the <see cref="LookupFilteringAttribute"/> class.
    /// </summary>
    /// <param name="lookupKey">The lookup key.</param>
    public LookupFilteringAttribute(string lookupKey)
        : base(Key)
    {
        SetOption("lookupKey", lookupKey);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LookupFilteringAttribute"/> class.
    /// </summary>
    /// <param name="lookupType">Type of the lookup to get lookup key from. Can be 
    /// a row with [LookupScript] attribute or a custom lookup script.</param>
    /// <exception cref="ArgumentOutOfRangeException">lookupType is null</exception>
    public LookupFilteringAttribute(Type lookupType)
        : base(Key)
    {
        var attr = lookupType.GetCustomAttribute<LookupScriptAttribute>(false);
        if (attr == null)
            throw new ArgumentOutOfRangeException("lookupType");

        SetOption("lookupKey", attr.Key ??
            LookupScriptAttribute.AutoLookupKeyFor(lookupType));
    }

    /// <summary>
    /// Gets or sets the ID field editor should filter on.
    /// </summary>
    /// <value>
    /// The identifier field.
    /// </value>
    public string? IdField
    {
        get { return GetOption<string>("idField"); }
        set { SetOption("idField", value); }
    }
}