using System.Runtime.CompilerServices;

namespace Serenity.TestUtils;

public static class AssertExtensions
{
    public static T AssertNotNull<T>(this T t, [CallerArgumentExpression(nameof(t))] string expression = null)
        where T: class
    {
        if (t == null && expression != null)
            throw new ArgumentNullException($"Assert.NotNull() Failure: {expression} is null");

        Assert.NotNull(t);
        return t;
    }
}