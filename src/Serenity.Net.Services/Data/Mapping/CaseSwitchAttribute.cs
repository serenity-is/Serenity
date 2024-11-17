namespace Serenity.Data.Mapping;

/// <summary>
/// Case expression with simple switch value
/// </summary>
public class CaseSwitchAttribute : CaseAttribute
{
    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="switchValue">Switch value</param>
    /// <param name="when1">When expression 1</param>
    /// <param name="then1">When expression 2</param>
    /// <param name="pairsElse">Additional when else pairs, followed by an optional ELSE statement.
    /// Else assumed only if the number of elements is odd</param>
    /// <exception cref="ArgumentNullException">One of expressions is null</exception>
    public CaseSwitchAttribute(object switchValue, object when1, object then1, params object[] pairsElse)
        : base(when1, then1, pairsElse)
    {
        Switch = switchValue ?? throw new ArgumentNullException(nameof(switchValue));
    }
}
