using Serenity.Web;

namespace Serenity.Localization;

/// <summary>
/// Contains initialization methods for adding local text keys implicitly defined by
/// <see cref="DisplayNameAttribute"/>, <see cref="TabAttribute"/>, <see cref="PlaceholderAttribute"/>,
/// <see cref="HintAttribute"/>, etc. attributes used in Form definitions.
/// </summary>
public static class PropertyItemsLocalTextRegistration
{
    /// <summary>
    /// Adds local text translations defined implicitly by <see cref="DisplayNameAttribute"/>,
    /// <see cref="TabAttribute"/>, <see cref="PlaceholderAttribute"/>, <see cref="HintAttribute"/>,
    /// etc. attributes used in Column/Form definitions.
    /// </summary>
    /// <param name="registry">The registry to add texts to.</param>
    /// <param name="typeSource">The type source to search for property item types in.</param>
    /// <param name="languageID">The language ID texts will be added for (default is the invariant language).</param>
    public static void AddPropertyItemsTexts(this ILocalTextRegistry registry, ITypeSource typeSource,
        string languageID = LocalText.InvariantLanguageID)
    {
        if (typeSource == null)
            throw new ArgumentNullException(nameof(typeSource));

        if (registry is null)
            throw new ArgumentNullException(nameof(registry));

        foreach (var type in typeSource.GetTypes())
        {
            if (GetPropertyItemsTextPrefix(type) is not string textPrefix)
                continue;

            void addText(string text, string? suffix)
            {
                if (string.IsNullOrEmpty(text))
                    return;

                if (IsLocalTextKeyCandidate(text))
                {
                    if (registry.TryGet(languageID, text, false) is null)
                        registry.Add(languageID, text, null);
                }
                else if (suffix is not null)
                    registry.Add(languageID, textPrefix + suffix, text);
            }

            void addAttributeText(Attribute attribute, string? text, string suffix)
            {
                var resolved = LocalText.ResolveFromAttribute(attribute, text, textPrefix + suffix);
                if (resolved.Key is not string key)
                {
                    if (!string.IsNullOrEmpty(resolved.Text))
                        registry.Add(languageID, textPrefix + suffix, resolved.Text);
                    return;
                }

                if (!string.IsNullOrEmpty(resolved.Text))
                    registry.Add(languageID, key, resolved.Text);
                else if (resolved.IsKey && registry.TryGet(languageID, key, false) is null)
                    registry.Add(languageID, key, null);
            }

            var addonParams = new Dictionary<string, object?>(StringComparer.Ordinal);

            foreach (var member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public))
            {
                if (member.GetCustomAttribute<CategoryAttribute>() is CategoryAttribute categoryAttr)
                    addAttributeText(categoryAttr, categoryAttr.Category,
                        "Categories." + categoryAttr.Category);

                if (member.GetCustomAttribute<TabAttribute>() is TabAttribute tabAttr)
                    addAttributeText(tabAttr, tabAttr.Value, "Tabs." + tabAttr.Value);

                if (member.GetCustomAttribute<DisplayNameAttribute>() is DisplayNameAttribute displayAttr)
                    addAttributeText(displayAttr, displayAttr.DisplayName, member.Name);

                if (member.GetCustomAttribute<HintAttribute>() is HintAttribute hintAttr)
                    addAttributeText(hintAttr, hintAttr.Hint, member.Name + "_Hint");

                if (member.GetCustomAttribute<PlaceholderAttribute>() is PlaceholderAttribute placeholderAttr)
                    addAttributeText(placeholderAttr, placeholderAttr.Value, member.Name + "_Placeholder");

                foreach (var addonAttr in member.GetCustomAttributes<EditorAddonAttribute>())
                {
                    addonParams.Clear();
                    addonAttr.SetParams(addonParams);
                    foreach (var pair in addonParams)
                    {
                        if (pair.Value is string s &&
                            addonAttr.IsLocalizableOption(pair.Key))
                        {
                            addText(s, suffix: null);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the form/column local text key prefix for the given type.
    /// </summary>
    /// <param name="type">The type with a form or column attribute.</param>
    /// <returns>The local text key prefix, or <c>null</c> if the type has neither a form nor a column attribute.</returns>
    public static string? GetPropertyItemsTextPrefix(Type type)
    {
        var formAttr = type.GetCustomAttribute<FormScriptAttribute>(inherit: false);
        string? itemsKey;
        if (formAttr is null)
        {
            var columnsAttr = type.GetCustomAttribute<ColumnsScriptAttribute>(inherit: false);
            if (columnsAttr is null)
                return null;

            if (columnsAttr.LocalTextPrefix is not null)
                return columnsAttr.LocalTextPrefix;

            itemsKey = columnsAttr.Key;
        }
        else if (formAttr.LocalTextPrefix is not null)
            return formAttr.LocalTextPrefix;
        else
            itemsKey = formAttr.Key;

        if (string.IsNullOrEmpty(itemsKey))
            itemsKey = type.FullName;

        return (formAttr is null ? "Columns." : "Forms.") + itemsKey + ".";
    }

    /// <summary>
    /// Returns <c>true</c> if the text value can be a local text key
    /// that could be passed to the client side.
    /// </summary>
    /// <param name="text">The key or text.</param>
    /// <param name="uiKeyOnly">If set to <c>true</c>, only considers keys with UI-related prefixes.</param>
    /// <returns><c>true</c> if the text looks like a local text key; otherwise, <c>false</c>.</returns>
    public static bool IsLocalTextKeyCandidate(string? text, bool uiKeyOnly = true)
    {
        return LocalText.IsLocalTextKeyCandidate(text, uiKeyOnly);
    }
}