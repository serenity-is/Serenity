using System.Runtime.CompilerServices;

namespace Serenity;

/// <summary>
/// This class contains methods for creating ArgumentException and subclasses
/// while avoiding analyzer warnings regarding mismatched argument names.
/// </summary>
public static class ArgumentExceptions
{
    /// <summary>
    /// Creates an ArgumentOutOfRangeException
    /// </summary>
    /// <param name="argument">Argument value, ignored</param>
    /// <param name="paramName"></param>
    /// <exception cref="ArgumentNullException"></exception>
#pragma warning disable IDE0060 // Remove unused parameter
    public static ArgumentOutOfRangeException OutOfRange(object argument, 
        [CallerArgumentExpression(nameof(argument))] string paramName = null)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        return new(paramName ?? "missingParamName");
    }
}