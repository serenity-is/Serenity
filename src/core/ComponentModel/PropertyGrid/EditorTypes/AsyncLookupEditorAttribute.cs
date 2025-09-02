namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the target property should use a "Lookup" editor with "async" set to true.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
public partial class AsyncLookupEditorAttribute : LookupEditorAttribute
{
    /// <summary>
    /// Editor type key
    /// </summary>
    public new const string Key = "Lookup";

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLookupEditorAttribute"/> class.
    /// </summary>
    /// <param name="lookupKey">The lookup key.</param>
    public AsyncLookupEditorAttribute(string lookupKey)
        : base(lookupKey)
    {
        SetOption("async", true);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLookupEditorAttribute"/> class.
    /// </summary>
    /// <param name="lookupType">Type of the lookup.</param>
    /// <exception cref="ArgumentNullException">lookupType</exception>
    /// <exception cref="ArgumentException">lookupType</exception>
    public AsyncLookupEditorAttribute(Type lookupType)
        : base(lookupType)
    {
        SetOption("async", true);
    }
}