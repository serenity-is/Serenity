namespace Serenity.ComponentModel;

/// <summary>
/// A validation attribute that the forms also check, by the client side rule class whose type key is its rule, e.g.
/// <c>Range</c> for <c>Serenity.RangeValidationRule</c>, with the minimum and the maximum as its parameters. The Validate attributes, like
/// <see cref="ValidateRangeAttribute"/>, and the ones deriving from <see cref="ValidationRuleAttribute"/> implement it.
/// </summary>
public interface IClientValidationRule
{
    /// <summary>
    /// Gets the type key of the class of the rule on the client side, which can be short like the keys of the editors, e.g.
    /// <c>Range</c> for <c>Serenity.RangeValidationRule</c>.
    /// </summary>
    string Rule { get; }

    /// <summary>
    /// Gets the parameters of the rule on the client side, which are serialized to JSON, or null when it has none.
    /// </summary>
    object? RuleParams { get; }

    /// <summary>
    /// Gets where the rule is checked: by the forms and on the server, which is the default, or only on one of them.
    /// </summary>
    ValidationSide Side { get; }
}
