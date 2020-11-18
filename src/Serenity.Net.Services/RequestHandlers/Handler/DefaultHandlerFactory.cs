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

        public object CreateDefaultHandler(Type rowType, Type handlerType, Type genericType)
        {
            if (rowType == null)
                throw new ArgumentNullException(nameof(rowType));

            if (handlerType == null)
                throw new ArgumentNullException(nameof(handlerType));

            var requestHandler = typeof(IRequestHandler<>).MakeGenericType(rowType);

            var handlers = registry.GetTypes(handlerType)
                .Where(type => requestHandler.IsAssignableFrom(type))
                .ToArray();

            if (handlers.Length == 1)
                return activator.CreateInstance(handlers[0]);

            if (handlers.Length == 0)
            {
                if (genericType == null)
                    throw new ArgumentNullException(nameof(genericType));

                return activator.CreateInstance(genericType.MakeGenericType(rowType));
            }

            var defaults = handlers.Where(x => x.GetAttribute<DefaultHandlerAttribute>()?.Value == true);
            if (defaults.Count() == 1)
                return activator.CreateInstance(defaults.First());

            throw new InvalidProgramException($"There are multiple {handlerType.FullName} types " +
                $"for row type {rowType.FullName}. Please add [DefaultHandler] to one of them.");
        }
    }
}