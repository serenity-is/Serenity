using Microsoft.Extensions.DependencyInjection;
using System;

namespace Serenity.Services
{
    public class DefaultBehaviorFactory : IBehaviorFactory
    {
        private readonly IServiceProvider provider;

        public DefaultBehaviorFactory(IServiceProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public object CreateInstance(Type behaviorType)
        {
            return ActivatorUtilities.CreateInstance(provider, behaviorType);
        }
    }
}