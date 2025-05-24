using System.Collections;
using System.Data.Common;

namespace Serenity.TestUtils;

public class MockBehaviorProvider(Func<Type, Type, Type, IEnumerable> resolver) : IBehaviorProvider
{
    private readonly Func<Type, Type, Type, IEnumerable> resolver = resolver 
        ?? throw new ArgumentNullException(nameof(resolver));

    public IEnumerable Resolve(Type handlerType, Type rowType, Type behaviorType)
    {
        return resolver?.Invoke(handlerType, rowType, behaviorType) ?? Array.Empty<object>();
    }
}
