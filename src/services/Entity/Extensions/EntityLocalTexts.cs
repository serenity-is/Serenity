namespace Serenity.Localization;

/// <summary>
/// Contains extension methods to register local texts for entity rows.
/// </summary>
public static class EntityLocalTexts
{
    /// <summary>
    /// Adds the row texts.
    /// </summary>
    /// <param name="registry">The registry.</param>
    /// <param name="rowInstances">The row instances.</param>
    /// <param name="languageID">The language identifier.</param>
    /// <exception cref="ArgumentNullException">
    /// registry
    /// or
    /// rowInstances
    /// </exception>
    public static void AddRowTexts(this ILocalTextRegistry registry, IEnumerable<IRow> rowInstances,
        string languageID = LocalText.InvariantLanguageID)
    {
        var provider = registry ?? throw new ArgumentNullException(nameof(registry));

        ArgumentNullException.ThrowIfNull(rowInstances);

        void addAttributeText(Attribute attribute, string? text, string defaultKey)
        {
            var resolved = LocalText.ResolveFromAttribute(attribute, text, defaultKey);
            if (resolved.Key is not string key)
            {
                if (!string.IsNullOrEmpty(resolved.Text))
                    provider.Add(languageID, defaultKey, resolved.Text);
                return;
            }

            if (!string.IsNullOrEmpty(resolved.Text))
                provider.Add(languageID, key, resolved.Text);
            else if (resolved.IsKey && provider.TryGet(languageID, key, false) is null)
                provider.Add(languageID, key, null);
        }

        foreach (var row in rowInstances)
        {
            var fields = row.Fields;
            string dbPrefix = "Db." + fields.LocalTextPrefix + ".";

            foreach (var field in fields)
            {
                if (field.GetAttribute<CategoryAttribute>() is CategoryAttribute categoryAttr)
                    addAttributeText(categoryAttr, categoryAttr.Category,
                        dbPrefix + "Categories." + categoryAttr.Category);

                if (field.GetAttribute<TabAttribute>() is TabAttribute tabAttr)
                    addAttributeText(tabAttr, tabAttr.Value, dbPrefix + "Tabs." + tabAttr.Value);

                if (field.GetAttribute<HintAttribute>() is HintAttribute hintAttr)
                    addAttributeText(hintAttr, hintAttr.Hint, field.AutoTextKey + "_Hint");

                if (field.GetAttribute<PlaceholderAttribute>() is PlaceholderAttribute placeholderAttr)
                    addAttributeText(placeholderAttr, placeholderAttr.Value,
                        field.AutoTextKey + "_Placeholder");

                if (field.Caption is not ILocalText lt)
                {
                    if (languageID != LocalText.InvariantLanguageID)
                        continue;

                    lt = field.Caption = new LocalText(field.PropertyName ?? field.Name);
                }

                if (string.IsNullOrEmpty(lt.Key))
                    continue;

                if (lt.OriginalKey is null)
                {
                    if (LocalText.IsLocalTextKeyCandidate(lt.Key, uiKeyOnly: true))
                    {
                        if (registry.TryGet(languageID, lt.Key, false) is null)
                            registry.Add(languageID, lt.Key, null);

                        continue;
                    }

                    lt.ReplaceKey(field.AutoTextKey);
                }

                provider.Add(languageID, lt.Key, lt.OriginalKey);
            }

            var displayName = row.GetType().GetCustomAttribute<DisplayNameAttribute>();
            if (displayName != null)
                addAttributeText(displayName, displayName.DisplayName, dbPrefix + "EntityPlural");

            var instanceName = row.GetType().GetCustomAttribute<InstanceNameAttribute>();
            if (instanceName != null)
                addAttributeText(instanceName, instanceName.InstanceName, dbPrefix + "EntitySingular");
        }
    }
}