using System.ComponentModel.DataAnnotations;

namespace Serenity.ComponentModel;

/// <summary>
/// Requires a text to have no more than a number of characters, and no fewer than <see cref="StringLengthAttribute.MinimumLength"/>
/// when it is set, which the forms also check. An empty value is valid, as being required is <see cref="RequiredAttribute"/>'s
/// concern.
/// </summary>
/// <param name="maximumLength">The most characters the text can have.</param>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class ValidateLengthAttribute(int maximumLength) : StringLengthAttribute(maximumLength), IClientValidationRule, IHasTextKey
{
    /// <summary>
    /// The type key of the class of the rule on the client side, i.e. <c>Serenity.LengthValidationRule</c>.
    /// </summary>
    public const string Key = "Length";

    /// <inheritdoc/>
    public string? TextKey { get; set; }

    /// <inheritdoc/>
    public ValidationSide Side { get; set; }

    /// <inheritdoc/>
    public string Rule => Key;

    /// <summary>
    /// Gets the minimum and the maximum length, where a minimum of zero is none.
    /// </summary>
    public object? RuleParams => new[] { MinimumLength, MaximumLength };

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string { Length: 0 } || IsValid(value))
            return ValidationResult.Success;

        return MinimumLength > 0 ?
            ValidationRuleMessage.Failure(this, validationContext, "Validation.RangeLength", MinimumLength, MaximumLength) :
            ValidationRuleMessage.Failure(this, validationContext, "Validation.MaxLength", MaximumLength);
    }
}
