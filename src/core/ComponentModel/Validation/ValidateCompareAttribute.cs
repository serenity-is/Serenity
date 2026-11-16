using System.ComponentModel.DataAnnotations;

namespace Serenity.ComponentModel;

/// <summary>
/// Requires a value to be the same as the one of another property, e.g. a password and its confirmation, which the forms
/// also check, with the field of that property.
/// </summary>
/// <param name="otherProperty">The name of the other property, e.g. <c>nameof(Password)</c>.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ValidateCompareAttribute(string otherProperty) : CompareAttribute(otherProperty), IClientValidationRule, IHasTextKey
{
    /// <summary>
    /// The type key of the class of the rule on the client side, i.e. <c>Serenity.CompareValidationRule</c>.
    /// </summary>
    public const string Key = "Compare";

    /// <inheritdoc/>
    public string? TextKey { get; set; }

    /// <inheritdoc/>
    public ValidationSide Side { get; set; }

    /// <inheritdoc/>
    public string Rule => Key;

    /// <inheritdoc/>
    public object? RuleParams => OtherProperty;

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext is null)
            throw new ArgumentNullException(nameof(validationContext));

        var other = validationContext.GetPropertyValue(OtherProperty);
        // an empty value is the same as another empty one, e.g. a null and an empty text of a form
        static bool empty(object? x) => x is null || x is string { Length: 0 };
        return Equals(value, other) || (empty(value) && empty(other)) ? ValidationResult.Success :
            ValidationRuleMessage.Failure(this, validationContext, "Validation.Compare");
    }
}
