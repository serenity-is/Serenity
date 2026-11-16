using System.ComponentModel.DataAnnotations;

namespace Serenity.ComponentModel;

/// <summary>
/// Finds the validation rules of a property, the forms' form of them, and checks them on the server. The rules are the
/// <see cref="ValidationAttribute"/>s of the property and the ones its other attributes imply, see
/// <see cref="IImpliesValidation"/>. The Validate attributes, like <see cref="ValidateRangeAttribute"/>, are also checked by
/// the forms, and so are the DataAnnotations attributes they derive from, which have the default messages of Serenity when
/// they are not given one, while any other <see cref="ValidationAttribute"/> is only checked on the server.
/// </summary>
public static class ValidationRules
{
    /// <summary>
    /// Gets the validation rules among the attributes of a property, and the ones its other attributes imply.
    /// </summary>
    /// <param name="attributes">The attributes of the property.</param>
    public static IEnumerable<ValidationAttribute> GetRules(IEnumerable<object> attributes)
    {
        if (attributes is null)
            throw new ArgumentNullException(nameof(attributes));

        foreach (var attribute in attributes)
        {
            if (attribute is ValidationAttribute validation)
                yield return validation;

            if (attribute is IImpliesValidation implies)
                foreach (var implied in implies.GetImpliedValidation() ?? [])
                    if (implied is not null)
                        yield return implied;
        }
    }

    /// <summary>
    /// Gets the suffix of the implied text key of the message of a rule, which follows the text key of its property, e.g.
    /// <c>_ValidateRange</c> for <c>Db.Northwind.Customer.Quantity_ValidateRange</c>, by the name of its attribute without
    /// Attribute, so a property can't have two rules with the same key, as an attribute of a rule can't be repeated. An
    /// attribute that allows being repeated, e.g. one rule class for several rules, also has the name of the rule, e.g.
    /// <c>_ValidateClient_even</c>, and two of them with the same rule need a <see cref="IHasTextKey.TextKey"/> each. Only the
    /// rules of Serenity have one, i.e. the ones that implement <see cref="IClientValidationRule"/>, as the messages of the
    /// DataAnnotations attributes are texts with the name of the field as <c>{0}</c>, not localizable texts.
    /// </summary>
    /// <param name="attribute">The attribute of the rule.</param>
    /// <returns>The suffix, or null when the rule has none.</returns>
    public static string? GetTextKeySuffix(ValidationAttribute attribute)
    {
        if (attribute is not IClientValidationRule rule)
            return null;

        var type = attribute.GetType();
        var name = type.Name;
        if (name.EndsWith("Attribute", StringComparison.Ordinal))
            name = name[..^"Attribute".Length];

        // the class name is what is stable for a rule that can't be repeated, while its rule may change by its parameters,
        // e.g. maxlength and rangelength of ValidateLength, so the rule is only added for one that can be repeated
        var usage = type.GetCustomAttribute<AttributeUsageAttribute>(inherit: true);
        return usage?.AllowMultiple == true && !string.IsNullOrEmpty(rule.Rule) ? "_" + name + "_" + rule.Rule : "_" + name;
    }

    /// <summary>
    /// Gets whether a rule has a message of its own, which is registered with its implied text key, see
    /// <see cref="GetTextKeySuffix"/>, i.e. it is a rule of Serenity with an <see cref="ValidationAttribute.ErrorMessage"/>.
    /// </summary>
    /// <param name="attribute">The attribute of the rule.</param>
    public static bool HasLocalizableMessage(ValidationAttribute attribute)
    {
        if (attribute is null)
            throw new ArgumentNullException(nameof(attribute));

        return attribute is IClientValidationRule && GetOwnMessage(attribute) is not null;
    }

    /// <summary>
    /// Gets the rule the forms check for a validation attribute, or null when they don't check it, i.e. it is only checked
    /// on the server, or is not one they know.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    public static ClientValidationRule? GetClientRule(ValidationAttribute attribute)
    {
        if (attribute is null)
            throw new ArgumentNullException(nameof(attribute));

        if (attribute is IClientValidationRule rule)
        {
            if (rule.Side == ValidationSide.Server)
                return null;

            return new ClientValidationRule
            {
                Rule = rule.Rule,
                Params = rule.RuleParams,
                TextKey = (attribute as IHasTextKey)?.TextKey,
                Message = GetOwnMessage(attribute)
            };
        }

        if (GetDataAnnotationsRule(attribute) is not { } known)
            return null;

        return new ClientValidationRule
        {
            Rule = known.Rule,
            Params = known.Params,
            // a message of a DataAnnotations attribute is formatted with the name of the field as {0}, which the forms
            // don't have, so it is only used when it has nothing to format
            Message = GetOwnMessage(attribute) is { } message && !message.Contains('{') ? message : null
        };
    }

    /// <summary>
    /// Checks a value by a validation rule on the server, unless it is only checked by the forms.
    /// </summary>
    /// <param name="attribute">The attribute of the rule.</param>
    /// <param name="value">The value.</param>
    /// <param name="context">The context, whose localizer localizes the message, see <see cref="ValidationContextExtensions.GetLocalizer"/>.</param>
    /// <returns>The message, or null when the value is valid.</returns>
    public static string? Validate(ValidationAttribute attribute, object? value, ValidationContext context)
    {
        if (attribute is null)
            throw new ArgumentNullException(nameof(attribute));

        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (attribute is IClientValidationRule { Side: ValidationSide.Client })
            return null;

        var result = attribute.GetValidationResult(value, context);
        if (result == ValidationResult.Success)
            return null;

        // the DataAnnotations attributes that the forms check have the same default messages as there, localized
        if (attribute is not IClientValidationRule && GetOwnMessage(attribute) is null &&
            GetDataAnnotationsRule(attribute) is { } known)
            return ValidationRuleMessage.Format(attribute, context.GetLocalizer(), null, known.TextKey, known.Args);

        return result?.ErrorMessage ?? ValidationRuleMessage.Format(attribute, context.GetLocalizer(), null, "Validation.Invalid");
    }

