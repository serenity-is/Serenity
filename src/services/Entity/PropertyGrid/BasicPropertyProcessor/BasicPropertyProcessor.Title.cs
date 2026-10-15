using Serenity.Localization;

namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private string? propertyItemsTextPrefix;

    private string? GetLocalizableTextValue<TAttribute>(IPropertySource source, string? text,
        Func<string> getSuffix, bool ignoreField = false, bool inferTextKey = true)
        where TAttribute : Attribute
    {
        if (inferTextKey && PropertyItemsLocalTextRegistration.IsLocalTextKeyCandidate(text))
            return text;

        if (string.IsNullOrEmpty(text))
            return text;

        bool fromField = !ignoreField &&
            source.GetAttribute<TAttribute>(AttributeOrigin.ExcludeBasedOnField) is null &&
            source.GetAttribute<TAttribute>() is not null;

        if (!fromField)
        {
            if (propertyItemsTextPrefix is null)
            {
                if (source.ReflectedType is not Type type)
                    return null;

                propertyItemsTextPrefix = PropertyItemsLocalTextRegistration
                    .GetPropertyItemsTextPrefix(type) ?? "";
            }

            if (string.IsNullOrEmpty(propertyItemsTextPrefix))
                return text;
        }

        if (getSuffix() is not string suffix)
            return text;

        return (fromField ? ("Db." + source.BasedOnField!.Fields.LocalTextPrefix + ".") :
            propertyItemsTextPrefix) + suffix;
    }

    private void SetTitle(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<DisplayNameAttribute>(AttributeOrigin.ExcludeBasedOnField);
        if (attr != null)
        {
            if (attr is IHasTextKey hasTextKey && !string.IsNullOrEmpty(hasTextKey.TextKey))
            {
                item.Title = hasTextKey.TextKey;
                return;
            }

            item.Title = GetLocalizableTextValue<DisplayNameAttribute>(source, attr.DisplayName,
                () => source.Name, ignoreField: true, inferTextKey: attr is not IHasTextKey);
            return;
        }

        if (item.Title == null)
        {
            var basedOnField = source.BasedOnField;

            if (basedOnField is not null)
                item.Title = basedOnField.Caption?.Key ?? basedOnField.AutoTextKey;
            else
                item.Title = source.Name;
        }
    }
}