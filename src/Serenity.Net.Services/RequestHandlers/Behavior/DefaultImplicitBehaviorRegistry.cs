namespace Serenity.Services;

/// <summary>
/// Default implementation for the <see cref="IImplicitBehaviorRegistry"/>
/// </summary>
public class DefaultImplicitBehaviorRegistry : IImplicitBehaviorRegistry
{
    private readonly IEnumerable<Type> behaviorTypes;

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="typeSource">The type source to extract <see cref="IImplicitBehavior"/> types from</param>
    public DefaultImplicitBehaviorRegistry(ITypeSource typeSource)
    {
        behaviorTypes = (typeSource ?? throw new ArgumentNullException(nameof(behaviorTypes)))
            .GetTypesWithInterface(typeof(IImplicitBehavior))
            .Where(type => !type.IsAbstract && !type.IsInterface);
    }

    /// <inheritdoc/>
    public IEnumerable<Type> GetTypes()
    {
        return behaviorTypes;
    }
}