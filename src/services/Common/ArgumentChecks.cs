using System.Runtime.CompilerServices;

namespace Serenity;

/// <summary>
/// Argument check helpers for validating that non-null values obtained from
/// members (e.g. <c>request.Entity</c>) are not null, returning the value when
/// it is not. Prefer <see cref="ArgumentNullException"/> static
/// <c>ThrowIfNull</c> when checking an actual method parameter, as that keeps
/// the parameter name recognized by CA2208 and null-state flow analysis.
/// </summary>
public static class ArgumentChecks
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/>
    /// is null, otherwise returns it.
    /// </summary>
    /// <typeparam name="T">Type of the argument.</typeparam>
    /// <param name="argument">Argument to check.</param>
    /// <param name="paramName">Automatically populated with the caller's expression,
    /// e.g. <c>"request.Entity"</c>, using
    /// <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The non-null <paramref name="argument"/>.</returns>
    public static T NotNull<T>(T? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        return argument ?? throw new ArgumentNullException(paramName ?? "missingParamName");
    }

    /// <summary>
    /// <typeparam name="T">Type of the argument.</typeparam>
    /// <param name="argument">Argument to check.</param>
    /// <param name="paramName">Automatically populated with the caller's expression,
    /// e.g. <c>"request.Id"</c>, using
    /// <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The non-null <paramref name="argument"/>.</returns>
    /// </summary>
    public static T NotNull<T>(T? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : struct
    {
        return argument ?? throw new ArgumentNullException(paramName ?? "missingParamName");
    }
}
