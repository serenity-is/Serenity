namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "Lookup" editor.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class LookupEditorAttribute : LookupEditorBaseAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public const string Key = "Lookup";

    /// <summary>
    /// Initializes a new instance of the <see cref="LookupEditorAttribute"/> class.
    /// </summary>
    /// <param name="lookupKey">The lookup key.</param>
    public LookupEditorAttribute(string lookupKey)
        : base(Key)
    {
        SetOption("lookupKey", lookupKey);
    }

    /// <summary>
    /// If you use this constructor, lookupKey will be determined by [LookupScript] attribute
    /// on specified lookup type. If this is a row type, make sure it has [LookupScript] attribute
    /// on it.
    /// </summary>
    public LookupEditorAttribute(Type lookupType)
        : base(Key)
    {
        if (lookupType == null)
            throw new ArgumentNullException("lookupType");

        var attr = lookupType.GetCustomAttribute<LookupScriptAttribute>(false);
        if (attr == null)
        {
            throw new ArgumentException(string.Format(
                "'{0}' type doesn't have a [LookupScript] attribute, so it can't " +
                "be used with a LookupEditor!",
                lookupType.FullName), "lookupType");
        }

        SetOption("lookupKey", attr.Key ??
            LookupScriptAttribute.AutoLookupKeyFor(lookupType));
    }
}