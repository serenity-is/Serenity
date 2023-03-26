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

        foreach (var row in rowInstances)
        {
            var fields = row.Fields;

            foreach (var field in fields)
            {
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
                    if (lt.Key.StartsWith("Db."))
                        continue;

                    lt.ReplaceKey(field.AutoTextKey);
                }

                provider.Add(languageID, lt.Key, lt.OriginalKey);
            }

            var displayName = row.GetType().GetCustomAttribute<DisplayNameAttribute>();
            if (displayName != null)
                provider.Add(languageID, "Db." + fields.LocalTextPrefix + ".EntityPlural", 
                    displayName.DisplayName);

            var instanceName = row.GetType().GetCustomAttribute<InstanceNameAttribute>();
            if (instanceName != null)
                provider.Add(languageID, "Db." + fields.LocalTextPrefix + ".EntitySingular",
                    instanceName.InstanceName);
        }
    }
}