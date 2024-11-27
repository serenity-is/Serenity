using Serenity.Localization;

namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private string propertyItemsTextPrefix;

    private string GetLocalizableTextValue<TAttribute>(IPropertySource source, string text,
        Func<string> getSuffix, bool ignoreField = false)
        where TAttribute : Attribute
    {
        if (PropertyItemsLocalTextRegistration.IsLocalTextKeyCandidate(text))
            return text;

        if (string.IsNullOrEmpty(text))
            return text;

        bool fromField = !ignoreField && 
            source.BasedOnField is not null &&
            source.Property?.GetAttribute<TAttribute>(false) is null &&
            source.BasedOnField.GetAttribute<TAttribute>() is not null;

        if (!fromField)
        {
            if (propertyItemsTextPrefix is null)
            {
                if (source.Property?.ReflectedType is not Type type)
                    return null;

                propertyItemsTextPrefix = PropertyItemsLocalTextRegistration
                    .GetPropertyItemsTextPrefix(type) ?? "";
            }

            if (string.IsNullOrEmpty(propertyItemsTextPrefix))
                return text;
        }

        if (getSuffix() is not string suffix)
            return text;

        return (fromField ? ("Db." + source.BasedOnField.Fields.LocalTextPrefix + ".") :
            propertyItemsTextPrefix) + suffix;
    }

    private void SetTitle(IPropertySource source, PropertyItem item)
    {
        var attr = source.Property?.GetCustomAttribute<DisplayNameAttribute>(false);
        if (attr != null)
        {
            item.Title = GetLocalizableTextValue<DisplayNameAttribute>(source, attr.DisplayName,
                () => source.Property?.Name, ignoreField: true);
            return;
        }

        if (item.Title == null)
        {
            var basedOnField = source.BasedOnField;

            if (basedOnField is not null)
                item.Title = basedOnField.Caption?.Key ?? basedOnField.AutoTextKey;
            else
                item.Title = source.Property?.Name;
        }
    }
}