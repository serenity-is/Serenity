using System;
using System.Linq;

namespace Serenity.Services
{
    public class DefaultHandlerFactory : IDefaultHandlerFactory
    {
        private readonly IDefaultHandlerRegistry registry;
        private readonly IHandlerActivator activator;

        public DefaultHandlerFactory(IDefaultHandlerRegistry registry, IHandlerActivator activator)
        {
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.activator = activator ?? throw new ArgumentNullException(nameof(activator));
        }

        public object CreateHandler(Type rowType, Type handlerInterface)
        {
            if (rowType == null)
                throw new ArgumentNullException(nameof(rowType));

            if (handlerInterface == null)
                throw new ArgumentNullException(nameof(handlerInterface));

            var requestHandler = typeof(IRequestHandler<>).MakeGenericType(rowType);

            var handlers = registry.GetTypes(handlerInterface)
                .Where(type => requestHandler.IsAssignableFrom(type))
                .ToArray();

            if (handlers.Length == 1)
                return activator.CreateInstance(handlers[0]);

            if (handlers.Length == 0)
            {
                var attr = handlerInterface.GetAttribute<GenericHandlerTypeAttribute>(true);
                if (attr == null)
                    throw new InvalidProgramException($"{handlerInterface.FullName} does not have a GenericHandlerTypeAttribute!");

                return activator.CreateInstance(attr.Value.MakeGenericType(rowType));
            }

            var defaults = handlers.Where(x => x.GetAttribute<DefaultHandlerAttribute>()?.Value == true);
            if (defaults.Count() == 1)
                return activator.CreateInstance(defaults.First());

            throw new InvalidProgramException($"There are multiple {handlerInterface.FullName} types " +
                $"for row type {rowType.FullName}. Please add [DefaultHandler] to one of them.");
        }
    }
}