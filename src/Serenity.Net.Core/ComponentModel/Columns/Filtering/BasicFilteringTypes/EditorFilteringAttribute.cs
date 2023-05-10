namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that property should a custom editor for filtering,
/// which is usually determined by form editor type of the property.
/// </summary>
/// <seealso cref="CustomFilteringAttribute" />
public class EditorFilteringAttribute : CustomFilteringAttribute
{
    /// <summary>
    /// Filtering type key
    /// </summary>
    public const string Key = "Editor";

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorFilteringAttribute"/> class.
    /// </summary>
    public EditorFilteringAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorFilteringAttribute"/> class.
    /// </summary>
    /// <param name="editorType">Type of the editor.</param>
    public EditorFilteringAttribute(string editorType)
        : base(Key)
    {
        EditorType = editorType ?? throw new ArgumentNullException(nameof(editorType));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorFilteringAttribute"/> class.
    /// </summary>
    /// <param name="editorTypeAttribute">The editor type attribute to read editor type from.</param>
    public EditorFilteringAttribute(Type editorTypeAttribute)
        : base(Key)
    {
        EditorType = ((EditorTypeAttribute)Activator.CreateInstance(editorTypeAttribute)).EditorType;
    }

    /// <summary>
    /// Gets or sets the type of the editor.
    /// </summary>
    /// <value>
    /// The type of the editor.
    /// </value>
    public string EditorType
    {
        get { return GetOption<string>("editorType")!; }
        set { SetOption("editorType", value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use relative comparisons, like GT/LT.
    /// </summary>
    /// <value>
    ///   <c>true</c> if should use relative comparisons; otherwise, <c>false</c>.
    /// </value>
    public bool UseRelative
    {
        get { return GetOption<bool>("useRelative"); }
        set { SetOption("useSearch", value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use LIKE kind of operators including
    /// starts with, ends with etc.
    /// </summary>
    /// <value>
    ///   <c>true</c> if should use LIKE operators; otherwise, <c>false</c>.
    /// </value>
    public bool UseLike
    {
        get { return GetOption<bool>("useLike"); }
        set { SetOption("useLike", value); }
    }
}