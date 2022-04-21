using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Services
{
    public class DefaultHandlerActivator : IHandlerActivator
    {
        private readonly IServiceProvider provider;

        public DefaultHandlerActivator(IServiceProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public object CreateInstance(Type handlerType)
        {
            return ActivatorUtilities.CreateInstance(provider, handlerType);
        }
    }
}