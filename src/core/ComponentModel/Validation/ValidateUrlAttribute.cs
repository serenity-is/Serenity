using System.ComponentModel.DataAnnotations;

namespace Serenity.ComponentModel;

/// <summary>
/// Requires a text to be an absolute http, https or ftp address with a host, which the forms also check. It is stricter
/// than <see cref="UrlAttribute"/>, which only checks how the text starts, and can not be derived from. Like it, it is a
/// <see cref="DataTypeAttribute"/> of <see cref="DataType.Url"/>, which tools like OpenAPI read. An empty value is valid, as being required is
/// <see cref="RequiredAttribute"/>'s concern.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class ValidateUrlAttribute() : DataTypeAttribute(DataType.Url), IClientValidationRule, IHasTextKey
{
    /// <summary>
    /// The type key of the class of the rule on the client side, i.e. <c>Serenity.UrlValidationRule</c>.
    /// </summary>
    public const string Key = "Url";

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

        return value is string text && !text.Any(char.IsWhiteSpace) &&
            Uri.TryCreate(text, UriKind.Absolute, out var uri) && !string.IsNullOrEmpty(uri.Host) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeFtp);
    }

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return IsValid(value) ? ValidationResult.Success :
            ValidationRuleMessage.Failure(this, validationContext, "Validation.Url");
    }
}
