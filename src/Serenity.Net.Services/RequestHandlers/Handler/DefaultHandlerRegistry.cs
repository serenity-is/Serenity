namespace Serenity.Services;

/// <summary>
/// Default implentation for the <see cref="IDefaultHandlerFactory"/>
/// </summary>
public class DefaultHandlerRegistry : IDefaultHandlerRegistry
{
    private readonly ITypeSource typeSource;

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="typeSource">Type source containing possible 
    /// handler classes.</param>
    /// <exception cref="ArgumentNullException">typeSource is null</exception>
    public DefaultHandlerRegistry(ITypeSource typeSource)
    {
        this.typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));
    }

    /// <inheritdoc/>
    public virtual IEnumerable<Type> GetTypes()
    {
        return typeSource.GetTypesWithInterface(typeof(IRequestHandler))
            .Where(type => !type.IsInterface && !type.IsAbstract);
    }

    /// <inheritdoc/>
    public IEnumerable<Type> GetTypes(Type handlerType)
    {
        return GetTypes().Where(type => handlerType.IsAssignableFrom(type));
    }
}