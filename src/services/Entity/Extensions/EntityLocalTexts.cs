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

        // a text with a key property of its own, e.g. the placeholder of a quick filter: the text is the default text of
        // the key, or of the implied one when there is no key, and is never taken as a key itself
        void addKeyedText(string? textKey, string? text, string impliedKey)
        {
            if (string.IsNullOrEmpty(textKey))
            {
                if (!string.IsNullOrEmpty(text))
                    provider.Add(languageID, impliedKey, text);
            }
            else if (!string.IsNullOrEmpty(text))
                provider.Add(languageID, textKey, text);
            else if (provider.TryGet(languageID, textKey, false) is null)
                provider.Add(languageID, textKey, null);
        }

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

                if (field.GetAttribute<QuickFilterAttribute>() is QuickFilterAttribute quickFilterAttr)
                    addKeyedText(quickFilterAttr.PlaceholderKey, quickFilterAttr.Placeholder,
                        field.AutoTextKey + "_QuickFilter_Placeholder");

                // the messages of the rules of Serenity, with their implied keys, e.g. Db.Northwind.Customer.Quantity_ValidateRange
                foreach (var ruleAttr in field.CustomAttributes.OfType<System.ComponentModel.DataAnnotations.ValidationAttribute>())
                {
                    if (ValidationRules.HasLocalizableMessage(ruleAttr) && ValidationRules.GetTextKeySuffix(ruleAttr) is { } ruleSuffix)
                        addAttributeText(ruleAttr, ruleAttr.ErrorMessage, field.AutoTextKey + ruleSuffix);
                }

                var displayAttr = field.GetAttribute<DisplayNameAttribute>();
                if (field.Caption is not ILocalText lt)
                {
                    if (languageID != LocalText.InvariantLanguageID)
                        continue;

                    lt = field.Caption = new LocalText(field.PublicName);
                }

                if (displayAttr is IHasTextKey hasTextKey &&
                    !string.IsNullOrEmpty(hasTextKey.TextKey))
                {
                    addAttributeText(displayAttr, displayAttr.DisplayName, field.AutoTextKey);
                    continue;
                }

                if (string.IsNullOrEmpty(lt.Key))
                    continue;

                if (lt.OriginalKey is null)
                {
                    if (displayAttr is not IHasTextKey &&
                        LocalText.IsLocalTextKeyCandidate(lt.Key, uiKeyOnly: true))
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