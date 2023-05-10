namespace Serenity.ComponentModel;

/// <summary>
/// Sets editor type of the target property.
/// </summary>
/// <seealso cref="Attribute" />
public class EditorTypeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditorTypeAttribute"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public EditorTypeAttribute(string type)
    {
        EditorType = type;
    }

    /// <summary>
    /// Transfers the current editor parameters to specified editorParams dictionary.
    /// </summary>
    /// <param name="editorParams">The editor parameters.</param>
    public virtual void SetParams(IDictionary<string, object?> editorParams)
    {
    }

    /// <summary>
    /// Gets the type of the editor.
    /// </summary>
    /// <value>
    /// The type of the editor.
    /// </value>
    public string EditorType { get; private set; }
}