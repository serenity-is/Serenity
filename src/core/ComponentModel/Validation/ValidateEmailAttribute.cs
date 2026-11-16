using System.ComponentModel.DataAnnotations;

namespace Serenity.ComponentModel;

/// <summary>
/// Requires a text to be an email address, by <see cref="Pattern"/>, which is stricter than <see cref="EmailAddressAttribute"/>,
/// as that only checks for an @ that is not the first or the last character, and can not be derived from. Like it, it is a
/// <see cref="DataTypeAttribute"/> of <see cref="DataType.EmailAddress"/>, which tools like OpenAPI read. The forms also check it. An empty value is valid, as being required is <see cref="RequiredAttribute"/>'s concern.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class ValidateEmailAttribute() : DataTypeAttribute(DataType.EmailAddress), IClientValidationRule, IHasTextKey
{
    /// <summary>
    /// The type key of the class of the rule on the client side, i.e. <c>Serenity.EmailValidationRule</c>.
    /// </summary>
    public const string Key = "Email";

    /// <summary>
    /// The pattern of an email address, of ASCII letters, digits and the symbols an address can have before the @, and a
    /// domain of labels of letters, digits and hyphens.
    /// </summary>
    public static readonly Regex Pattern = new(
        @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
        RegexOptions.Compiled | RegexOptions.ECMAScript, TimeSpan.FromSeconds(1));

    /// <inheritdoc/>
    public string? TextKey { get; set; }

    /// <inheritdoc/>
    public ValidationSide Side { get; set; }

    /// <inheritdoc/>
    public string Rule => Key;

    /// <inheritdoc/>
    public object? RuleParams => null;

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (value is null || value is string { Length: 0 })
            return true;

        return value is string text && Pattern.IsMatch(text);
    }

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return IsValid(value) ? ValidationResult.Success :
            ValidationRuleMessage.Failure(this, validationContext, "Validation.Email");
    }
}
