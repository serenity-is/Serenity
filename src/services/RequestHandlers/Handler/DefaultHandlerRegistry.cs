using Microsoft.Extensions.Primitives;

namespace Serenity.Services;

/// <summary>
/// Default implementation for the <see cref="IDefaultHandlerRegistry"/>.
/// </summary>
/// <remarks>
/// Scans the <see cref="ITypeSource"/> lazily on first use and caches the concrete
/// <see cref="IRequestHandler"/> types. When the type source also implements
/// <see cref="IChangeTokenProvider"/>, the cache is reset whenever its change token fires,
/// e.g. when dynamic assemblies or feature toggles change, and rebuilt on next use. A failed
/// scan is not cached, so a later call retries it.
/// </remarks>
public class DefaultHandlerRegistry : IDefaultHandlerRegistry, IDisposable
{
    private readonly ITypeSource typeSource;
    private readonly object handlerTypesLock = new();
    private volatile Type[]? handlerTypes;
    private IDisposable? changeSubscription;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="typeSource">Type source containing possible handler classes.</param>
    /// <exception cref="ArgumentNullException"><paramref name="typeSource"/> is <c>null</c>.</exception>
    public DefaultHandlerRegistry(ITypeSource typeSource)
    {
        this.typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));

        if (this.typeSource is IChangeTokenProvider changeTokenProvider)
        {
            changeSubscription = ChangeToken.OnChange(changeTokenProvider.GetChangeToken, ResetHandlerTypes);
        }
    }

    private Type[] GetHandlerTypes()
    {
        var types = handlerTypes;
        if (types is not null)
            return types;

        lock (handlerTypesLock)
        {
            types = handlerTypes;
            if (types is not null)
                return types;

            types = BuildHandlerTypes();
            handlerTypes = types;
            return types;
        }
    }

    private void ResetHandlerTypes()
    {
        lock (handlerTypesLock)
        {
            handlerTypes = null;
        }
    }

    private Type[] BuildHandlerTypes()
    {
        return [.. typeSource.GetTypesWithInterface(typeof(IRequestHandler))
            .Where(type => !type.IsInterface && !type.IsAbstract)];
    }

    /// <inheritdoc/>
    public virtual IEnumerable<Type> GetTypes()
    {
        return GetHandlerTypes();
    }

    /// <inheritdoc/>
    public IEnumerable<Type> GetTypes(Type handlerType)
    {
        return GetHandlerTypes().Where(handlerType.IsAssignableFrom);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        changeSubscription?.Dispose();
        changeSubscription = null;
    }
}
