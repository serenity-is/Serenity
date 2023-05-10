using System.Diagnostics.CodeAnalysis;

namespace Serenity;

/// <summary>
/// Basic collection extensions
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Adds multiple items to a list.
    /// </summary>
    /// <typeparam name="T">Type of items</typeparam>
    /// <param name="list">The list.</param>
    /// <param name="values">The values.</param>
    public static void AddRange<T>(this ICollection<T> list, params T[] values)
    {
        if (list is null)
            throw new ArgumentException(nameof(list));

        foreach (T value in values)
            list.Add(value);
    }

    /// <summary>
    /// Adds multiple items to a list.
    /// </summary>
    /// <typeparam name="T">Type of items</typeparam>
    /// <param name="list">The list.</param>
    /// <param name="values">The values.</param>
    public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> values)
    {
        if (list is null)
            throw new ArgumentException(nameof(list));

        foreach (T value in values)
            list.Add(value);
    }

    /// <summary>
    /// Gets the specified key or returns default value of TValue if not found.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dict">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <returns>Value for specified key, or default value if not found.</returns>
    [return: MaybeNull]
    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
    {
        if (dict is null)
            throw new ArgumentException(nameof(dict));

        if (!dict.TryGetValue(key, out TValue value))
            return default;

        return value;
    }

    /// <summary>
    /// Gets the specified key or returns defaultValue if not found.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dict">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>
    /// Value for specified key, or default value if not found.
    /// </returns>
    [return: MaybeNull]
    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
    {
        if (dict is null)
            throw new ArgumentException(nameof(dict));

        if (!dict.TryGetValue(key, out TValue value))
            return defaultValue;

        return value;
    }
}