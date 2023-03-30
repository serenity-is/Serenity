using Serenity.Localization;

namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private string propertyItemsTextPrefix;

    private string GetLocalizableTextValue(IPropertySource source, string text,
        Func<string> getSuffix)
    {
        if (PropertyItemsLocalTextRegistration.MightBeLocalTextKey(text))
            return text;

        if (propertyItemsTextPrefix is null)
        {
            if (source.Property?.ReflectedType is not Type type)
                return null;

            propertyItemsTextPrefix = PropertyItemsLocalTextRegistration
                .GetPropertyItemsTextPrefix(type) ?? "";
        }

        if (string.IsNullOrEmpty(propertyItemsTextPrefix))
            return text;

        if (getSuffix() is not string suffix)
            return text;

        return propertyItemsTextPrefix + suffix;
    }

    private void SetTitle(IPropertySource source, PropertyItem item)
    {
        if (source.Property != null)
        {
            var attr = source.Property.GetCustomAttribute<DisplayNameAttribute>(false);
            if (attr != null)
                item.Title = GetLocalizableTextValue(source, attr.DisplayName, () => source.Property?.Name);
        }

        if (item.Title == null)
        {
            var basedOnField = source.BasedOnField;

            if (basedOnField is not null)
            {
                item.Title = basedOnField.Caption is not null ?
                    basedOnField.Caption.Key : basedOnField.AutoTextKey;
            }
            else
                item.Title = item.Name;
        }
    }
}