    /// <summary>
    /// Checks a value by a validation rule on the server asynchronously, unless it is only checked by the forms. A rule that
    /// implements <see cref="IAsyncValidationRule"/> is awaited, and any other is checked synchronously, see
    /// <see cref="Validate"/>.
    /// </summary>
    /// <param name="attribute">The attribute of the rule.</param>
    /// <param name="value">The value.</param>
    /// <param name="context">The context, whose localizer localizes the message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The message, or null when the value is valid.</returns>
    public static async ValueTask<string?> ValidateAsync(ValidationAttribute attribute, object? value, ValidationContext context,
        CancellationToken cancellationToken = default)
    {
        if (attribute is null)
            throw new ArgumentNullException(nameof(attribute));

        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (attribute is not IAsyncValidationRule asyncRule)
            return Validate(attribute, value, context);

        if (attribute is IClientValidationRule { Side: ValidationSide.Client })
            return null;

        var result = await asyncRule.IsValidAsync(value, context, cancellationToken).ConfigureAwait(false);
        if (result == ValidationResult.Success)
            return null;

        return result?.ErrorMessage ?? ValidationRuleMessage.Format(attribute, context.GetLocalizer(), null, "Validation.Invalid");
    }

    /// <summary>
    /// Validates the properties of an object by what the properties of its type, or of the <see cref="ObjectValidationOptions.RulesType"/>,
    /// e.g. a form, declare: <see cref="RequiredAttribute"/>, <see cref="RequiredWhenAttribute"/> and the validation rules, like
    /// <see cref="ValidateRangeAttribute"/>. As in the browser, a property that is hidden, by <see cref="VisibleAttribute"/>, on
    /// insert or update, or by its <see cref="VisibleWhenAttribute"/>s, or is read only, by
    /// <see cref="System.ComponentModel.ReadOnlyAttribute"/> or its <see cref="ReadOnlyWhenAttribute"/>s, is not checked, and
    /// neither is a rule only checked by the forms.
    /// </summary>
    /// <param name="instance">The object, e.g. a row, whose properties are read by the names of the ones of the rules type.</param>
    /// <param name="options">How it is validated.</param>
    /// <returns>The first property whose value is not valid, or null when they all are.</returns>
    public static ObjectValidationFailure? ValidateObject(object instance, ObjectValidationOptions? options = null)
    {
        foreach (var check in ObjectValidator.GetChecks(instance, options))
        {
            if (check.Required && ObjectValidator.IsMissing(check.Value))
                return new ObjectValidationFailure(check.Name, null);

            foreach (var rule in check.Rules)
                if (Validate(rule, check.Value, check.Context) is { } message)
                    return new ObjectValidationFailure(check.Name, message);
        }

        return null;
    }

    /// <summary>
    /// Validates the properties of an object like <see cref="ValidateObject"/>, awaiting the rules that can be checked
    /// asynchronously, see <see cref="IAsyncValidationRule"/>.
    /// </summary>
    /// <param name="instance">The object.</param>
    /// <param name="options">How it is validated.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The first property whose value is not valid, or null when they all are.</returns>
    public static async ValueTask<ObjectValidationFailure?> ValidateObjectAsync(object instance, ObjectValidationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        foreach (var check in ObjectValidator.GetChecks(instance, options))
        {
            if (check.Required && ObjectValidator.IsMissing(check.Value))
                return new ObjectValidationFailure(check.Name, null);

            foreach (var rule in check.Rules)
                if (await ValidateAsync(rule, check.Value, check.Context, cancellationToken).ConfigureAwait(false) is { } message)
                    return new ObjectValidationFailure(check.Name, message);
        }

        return null;
    }

    private static string? GetOwnMessage(ValidationAttribute attribute)
    {
        return attribute.ErrorMessageResourceType is null && !string.IsNullOrEmpty(attribute.ErrorMessage) ?
            attribute.ErrorMessage : null;
    }

    private static (string Rule, object? Params, string TextKey, object?[] Args)? GetDataAnnotationsRule(ValidationAttribute attribute)
    {
        return attribute switch
        {
            RangeAttribute range => ("Range", new[] { range.Minimum, range.Maximum }, "Validation.Range", [range.Minimum, range.Maximum]),
            StringLengthAttribute length when length.MinimumLength > 0 => ("Length",
                new[] { length.MinimumLength, length.MaximumLength }, "Validation.RangeLength", [length.MinimumLength, length.MaximumLength]),
            StringLengthAttribute length => ("Length", new[] { 0, length.MaximumLength }, "Validation.MaxLength", [length.MaximumLength]),
            System.ComponentModel.DataAnnotations.MaxLengthAttribute { Length: > 0 } max => ("Length", new[] { 0, max.Length }, "Validation.MaxLength", [max.Length]),
            MinLengthAttribute min => ("Length", new int?[] { min.Length, null }, "Validation.MinLength", [min.Length]),
            RegularExpressionAttribute regex => ("Pattern", regex.Pattern, "Validation.Pattern", []),
            EmailAddressAttribute => ("Email", null, "Validation.Email", []),
            UrlAttribute => ("Url", null, "Validation.Url", []),
            CompareAttribute compare => ("Compare", compare.OtherProperty, "Validation.Compare", []),
            _ => null
        };
    }
}
