using Serenity.Localization;

namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetValidationRules(IPropertySource source, PropertyItem item)
    {
        var all = source.GetAttributes<Attribute>().ToList();
        List<ClientValidationRule>? rules = null;
        List<Attribute>? own = null;

        foreach (var attribute in ValidationRules.GetRules(all))
        {
            if (ValidationRules.GetClientRule(attribute) is not { } rule)
                continue;

            // a message of its own is a localizable text with the implied key of the rule, e.g. Quantity_ValidateRange
            if (string.IsNullOrEmpty(rule.TextKey) && ValidationRules.HasLocalizableMessage(attribute) &&
                ValidationRules.GetTextKeySuffix(attribute) is { } suffix)
            {
                own ??= [.. source.GetAttributes<Attribute>(AttributeOrigin.ExcludeBasedOnField)];
                var propertyKey = GetValidationPropertyTextKey(source, fromField: !own.Contains(attribute));
                if (propertyKey is not null)
                    rule.TextKey = propertyKey + suffix;
            }

            (rules ??= []).Add(rule);
        }

        item.ValidationRules = rules;
    }

    /// <summary>
    /// Gets the text key of a property, which the implied text keys of its rules start with: the one of the field of the row
    /// for a rule of the field, e.g. <c>Db.Northwind.Customer.Quantity</c>, else the one of the property in its form or
    /// columns, e.g. <c>Forms.Northwind.Customer.Quantity</c>, or null for a type that is neither.
    /// </summary>
    private string? GetValidationPropertyTextKey(IPropertySource source, bool fromField)
    {
        if (fromField && source.BasedOnField is { } field)
            return field.AutoTextKey;

        if (propertyItemsTextPrefix is null)
        {
            if (source.ReflectedType is not Type type)
                return null;

            propertyItemsTextPrefix = PropertyItemsLocalTextRegistration.GetPropertyItemsTextPrefix(type) ?? "";
        }

        return string.IsNullOrEmpty(propertyItemsTextPrefix) ? null : propertyItemsTextPrefix + source.Name;
    }
}
