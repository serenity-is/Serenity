using System;

namespace Serenity.Services
{
    public interface IBehaviorFactory
    {
        object Create(Type behaviorType);
    }
}