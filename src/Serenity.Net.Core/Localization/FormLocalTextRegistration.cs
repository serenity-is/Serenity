namespace Serenity.Localization;

/// <summary>
/// Contains initialization method for adding local text keys implicitly defined by
/// DisplayName, Tab, Placeholder, Hint etc. attributes used in Form definitions
/// </summary>
public static class FormLocalTextRegistration
{
    private static readonly Regex LocalTextKeyLike = new(@"^([A-Z][A-za-z0-9]*\.)+[A-Z][A-za-z0-9]*$", RegexOptions.Compiled);

    /// <summary>
    /// Adds local text translations defined implicitly by DisplayName, Tab, Placeholder,
    /// Hint etc. attributes used in Form definitions.
    /// </summary>
    /// <param name="typeSource">Type source to search for enumeration classes in</param>
    /// <param name="languageID">Language ID texts will be added (default is invariant language)</param>
    /// <param name="registry">Registry</param>
    public static void AddFormTexts(this ILocalTextRegistry registry, ITypeSource typeSource,
        string languageID = LocalText.InvariantLanguageID)
    {
        if (typeSource == null)
            throw new ArgumentNullException(nameof(typeSource));

        if (registry is null)
            throw new ArgumentNullException(nameof(registry));

        foreach (var type in typeSource.GetTypes())
        {
            var formAttr = type.GetAttribute<FormScriptAttribute>();
            if (formAttr is null)
                continue;

            var formKey = formAttr.Key ?? type.FullName;
            if (string.IsNullOrEmpty(formKey))
                continue;

            var formPrefix = "Forms." + formKey + ".";

            void addKey(string key)
            {
                if (registry.TryGet(languageID, key, false) is not null)
                    return;

                registry.Add(languageID, key, null);
            }

            void addText(string key, string text)
            {
                registry.Add(languageID, key, text);
            }

            foreach (var member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public))
            {
                var category = member.GetCustomAttribute<CategoryAttribute>();
                if (category != null && !string.IsNullOrEmpty(category.Category))
                {
                    if (LocalTextKeyLike.IsMatch(category.Category))
                        addKey(category.Category);
                    else
                        addText(formPrefix + "Categories." + category.Category, category.Category);
                }

                var tab = member.GetCustomAttribute<TabAttribute>();
                if (tab != null && !string.IsNullOrEmpty(tab.Value))
                {
                    if (LocalTextKeyLike.IsMatch(tab.Value))
                        addKey(tab.Value);
                    else
                        addText(formPrefix + "Tabs." + tab.Value, tab.Value);
                }

                var displayName = member.GetCustomAttribute<DisplayNameAttribute>();
                if (displayName != null && !string.IsNullOrEmpty(displayName.DisplayName))
                {
                    if (LocalTextKeyLike.IsMatch(displayName.DisplayName))
                        addKey(displayName.DisplayName);
                    else
                        addText(formPrefix + member.Name, displayName.DisplayName);
                }

                var hint = member.GetCustomAttribute<HintAttribute>();
                if (hint != null && !string.IsNullOrEmpty(hint.Hint))
                {
                    if (LocalTextKeyLike.IsMatch(hint.Hint))
                        addKey(hint.Hint);
                    else
                        addText(formPrefix + member.Name + "_Hint", hint.Hint);
                }

                var plholder = member.GetCustomAttribute<PlaceholderAttribute>();
                if (plholder != null && !string.IsNullOrEmpty(plholder.Value))
                {
                    if (LocalTextKeyLike.IsMatch(plholder.Value))
                        addKey(plholder.Value);
                    else
                        addText(formPrefix + member.Name + "_Placeholder", plholder.Value);
                }
            }
        }
    }
}