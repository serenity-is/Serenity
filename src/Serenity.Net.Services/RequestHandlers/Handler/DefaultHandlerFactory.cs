namespace Serenity.Services;

/// <summary>
/// Default implementation for the <see cref="IDefaultHandlerFactory"/>
/// </summary>
public class DefaultHandlerFactory : IDefaultHandlerFactory
{
    private readonly IDefaultHandlerRegistry registry;
    private readonly IHandlerActivator activator;
    private readonly ConcurrentDictionary<(Type rowType, Type handlerInterface), Type> cache;

    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    /// <param name="registry">Default handler registry</param>
    /// <param name="activator">Handler activator</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DefaultHandlerFactory(IDefaultHandlerRegistry registry, IHandlerActivator activator)
    {
        cache = new ConcurrentDictionary<(Type rowType, Type handlerInterface), Type>();
        this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
        this.activator = activator ?? throw new ArgumentNullException(nameof(activator));
    }

    private Type GetHandlerType((Type rowType, Type handlerInterface) args)
    {
        var requestHandler = typeof(IRequestHandler<>).MakeGenericType(args.rowType);

        var handlers = registry.GetTypes(args.handlerInterface)
            .Where(type => requestHandler.IsAssignableFrom(type))
            .ToArray();

        if (handlers.Length == 1)
            return handlers[0];

        if (handlers.Length == 0)
        {
            var attr = args.handlerInterface.GetAttribute<GenericHandlerTypeAttribute>(true);
            if (attr == null)
                throw new InvalidProgramException($"{args.handlerInterface.FullName} does not have a GenericHandlerTypeAttribute!");

            return attr.Value.MakeGenericType(args.rowType);
        }

        var defaults = handlers.Where(x => x.GetAttribute<DefaultHandlerAttribute>()?.Value == true);
        if (defaults.Count() == 1)
            return defaults.First();

        throw new InvalidProgramException($"There are multiple {args.handlerInterface.FullName} types " +
            $"for row type {args.rowType.FullName}. Please add [DefaultHandler] to one of them.");
    }

    /// <inheritdoc/>
    public object CreateHandler(Type rowType, Type handlerInterface)
    {
        if (rowType == null)
            throw new ArgumentNullException(nameof(rowType));

        if (handlerInterface == null)
            throw new ArgumentNullException(nameof(handlerInterface));

        var handlerType = cache.GetOrAdd((rowType, handlerInterface), GetHandlerType);
        return activator.CreateInstance(handlerType);
    }
}