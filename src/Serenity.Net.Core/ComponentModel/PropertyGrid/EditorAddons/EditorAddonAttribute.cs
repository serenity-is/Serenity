using System.Diagnostics.CodeAnalysis;

namespace Serenity.ComponentModel;

/// <summary>
/// Adds an editor addon to the target property.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="EditorAddonAttribute"/> class.
/// </remarks>
/// <param name="type">The type.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class EditorAddonAttribute(string type) : Attribute
{
    private Dictionary<string, object?>? options;

    /// <summary>
    /// Sets the parameters for each pair in the editorParams dictionary.
    /// </summary>
    /// <param name="editorParams">The editor parameters.</param>
    public virtual void SetParams(IDictionary<string, object?> editorParams)
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
    [return: MaybeNull]
    protected TType GetOption<TType>(string key)
    {
        if (options == null)
            return default;

        if (!options.TryGetValue(key, out object? obj) || obj == null)
            return default;

        return (TType)obj;
    }

    /// <summary>
    /// Gets the type of the editor.
    /// </summary>
    /// <value>
    /// The type of the editor.
    /// </value>
    public string AddonType { get; private set; } = type ?? throw new ArgumentNullException(nameof(type));

    /// <summary>
    /// Gets or sets the addon key which is set as data-addonkey attribute.
    /// </summary>
    public string? AddonKey
    {
        get => GetOption<string?>("addonKey");
        set => SetOption("addonKey", value);
    }
}