using System.Collections;

namespace Serenity.Tests;

public class NullBehaviorProvider : IBehaviorProvider
{
    public IEnumerable Resolve(Type handlerType, Type rowType, Type behaviorType)
    {
        return Array.Empty<Type>();
    }
}