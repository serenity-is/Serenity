using System.Diagnostics.CodeAnalysis;

namespace Serenity.ComponentModel;

/// <summary>
/// Base class for custom formatter type attributes
/// </summary>
/// <seealso cref="FormatterTypeAttribute" />
public abstract class CustomFormatterAttribute : FormatterTypeAttribute
{
    private Dictionary<string, object?>? options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomFormatterAttribute"/> class.
    /// </summary>
    /// <param name="formatterType">Type of the formatter.</param>
    public CustomFormatterAttribute(string formatterType)
        : base(formatterType)
    {
    }

    /// <summary>
    /// Sets the parameters for formatter.
    /// </summary>
    /// <param name="formatterParams">The formatter parameters.</param>
    public override void SetParams(IDictionary<string, object?> formatterParams)
    {
        if (formatterParams is null)
            throw new ArgumentNullException(nameof(formatterParams));

        if (options != null)
            foreach (var opt in options)
                formatterParams[opt.Key] = opt.Value;
    }

    /// <summary>
    /// Sets the option.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    protected void SetOption(string key, object? value)
    {
        options ??= new(StringComparer.Ordinal);
        options[key] = value;
    }

    /// <summary>
    /// Gets value of an option.
    /// </summary>
    /// <typeparam name="TType">The type of the option.</typeparam>
    /// <param name="key">The option key.</param>
    /// <returns>Option value</returns>
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
