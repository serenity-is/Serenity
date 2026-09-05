namespace Serenity.Services;

/// <summary>
/// Default implementation for the <see cref="IDefaultHandlerFactory"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="registry">Default handler registry</param>
/// <param name="activator">Handler activator</param>
/// <exception cref="ArgumentNullException"><paramref name="registry"/> or <paramref name="activator"/> is <c>null</c>.</exception>
public class DefaultHandlerFactory(IDefaultHandlerRegistry registry, IHandlerActivator activator) : IDefaultHandlerFactory
{
    private readonly IDefaultHandlerRegistry registry = registry ?? throw new ArgumentNullException(nameof(registry));
    private readonly IHandlerActivator activator = activator ?? throw new ArgumentNullException(nameof(activator));
    private readonly ConcurrentDictionary<(Type rowType, Type handlerInterface), (Type HandlerType, Type WrapperType)> cache = new();

    private Type ResolveCustomHandler(Type handlerInterface, Type companionInterface, Type rowType)
    {
        var requestHandler = typeof(IRequestHandler<>).MakeGenericType(rowType);
        bool isMatch(Type x) => requestHandler.IsAssignableFrom(x) &&
            x.GetCustomAttribute<DefaultHandlerAttribute>(inherit: false)?.Value != false;

        // collect custom handler types that implement the requested handler interface,
        // its companion (other sync/async mode) interface, or both. a type implementing
        // both is counted only once.
        var handlersEnum = registry.GetTypes(handlerInterface);
        if (companionInterface != null)
            handlersEnum = handlersEnum.Concat(registry.GetTypes(companionInterface)).Distinct();

        var handlers = handlersEnum.Where(isMatch).ToArray();

        if (handlers.Length == 1)
            return handlers[0];

        if (handlers.Length == 0)
            return null;

        var defaults = handlers.Where(x => x.GetCustomAttribute<DefaultHandlerAttribute>(inherit: false)?.Value == true);
        if (defaults.Count() == 1)
            return defaults.First();

        throw new InvalidProgramException($"There are multiple {handlerInterface.FullName} types " +
            $"for row type {rowType.FullName}. Please add [DefaultHandler] to one of them.");
    }

    private (Type HandlerType, Type WrapperType) GetHandlerType((Type rowType, Type handlerInterface) args)
    {
        var companionAttr = args.handlerInterface.GetCustomAttribute<CompanionHandlerTypeAttribute>(inherit: true);
        var companionInterface = companionAttr?.CompanionType;

        // resolve the single custom handler (if any) that implements the requested handler
        // interface and/or its companion (other sync/async mode) interface
        var handlerType = ResolveCustomHandler(args.handlerInterface, companionInterface, args.rowType);
        if (handlerType == null)
        {
            var attr = args.handlerInterface.GetCustomAttribute<GenericHandlerTypeAttribute>(inherit: true) ??
                throw new InvalidProgramException($"{args.handlerInterface.FullName} does not have a GenericHandlerTypeAttribute!");
            return (attr.Value.MakeGenericType(args.rowType), null);
        }

        // if the custom handler does not implement the requested interface, it must implement
        // the companion interface, so wrap it to adapt it to the requested mode. e.g. an async
        // behavior requesting a save handler for a row that only has a synchronous custom
        // save handler.
        if (!args.handlerInterface.IsAssignableFrom(handlerType))
            return (handlerType, companionAttr.WrapperType.MakeGenericType(args.rowType));

        return (handlerType, null);
    }

    /// <inheritdoc/>
    public object CreateHandler(Type rowType, Type handlerInterface)
    {
        ArgumentNullException.ThrowIfNull(rowType);

        ArgumentNullException.ThrowIfNull(handlerInterface);

        var (handlerType, wrapperType) = cache.GetOrAdd((rowType, handlerInterface), GetHandlerType);
        var handler = activator.CreateInstance(handlerType);
        if (wrapperType != null)
            return Activator.CreateInstance(wrapperType, handler);
        return handler;
    }
}
