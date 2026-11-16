using System.ComponentModel.DataAnnotations;

namespace Serenity.ComponentModel;

/// <summary>
/// Requires a value to be between a minimum and a maximum, both included, which the forms also check. The limits are parsed,
/// and a value given as text is converted, in the invariant culture. An empty value is valid, as being required is
/// <see cref="RequiredAttribute"/>'s concern.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class ValidateRangeAttribute : RangeAttribute, IClientValidationRule, IHasTextKey
{
    /// <summary>
    /// The type key of the class of the rule on the client side, i.e. <c>Serenity.RangeValidationRule</c>.
    /// </summary>
    public const string Key = "Range";

    /// <summary>
    /// Creates a range of integers.
    /// </summary>
    /// <param name="minimum">The minimum.</param>
    /// <param name="maximum">The maximum.</param>
    public ValidateRangeAttribute(int minimum, int maximum) : base(minimum, maximum)
    {
        ParseLimitsInInvariantCulture = true;
        ConvertValueInInvariantCulture = true;
    }

    /// <summary>
    /// Creates a range of numbers.
    /// </summary>
    /// <param name="minimum">The minimum.</param>
    /// <param name="maximum">The maximum.</param>
    public ValidateRangeAttribute(double minimum, double maximum) : base(minimum, maximum)
    {
        ParseLimitsInInvariantCulture = true;
        ConvertValueInInvariantCulture = true;
    }

    /// <summary>
    /// Creates a range of another type, e.g. dates, whose limits are given as text, e.g. <c>2026-01-01</c>.
    /// </summary>
    /// <param name="type">The type of the value.</param>
    /// <param name="minimum">The minimum, as text in the invariant culture.</param>
    /// <param name="maximum">The maximum, as text in the invariant culture.</param>
    public ValidateRangeAttribute(Type type, string minimum, string maximum) : base(type, minimum, maximum)
    {
        ParseLimitsInInvariantCulture = true;
        ConvertValueInInvariantCulture = true;
    }

    /// <inheritdoc/>
    public string? TextKey { get; set; }

    /// <inheritdoc/>
    public ValidationSide Side { get; set; }

    /// <inheritdoc/>
    public string Rule => Key;

    /// <inheritdoc/>
    public object? RuleParams => new[] { Minimum, Maximum };

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return IsValid(value) ? ValidationResult.Success :
            ValidationRuleMessage.Failure(this, validationContext, "Validation.Range", Minimum, Maximum);
    }
}
