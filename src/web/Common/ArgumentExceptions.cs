using System.Runtime.CompilerServices;

namespace Serenity;

/// <summary>
/// This class contains methods for creating ArgumentException and subclasses
/// while avoiding analyzer warnings regarding mismatched argument names.
/// </summary>
public static class ArgumentExceptions
{
    /// <summary>
    /// Creates an <see cref="ArgumentOutOfRangeException"/> for the given argument.
    /// </summary>
    /// <param name="argument">The argument value; ignored by this method.</param>
    /// <param name="paramName">The name of the parameter that caused the exception.</param>
    /// <returns>A new <see cref="ArgumentOutOfRangeException"/> instance.</returns>
#pragma warning disable IDE0060 // Remove unused parameter
    public static ArgumentOutOfRangeException OutOfRange(object argument, 
        [CallerArgumentExpression(nameof(argument))] string paramName = null)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        return new(paramName ?? "missingParamName");
    }
}