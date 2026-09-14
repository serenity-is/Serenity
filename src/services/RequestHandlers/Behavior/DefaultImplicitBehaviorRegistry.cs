namespace Serenity.Services;

/// <summary>
/// Default implementation for the <see cref="IImplicitBehaviorRegistry"/>
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="typeSource">The type source to extract <see cref="IImplicitBehavior"/> types from</param>
/// <exception cref="ArgumentNullException"><paramref name="typeSource"/> is <c>null</c>.</exception>
public class DefaultImplicitBehaviorRegistry(ITypeSource typeSource) : IImplicitBehaviorRegistry
{
    private readonly IEnumerable<Type> behaviorTypes = (typeSource ?? throw new ArgumentNullException(nameof(typeSource)))
            .GetTypesWithInterface(typeof(IImplicitBehavior))
            .Where(type => !type.IsAbstract && !type.IsInterface);

    /// <inheritdoc/>
    public IEnumerable<Type> GetTypes()
    {
        return behaviorTypes;
    }
}