using System.Diagnostics.CodeAnalysis;

namespace Serenity.ComponentModel;

/// <summary>
/// Base class which other custom filtering types derive from.
/// </summary>
/// <seealso cref="FilteringTypeAttribute" />
public abstract class CustomFilteringAttribute : FilteringTypeAttribute
{
    private Dictionary<string, object?>? options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomFilteringAttribute"/> class.
    /// </summary>
    /// <param name="filteringType">Type of the filtering.</param>
    public CustomFilteringAttribute(string filteringType)
        : base(filteringType)
    {
    }

    /// <summary>
    /// Sets the parameters.
    /// </summary>
    /// <param name="filteringParams">The filtering parameters.</param>
    public override void SetParams(IDictionary<string, object?> filteringParams)
    {
        if (filteringParams is null)
            throw new ArgumentNullException(nameof(filteringParams));

        if (options != null)
            foreach (var opt in options)
                filteringParams[opt.Key] = opt.Value;
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
    /// Gets the option.
    /// </summary>
    /// <typeparam name="TType">The type of the value.</typeparam>
    /// <param name="key">The key.</param>
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
