namespace Serenity.ComponentModel;

/// <summary>
/// Base class for dynamic script attributes.
/// Dynamic scripts contain dynamic data, but they are not parameterized.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public abstract class DynamicScriptAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicScriptAttribute"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    public DynamicScriptAttribute(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>
    /// The key.
    /// </value>
    public string Key { get; private set; }

    /// <summary>
    /// Gets or sets the duration of the caching.
    /// </summary>
    /// <value>
    /// The duration of the caching.
    /// </value>
    public int CacheDuration { get; set; }

    /// <summary>
    /// Gets or sets the cache group key. 
    /// Group keys are used to invalidate a group of items.
    /// </summary>
    /// <value>
    /// The cache group key.
    /// </value>
    public string? CacheGroupKey { get; set; }
}