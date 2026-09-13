using System.Runtime.CompilerServices;

namespace Serenity;

public static class ArgumentExceptionHelper
{
#if NETSTANDARD2_0
    extension(ArgumentNullException myInterface)
    {
        public static void ThrowIfNull([System.Diagnostics.CodeAnalysis.NotNull] object? argument, [CallerMemberName] string? paramName = null)
        {
            if (argument is null)
                throw new ArgumentNullException(paramName ?? "unknown");
        }
    }
#endif

#pragma warning disable IDE0060 // Remove unused parameter
#if ISSOURCEGENERATOR
    public static ArgumentOutOfRangeException OutOfRange(object? argument, [CallerMemberName] string? paramName = null)
#else
    public static ArgumentOutOfRangeException OutOfRange(object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
#endif
    {
        return new(paramName ?? "unknown");
    }
#pragma warning restore IDE0060 // Remove unused parameter
}