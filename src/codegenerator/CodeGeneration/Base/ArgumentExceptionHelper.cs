using System.Runtime.CompilerServices;

namespace Serenity;

public static class ArgumentExceptionHelper
{
#if ISSOURCEGENERATOR
    public static void ThrowIfNull(object argument, [CallerMemberName] string paramName = null)
    {
        if (argument is null)
            throw new ArgumentNullException(paramName ?? "unknown");
#else
    public static void ThrowIfNull(this object argument, [CallerArgumentExpression(nameof(argument))] string paramName = null)
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
#endif
    }

#pragma warning disable IDE0060 // Remove unused parameter
#if ISSOURCEGENERATOR
    public static ArgumentOutOfRangeException OutOfRange(object argument, [CallerMemberName] string paramName = null)
#else
    public static ArgumentOutOfRangeException OutOfRange(object argument, [CallerArgumentExpression(nameof(argument))] string paramName = null)
#endif
    {
        return new(paramName ?? "unknown");
    }
#pragma warning restore IDE0060 // Remove unused parameter
}