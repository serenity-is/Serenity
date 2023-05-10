using System.Diagnostics.CodeAnalysis;

namespace Serenity.ComponentModel;

/// <summary>
/// Base attribute class that all other custom editor types
/// derives from.
/// </summary>
/// <seealso cref="EditorTypeAttribute" />
public abstract class CustomEditorAttribute : EditorTypeAttribute
{
    private Dictionary<string, object?>? options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomEditorAttribute"/> class.
    /// </summary>
    /// <param name="editorType">Type of the editor.</param>
    public CustomEditorAttribute(string editorType)
        : base(editorType)
    {
    }

    /// <summary>
    /// Sets the parameters for each pair in the editorParams dictionary.
    /// </summary>
    /// <param name="editorParams">The editor parameters.</param>
    public override void SetParams(IDictionary<string, object?> editorParams)
    {
        if (editorParams is null)
            throw new ArgumentNullException(nameof(editorParams));

        if (options != null)
            foreach (var opt in options)
                editorParams[opt.Key] = opt.Value;
    }

    /// <summary>
    /// Sets the editor option.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    protected void SetOption(string key, object? value)
    {
        options ??= new(StringComparer.Ordinal);
        options[key] = value;
    }

    /// <summary>
    /// Gets the editor option.
    /// </summary>
    /// <typeparam name="TType">The type of the type.</typeparam>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    [return:MaybeNull]
    protected TType GetOption<TType>(string key)
    {
        if (options == null)
            return default;

        if (!options.TryGetValue(key, out object? obj) || obj == null)
            return default;

        return (TType)obj;
    }
}