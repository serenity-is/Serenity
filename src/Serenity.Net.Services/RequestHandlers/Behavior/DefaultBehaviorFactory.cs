using Microsoft.Extensions.DependencyInjection;
using System;

namespace Serenity.Services
{
    public class DefaultBehaviorFactory : IBehaviorFactory
    {
        private readonly IServiceProvider provider;

        protected DefaultBehaviorFactory(IServiceProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public object CreateInstance(Type behaviorType)
        {
            return provider.GetRequiredService(behaviorType);
        }
    }
}