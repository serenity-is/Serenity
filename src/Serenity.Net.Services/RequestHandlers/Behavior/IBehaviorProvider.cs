using System;
using System.Collections;

namespace Serenity.Services
{
    /// <summary>
    /// Interface abstraction for behavior provider
    /// </summary>
    public interface IBehaviorProvider
    {
        IEnumerable Resolve(Type forType, Type behaviorType, Type[] explicitBehaviors);
    }
}