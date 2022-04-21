namespace Serenity.Localization
{
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
                var prefix = fields.LocalTextPrefix;

                foreach (var field in row.Fields)
                {
                    LocalText lt = field.Caption;
                    if (lt != null &&
                        !lt.Key.IsEmptyOrNull())
                    {
                        if (lt is InitializedLocalText initialized)
                        {
                            provider.Add(languageID, initialized.Key, initialized.InitialText);
                        }
                        else
                        {
                            if (!lt.Key.StartsWith("Db."))
                            {
                                var key = "Db." + prefix + "." + (field.PropertyName ?? field.Name);
                                provider.Add(languageID, key, lt.Key);
                                field.Caption = new InitializedLocalText(key, lt.Key);
                            }
                        }
                    }
                }

                var displayName = row.GetType().GetCustomAttribute<DisplayNameAttribute>();
                if (displayName != null)
                    provider.Add(languageID, "Db." + prefix + ".EntityPlural", displayName.DisplayName);

                var instanceName = row.GetType().GetCustomAttribute<InstanceNameAttribute>();
                if (instanceName != null)
                    provider.Add(languageID, "Db." + prefix + ".EntitySingular", instanceName.InstanceName);
            }
        }
    }
}