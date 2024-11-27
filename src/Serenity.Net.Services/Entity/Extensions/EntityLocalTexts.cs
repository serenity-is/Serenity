namespace Serenity.Localization;

/// <summary>
/// EntityLocalTexts
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

        if (rowInstances == null)
            throw new ArgumentNullException(nameof(rowInstances));

        void addText(string text, string key)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (PropertyItemsLocalTextRegistration.IsLocalTextKeyCandidate(text))
            {
                if (registry.TryGet(languageID, text, false) is null)
                    registry.Add(languageID, text, null);
            }
            else if (key is not null)
                registry.Add(languageID, key, text);
        }

        foreach (var row in rowInstances)
        {
            var fields = row.Fields;
            string dbPrefix = "Db." + fields.LocalTextPrefix + ".";

            foreach (var field in fields)
            {
                if (field.GetAttribute<CategoryAttribute>()?.Category is string category)
                    addText(category, dbPrefix + "Categories." + category);

                if (field.GetAttribute<TabAttribute>()?.Value is string tab)
                    addText(tab, dbPrefix + "Tabs." + tab);

                if (field.GetAttribute<HintAttribute>()?.Hint is string hint)
                    addText(hint, field.AutoTextKey + "_Hint");

                if (field.GetAttribute<PlaceholderAttribute>()?.Value is string placeholder)
                    addText(placeholder, field.AutoTextKey + "_Placeholder");

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
                    if (PropertyItemsLocalTextRegistration.IsLocalTextKeyCandidate(lt.Key))
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
                provider.Add(languageID, dbPrefix + "EntityPlural", 
                    displayName.DisplayName);

            var instanceName = row.GetType().GetCustomAttribute<InstanceNameAttribute>();
            if (instanceName != null)
                provider.Add(languageID, dbPrefix + "EntitySingular",
                    instanceName.InstanceName);
        }
    }
}