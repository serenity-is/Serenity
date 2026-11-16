using System.ComponentModel.DataAnnotations;

namespace Serenity.ComponentModel;

/// <summary>
/// The base of a custom validation rule that the forms also check, by the client side rule class of the type key
/// <see cref="Rule"/>, which is given to the constructor, like the key of a custom editor, as a <c>Key</c> constant of the
/// attribute, e.g. <c>public const string Key = "MyApp.EvenValidationRule"</c>, or else by the convention of
/// <see cref="GetConventionalRule"/>. The server typings generator finds it the same way, and imports that class in the forms
/// that have the rule, so it is loaded with them. Its message is localized like the ones of the Validate
/// attributes, see <see cref="ValidationRuleMessage"/>, by <see cref="DefaultTextKey"/> when no other is given. A rule that
/// needs to wait, e.g. for a query, can override <see cref="IsValidValueAsync"/>, which the asynchronous save handlers await,
/// while the synchronous ones, and other validators, still call <see cref="IsValidValue"/>.
/// </summary>
/// <example>
/// <code>
/// public class ValidateEvenAttribute() : ValidationRuleAttribute(Key)
/// {
///     public const string Key = "MyApp.EvenValidationRule";
///     protected override string DefaultTextKey => "Validation.Even";
///     protected override bool IsValidValue(object? value, ValidationContext context) =>
///         Convert.ToInt64(value, CultureInfo.InvariantCulture) % 2 == 0;
/// }
/// </code>
/// </example>
/// <remarks>
/// A rule can't be repeated on a property, like the Validate attributes, as the implied text key of its message, see
/// <see cref="ValidationRules.GetTextKeySuffix"/>, and its rule on the client side would be the same. A derived class that
/// is used for several rules, e.g. by a rule name given to it, can have an AttributeUsage of its own allowing it to be
/// repeated: the implied text keys of its messages then also have the rule, e.g. <c>Quantity_ValidateClient_even</c>, and
/// two of them with the same rule need a <see cref="TextKey"/> each.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public abstract class ValidationRuleAttribute : ValidationAttribute, IClientValidationRule, IAsyncValidationRule, IHasTextKey
{
    private readonly string? rule;

    /// <summary>
    /// Creates a rule whose client side class is found by the convention of <see cref="GetConventionalRule"/>.
    /// </summary>
    protected ValidationRuleAttribute()
    {
    }

    /// <summary>
    /// Creates a rule with the type key of its client side class, e.g. a <c>Key</c> constant of the attribute.
    /// </summary>
    /// <param name="rule">The type key of the class of the rule on the client side.</param>
    protected ValidationRuleAttribute(string rule)
    {
        this.rule = !string.IsNullOrEmpty(rule) ? rule : throw new ArgumentNullException(nameof(rule));
    }

    /// <inheritdoc/>
    public string? TextKey { get; set; }

    /// <inheritdoc/>
    public ValidationSide Side { get; set; }

    /// <summary>
    /// Gets the type key of the class of the rule on the client side, the one given to the constructor, or else the one of the
    /// attribute by the convention of <see cref="GetConventionalRule"/>. A rule that overrides it to give another key is not
    /// found by the server typings generator, so its class is loaded on demand, if it implements LazyLoadInclude.
    /// </summary>
    public virtual string Rule => rule ?? GetConventionalRule(GetType());

    /// <summary>
    /// Gets the type key of the client side rule class of a rule attribute by convention: its full name without the
    /// Validate prefix of its name and its Attribute suffix, with a ValidationRule suffix, e.g.
    /// <c>MyApp.EvenValidationRule</c> for <c>MyApp.ValidateEvenAttribute</c>.
    /// </summary>
    /// <param name="attributeType">The type of the attribute.</param>
    public static string GetConventionalRule(Type attributeType)
    {
        if (attributeType is null)
            throw new ArgumentNullException(nameof(attributeType));

        var name = attributeType.Name;
        if (name.EndsWith("Attribute", StringComparison.Ordinal))
            name = name[..^"Attribute".Length];
        if (name.StartsWith("Validate", StringComparison.Ordinal) && name.Length > "Validate".Length)
            name = name["Validate".Length..];

        return (string.IsNullOrEmpty(attributeType.Namespace) ? "" : attributeType.Namespace + ".") + name + "ValidationRule";
    }

    /// <inheritdoc/>
    public virtual object? RuleParams => null;

    /// <summary>
    /// Gets the local text key of the message when neither a <see cref="TextKey"/> nor a
    /// <see cref="ValidationAttribute.ErrorMessage"/> is given, by default <c>Validation.Invalid</c>.
    /// </summary>
    protected virtual string DefaultTextKey => "Validation.Invalid";

    /// <summary>
    /// Gets the parameters that are put in the message, e.g. <c>{0}</c>, by default none.
    /// </summary>
    protected virtual object?[] GetMessageArgs() => [];

    /// <summary>
    /// Gets whether a value is valid. An empty value, i.e. null or an empty text, is valid without calling it, as being
    /// required is <see cref="RequiredAttribute"/>'s concern.
    /// </summary>
    /// <param name="value">The value, which is not empty.</param>
    /// <param name="context">The context, e.g. for the other values of the object, or its services.</param>
    protected abstract bool IsValidValue(object? value, ValidationContext context);

    /// <summary>
    /// Gets whether a value is valid asynchronously, by default by <see cref="IsValidValue"/>. An empty value, i.e. null or an
    /// empty text, is valid without calling it. A rule that overrides it should still give <see cref="IsValidValue"/> a
    /// synchronous check, as the synchronous save handlers and other validators call that one.
    /// </summary>
    /// <param name="value">The value, which is not empty.</param>
    /// <param name="context">The context, e.g. for the other values of the object, or its services.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected virtual ValueTask<bool> IsValidValueAsync(object? value, ValidationContext context, CancellationToken cancellationToken)
    {
        return new ValueTask<bool>(IsValidValue(value, context));
    }

    /// <inheritdoc/>
    public async ValueTask<ValidationResult?> IsValidAsync(object? value, ValidationContext context,
        CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (value is null || value is string { Length: 0 } ||
            await IsValidValueAsync(value, context, cancellationToken).ConfigureAwait(false))
            return ValidationResult.Success;

        return ValidationRuleMessage.Failure(this, context, DefaultTextKey, GetMessageArgs());
    }

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext is null)
            throw new ArgumentNullException(nameof(validationContext));

        if (value is null || value is string { Length: 0 } || IsValidValue(value, validationContext))
            return ValidationResult.Success;

        return ValidationRuleMessage.Failure(this, validationContext, DefaultTextKey, GetMessageArgs());
    }
}
