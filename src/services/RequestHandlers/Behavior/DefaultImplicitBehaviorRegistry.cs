using Microsoft.Extensions.Primitives;

namespace Serenity.Services;

/// <summary>
/// Default implementation for the <see cref="IImplicitBehaviorRegistry"/>
/// </summary>
/// <remarks>
/// Scans the <see cref="ITypeSource"/> lazily on first use and caches the concrete
/// <see cref="IImplicitBehavior"/> types. When the type source also implements
/// <see cref="IChangeTokenProvider"/>, the cache is reset whenever its change token fires,
/// e.g. when dynamic assemblies or feature toggles change, and rebuilt on next use. A failed
/// scan is not cached, so a later call retries it.
/// </remarks>
public class DefaultImplicitBehaviorRegistry : IImplicitBehaviorRegistry, IDisposable
{
    private readonly ITypeSource typeSource;
    private readonly object behaviorTypesLock = new();
    private volatile Type[]? behaviorTypes;
    private IDisposable? changeSubscription;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="typeSource">The type source to extract <see cref="IImplicitBehavior"/> types from</param>
    /// <exception cref="ArgumentNullException"><paramref name="typeSource"/> is <c>null</c>.</exception>
    public DefaultImplicitBehaviorRegistry(ITypeSource typeSource)
    {
        this.typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));

        if (this.typeSource is IChangeTokenProvider changeTokenProvider)
        {
            changeSubscription = ChangeToken.OnChange(changeTokenProvider.GetChangeToken, ResetBehaviorTypes);
        }
    }

    private Type[] GetBehaviorTypes()
    {
        var types = behaviorTypes;
        if (types is not null)
            return types;

        lock (behaviorTypesLock)
        {
            types = behaviorTypes;
            if (types is not null)
                return types;

            types = BuildBehaviorTypes();
            behaviorTypes = types;
            return types;
        }
    }

    private void ResetBehaviorTypes()
    {
        lock (behaviorTypesLock)
        {
            behaviorTypes = null;
        }
    }

    private Type[] BuildBehaviorTypes()
    {
        return [.. typeSource.GetTypesWithInterface(typeof(IImplicitBehavior))
            .Where(type => !type.IsAbstract && !type.IsInterface)];
    }

    /// <inheritdoc/>
    public IEnumerable<Type> GetTypes()
    {
        return GetBehaviorTypes();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        changeSubscription?.Dispose();
        changeSubscription = null;
    }
}
