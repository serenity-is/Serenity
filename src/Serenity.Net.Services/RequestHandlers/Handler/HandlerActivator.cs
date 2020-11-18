using Microsoft.Extensions.DependencyInjection;
using System;

namespace Serenity.Services
{
    public class HandlerActivator : IHandlerActivator
    {
        private readonly IServiceProvider provider;

        protected HandlerActivator(IServiceProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public object CreateInstance(Type handlerType)
        {
            return provider.GetRequiredService(handlerType);
        }
    }
}