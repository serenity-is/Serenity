using System.ComponentModel.DataAnnotations;
using Serenity.Localization;

namespace Serenity.ComponentModel;

/// <summary>
/// Finds what is checked for each property of an object, for <see cref="ValidationRules.ValidateObject"/> and
/// <see cref="ValidationRules.ValidateObjectAsync"/>, which only differ by how they check the rules.
/// </summary>
internal static class ObjectValidator
{
    private static readonly object Missing = new();

    public static IEnumerable<ObjectValidationCheck> GetChecks(object instance, ObjectValidationOptions? options)
    {
        if (instance is null)
            throw new ArgumentNullException(nameof(instance));

        options ??= new ObjectValidationOptions();
        var rulesType = options.RulesType ?? instance.GetType();
        var properties = rulesType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.GetIndexParameters().Length == 0)
            .ToList();

        var values = new Dictionary<string, object?>(StringComparer.Ordinal);
        object? valueOf(string name)
        {
            if (values.TryGetValue(name, out var value))
                return value;

            if (options.GetValue is { } getValue)
                value = getValue(name);
            else
            {
                var property = instance.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                value = property is null || property.GetIndexParameters().Length > 0 ? Missing : property.GetValue(instance);
            }

            values[name] = value;
            return value;
        }

        object? conditionValue(string name) => valueOf(name) is var value && value == Missing ? null : value;
        bool allMet(IEnumerable<PropertyConditionAttribute> conditions) =>
            conditions.All(x => x.ToCondition().IsMet(conditionValue(x.Field)));

        var hidden = GetHidden(properties, options.IsInsert, allMet);
        var textKeyPrefix = options.TextKeyPrefix ?? PropertyItemsLocalTextRegistration.GetPropertyItemsTextPrefix(rulesType);

        foreach (var property in properties)
        {
            var name = property.Name;
            if (hidden.Contains(name) || (options.IncludeProperty is { } include && !include(name)))
                continue;

            var value = valueOf(name);
            if (value == Missing)
                continue;

            // the browser doesn't validate what can't be changed
            if (property.GetCustomAttribute<System.ComponentModel.ReadOnlyAttribute>()?.IsReadOnly == true)
                continue;

            var readOnlyWhen = property.GetCustomAttributes<ReadOnlyWhenAttribute>().ToList();
            if (readOnlyWhen.Count > 0 && allMet(readOnlyWhen))
                continue;

            var requiredWhen = property.GetCustomAttributes<RequiredWhenAttribute>().ToList();
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            // a checkbox is not required, as false is a value of it, like in the browser
            var required = type != typeof(bool) &&
                ((options.CheckRequiredAttribute && property.GetCustomAttribute<RequiredAttribute>()?.IsRequired == true) ||
                 (requiredWhen.Count > 0 && allMet(requiredWhen)));

            var rules = options.CheckRules ? ValidationRules.GetRules(property.GetCustomAttributes()).ToList() : [];
            if (!required && rules.Count == 0)
                continue;

            var items = new Dictionary<object, object?>();
            if (!string.IsNullOrEmpty(textKeyPrefix))
                items[ValidationRuleMessage.PropertyTextKeyItem] = textKeyPrefix + name;

            var context = new ValidationContext(instance, options.Services, items) { MemberName = name };
            yield return new ObjectValidationCheck(name, value, required, rules, context);
        }
    }

    /// <summary>
    /// Gets the properties that are hidden, as the browser hides them: by <see cref="VisibleAttribute"/>, on insert or update,
    /// or by their visibility conditions, where one hidden by its conditions also hides the ones whose conditions are on it.
    /// </summary>
    private static HashSet<string> GetHidden(List<PropertyInfo> properties, bool? isInsert,
        Func<IEnumerable<PropertyConditionAttribute>, bool> allMet)
    {
        var hidden = new HashSet<string>(StringComparer.Ordinal);
        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<VisibleAttribute>()?.Value == false ||
                (isInsert == true && property.GetCustomAttribute<HideOnInsertAttribute>()?.Value == true) ||
                (isInsert == false && property.GetCustomAttribute<HideOnUpdateAttribute>()?.Value == true))
                hidden.Add(property.Name);
        }

        var conditional = properties
            .Select(x => (x.Name, Conditions: x.GetCustomAttributes<VisibleWhenAttribute>().ToList()))
            .Where(x => x.Conditions.Count > 0 && !hidden.Contains(x.Name))
            .ToList();

        var shown = conditional.ToDictionary(x => x.Name, x => allMet(x.Conditions), StringComparer.Ordinal);
        var byConditions = new HashSet<string>(StringComparer.Ordinal);
        // until no more are hidden by the ones they depend on, which a cycle of them can't keep going beyond
        for (var pass = 0; pass <= conditional.Count; pass++)
        {
            var changed = false;
            foreach (var (name, conditions) in conditional)
            {
                var isHidden = !shown[name] || conditions.Any(x => x.Field != name && byConditions.Contains(x.Field));
                if (isHidden != byConditions.Contains(name))
                {
                    if (isHidden)
                        byConditions.Add(name);
                    else
                        byConditions.Remove(name);
                    changed = true;
                }
            }

            if (!changed)
                break;
        }

        hidden.UnionWith(byConditions);
        return hidden;
    }

    /// <summary>
    /// Gets whether a required value is missing: null, an empty or white space text, or an empty list.
    /// </summary>
    public static bool IsMissing(object? value)
    {
        return value switch
        {
            null => true,
            string s => string.IsNullOrWhiteSpace(s),
            System.Collections.ICollection { Count: 0 } => true,
            _ => false
        };
    }
}
