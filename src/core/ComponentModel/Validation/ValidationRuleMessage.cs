using System.ComponentModel.DataAnnotations;

namespace Serenity.ComponentModel;

/// <summary>
/// Gets the messages of the validation rules, which are localized: the text of the <see cref="IHasTextKey.TextKey"/> of the
/// attribute, else the text of its implied key, which its <see cref="ValidationAttribute.ErrorMessage"/> is registered with,
/// e.g. <c>Db.Northwind.Customer.Quantity_ValidateRange</c>, see <see cref="ValidationRules.GetTextKeySuffix"/>, else its
/// message as it is, else the default message of the rule, by its local text key, e.g. <c>Validation.Range</c>. The parameters of the rule are put in the message, e.g. <c>{0}</c> for the
/// minimum of a range, unlike the messages of DataAnnotations, whose <c>{0}</c> is the name of the field, as a message of a
/// form is displayed by the field.
/// </summary>
public static class ValidationRuleMessage
{
    /// <summary>
    /// The key of the item of a <see cref="ValidationContext"/> that has the text key of the property being validated,
    /// e.g. <c>Db.Northwind.Customer.Quantity</c>, which the implied text keys of its rules start with.
    /// </summary>
    public const string PropertyTextKeyItem = "Serenity.ValidationPropertyTextKey";

    private static readonly Dictionary<string, string> defaults = new(StringComparer.Ordinal)
    {
        ["Validation.Compare"] = "Please enter the same value again.",
        ["Validation.Email"] = "Please enter a valid email address.",
        ["Validation.Invalid"] = "Please enter a valid value.",
        ["Validation.MaxLength"] = "Please enter no more than {0} characters.",
        ["Validation.MinLength"] = "Please enter no less than {0} characters.",
        ["Validation.Pattern"] = "Please enter a value in the expected format.",
        ["Validation.Range"] = "Please enter a value between {0} and {1}.",
        ["Validation.RangeLength"] = "Please enter a value between {0} and {1} characters long.",
        ["Validation.Url"] = "Please enter a valid URL (e.g. http://www.site.com)."
    };

    /// <summary>
    /// Gets the invariant text of a default message, for when the local texts are not registered, e.g. in a unit test.
    /// </summary>
    /// <param name="key">The local text key of the message, e.g. <c>Validation.Range</c>.</param>
    public static string? GetDefaultText(string key) => defaults.TryGetValue(key, out var text) ? text : null;

    /// <summary>
    /// Gets the message of an attribute whose value is not valid.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    /// <param name="localizer">The localizer, or null to use the invariant texts.</param>
    /// <param name="impliedTextKey">The implied text key of the message, see <see cref="GetImpliedTextKey"/>, or null.</param>
    /// <param name="defaultKey">The local text key of the default message of the rule.</param>
    /// <param name="args">The parameters of the rule, which are put in the message.</param>
    public static string Format(ValidationAttribute attribute, ITextLocalizer? localizer, string? impliedTextKey,
        string defaultKey, params object?[] args)
    {
        if (attribute is null)
            throw new ArgumentNullException(nameof(attribute));

        string? text = null;
        if (attribute is IHasTextKey { TextKey: { Length: > 0 } key })
            text = localizer?.TryGet(key);

        if (text is null && !string.IsNullOrEmpty(impliedTextKey))
            text = localizer?.TryGet(impliedTextKey!);

        if (text is null && attribute.ErrorMessageResourceType is null && !string.IsNullOrEmpty(attribute.ErrorMessage))
            text = attribute.ErrorMessage;

        text ??= localizer?.TryGet(defaultKey) ?? GetDefaultText(defaultKey) ?? defaultKey;

        return args.Length == 0 ? text : string.Format(CultureInfo.CurrentCulture, text, args);
    }

    /// <summary>
    /// Gets the result for an attribute whose value is not valid, with its message, see
    /// <see cref="Format(ValidationAttribute, ITextLocalizer?, string, object?[])"/>, for the member being validated.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    /// <param name="context">The context, whose localizer is used, see <see cref="ValidationContextExtensions.GetLocalizer"/>,
    /// and the text key of whose property the implied text key of the message starts with, see <see cref="PropertyTextKeyItem"/>.</param>
    /// <param name="defaultKey">The local text key of the default message of the rule.</param>
    /// <param name="args">The parameters of the rule.</param>
    public static ValidationResult Failure(ValidationAttribute attribute, ValidationContext? context, string defaultKey,
        params object?[] args)
    {
        var message = Format(attribute, context?.GetLocalizer(), GetImpliedTextKey(attribute, context), defaultKey, args);
        return context?.MemberName is { } member ? new ValidationResult(message, [member]) : new ValidationResult(message);
    }

    /// <summary>
    /// Gets the implied text key of the message of a rule, which is the text key of the property being validated with the
    /// suffix of the rule, e.g. <c>Db.Northwind.Customer.Quantity_ValidateRange</c>, when the context has the text key of the
    /// property, see <see cref="PropertyTextKeyItem"/>, and the rule has one, see <see cref="ValidationRules.GetTextKeySuffix"/>.
    /// </summary>
    /// <param name="attribute">The attribute of the rule.</param>
    /// <param name="context">The context.</param>
    public static string? GetImpliedTextKey(ValidationAttribute attribute, ValidationContext? context)
    {
        if (context?.Items is null || !context.Items.TryGetValue(PropertyTextKeyItem, out var value) ||
            value is not string { Length: > 0 } propertyKey)
            return null;

        return ValidationRules.GetTextKeySuffix(attribute) is { } suffix ? propertyKey + suffix : null;
    }
}
