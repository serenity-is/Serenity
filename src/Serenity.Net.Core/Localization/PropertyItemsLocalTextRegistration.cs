using Serenity.Web;
using static System.Net.Mime.MediaTypeNames;

namespace Serenity.Localization;

/// <summary>
/// Contains initialization method for adding local text keys implicitly defined by
/// DisplayName, Tab, Placeholder, Hint etc. attributes used in Form definitions
/// </summary>
public static class PropertyItemsLocalTextRegistration
{
    private static readonly Regex LocalTextKeyLike = new(@"^([A-Z][A-za-z0-9]*\.)+[A-Z][A-za-z0-9]*$", RegexOptions.Compiled);

    /// <summary>
    /// Adds local text translations defined implicitly by DisplayName, Tab, Placeholder,
    /// Hint etc. attributes used in Column/Form etc. definitions.
    /// </summary>
    /// <param name="typeSource">Type source to search for enumeration classes in</param>
    /// <param name="languageID">Language ID texts will be added (default is invariant language)</param>
    /// <param name="registry">Registry</param>
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

            void addText(string key, string text)
            {
                registry.Add(languageID, key, text);
            }

            bool tryAddKey(string text)
            {
                if (MightBeLocalTextKey(text))
                {
                    if (registry.TryGet(languageID, text, false) is null)
                        registry.Add(languageID, text, null);
                    return true;
                }

                return false;
            }

            foreach (var member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public))
            {
                var category = member.GetCustomAttribute<CategoryAttribute>();
                if (category != null && !string.IsNullOrEmpty(category.Category))
                {
                    if (!tryAddKey(category.Category))
                        addText(textPrefix + "Categories." + category.Category, category.Category);
                }

                var tab = member.GetCustomAttribute<TabAttribute>();
                if (tab != null && !string.IsNullOrEmpty(tab.Value))
                {
                    if (!tryAddKey(tab.Value))
                        addText(textPrefix + "Tabs." + tab.Value, tab.Value);
                }

                var displayName = member.GetCustomAttribute<DisplayNameAttribute>();
                if (displayName != null && !string.IsNullOrEmpty(displayName.DisplayName))
                {
                    if (!tryAddKey(displayName.DisplayName))
                        addText(textPrefix + member.Name, displayName.DisplayName);
                }

                var hint = member.GetCustomAttribute<HintAttribute>();
                if (hint != null && !string.IsNullOrEmpty(hint.Hint))
                {
                    if (!tryAddKey(hint.Hint))
                        addText(textPrefix + member.Name + "_Hint", hint.Hint);
                }

                var plholder = member.GetCustomAttribute<PlaceholderAttribute>();
                if (plholder != null && !string.IsNullOrEmpty(plholder.Value))
                {
                    if (!tryAddKey(plholder.Value))
                        addText(textPrefix + member.Name + "_Placeholder", plholder.Value);
                }
            }
        }
    }

    /// <summary>
    /// Gets form/column local text key prefix for given type
    /// </summary>
    /// <param name="type">Type with form/column attribute</param>
    public static string? GetPropertyItemsTextPrefix(Type type)
    {
        var formAttr = type.GetAttribute<FormScriptAttribute>();
        string? itemsKey;
        if (formAttr is null)
        {
            var columnsAttr = type.GetAttribute<ColumnsScriptAttribute>();
            if (columnsAttr is null)
                return null;

            itemsKey = columnsAttr.Key;
        }
        else
            itemsKey = formAttr.Key;

        if (string.IsNullOrEmpty(itemsKey))
            itemsKey = type.FullName;

        return (formAttr is null ? "Columns." : "Forms.") + itemsKey + ".";
    }

    /// <summary>
    /// Returns true if the text value can be a local text key
    /// that could be passed to the client side
    /// </summary>
    /// <param name="text">Key or text</param>
    /// <returns></returns>
    public static bool MightBeLocalTextKey(string text)
    {
        return !string.IsNullOrEmpty(text) &&
            LocalTextKeyLike.IsMatch(text) &&
            LocalTextPackages.DefaultSitePackageIncludes.IsMatch(text);
    }
}