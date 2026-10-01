using System.Collections.ObjectModel;

namespace Serenity.Data;

/// <summary>
/// Resolves function-call nodes read from criteria JSON.
/// </summary>
public static class FunctionCallCriteriaFactory
{
    private static IReadOnlyDictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>> factories =
        CopyFactories(new Dictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["UPPER"] = arguments => arguments.Length == 1
                ? new UpperFunctionCriteria(arguments[0])
                : null
        });

    private static readonly AsyncLocal<IReadOnlyDictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>?> localFactories = new();

    /// <summary>
    /// Sets the global factory registry, returning the previous snapshot.
    /// By default, supplied entries are merged over the existing factories;
    /// set <paramref name="merge"/> to false to replace the registry. Pass
    /// null with <paramref name="merge"/> false to clear all factories.
    /// The input is copied into an immutable, case-insensitive snapshot. The
    /// previous snapshot is returned and can be passed back to restore it.
    /// </summary>
    /// <param name="factories">New factories, or null when replacing with an empty registry.</param>
    /// <param name="merge">True to merge over the current factories; false to replace them.</param>
    /// <returns>The previous factory snapshot.</returns>
    public static IReadOnlyDictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>> SetFactories(
        IReadOnlyDictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>? factories,
        bool merge = true)
    {
        var incoming = factories is null
            ? new Dictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>(StringComparer.OrdinalIgnoreCase)
            : CopyFactories(factories).ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);

        while (true)
        {
            var current = Volatile.Read(ref FunctionCallCriteriaFactory.factories);
            var next = merge
                ? current.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in incoming)
                next[item.Key] = item.Value;

            IReadOnlyDictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>> snapshot =
                new ReadOnlyDictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>(next);

            if (ReferenceEquals(Interlocked.CompareExchange(ref FunctionCallCriteriaFactory.factories,
                    snapshot, current), current))
                return current;
        }
    }

    /// <summary>
    /// Sets the factory registry for the current async context. When set, it
    /// is exclusive of global factories. By default, the supplied factories
    /// are merged over the current global snapshot. Set <paramref name="merge"/>
    /// to false to use only the supplied entries. Passing null clears the local
    /// registry and restores global lookup, regardless of <paramref name="merge"/>.
    /// The supplied dictionary is copied into a read-only snapshot.
    /// </summary>
    /// <param name="factories">Local factories, or null to return to global factories.</param>
    /// <param name="merge">True to merge with global factories; false to replace them locally.</param>
    /// <returns>The previous local registry, if any.</returns>
    public static IReadOnlyDictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>? SetLocalFactories(
        IReadOnlyDictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>? factories,
        bool merge = true)
    {
        var old = localFactories.Value;
        if (factories is null)
            localFactories.Value = null;
        else if (merge)
        {
            var merged = Volatile.Read(ref FunctionCallCriteriaFactory.factories)
                .ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
            foreach (var item in factories)
                merged[item.Key] = item.Value;
            localFactories.Value = CopyFactories(merged);
        }
        else
            localFactories.Value = CopyFactories(factories);

        return old;
    }

    /// <summary>
    /// Creates a function-call criteria from its JSON name and parsed arguments.
    /// </summary>
    /// <param name="functionName">The serialized function name.</param>
    /// <param name="arguments">The parsed arguments.</param>
    /// <returns>The function criteria, or null when this function isn't supported.</returns>
    public static FunctionCallCriteria? Create(string functionName, BaseCriteria[] arguments)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(functionName);
        ArgumentNullException.ThrowIfNull(arguments);

        return CreateFromRegistry(functionName, arguments);
    }

    private static FunctionCallCriteria? CreateFromRegistry(string functionName, BaseCriteria[] arguments)
    {
        if (localFactories.Value is { } local)
            return local.TryGetValue(functionName, out var localFactory)
                ? localFactory(arguments)
                : null;

        var currentFactories = Volatile.Read(ref factories);
        return currentFactories.TryGetValue(functionName, out var factory) ? factory(arguments) : null;
    }

    private static IReadOnlyDictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>> CopyFactories(
        IReadOnlyDictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>> factories)
    {
        ArgumentNullException.ThrowIfNull(factories);

        var copy = new Dictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>(
            StringComparer.OrdinalIgnoreCase);
        foreach (var item in factories)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(item.Key);
            ArgumentNullException.ThrowIfNull(item.Value);
            copy.Add(item.Key, item.Value);
        }

        return new ReadOnlyDictionary<string, Func<BaseCriteria[], FunctionCallCriteria?>>(copy);
    }
}
