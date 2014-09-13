
namespace Serenity.Localization
{
    using Localization;
    using Serenity.Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    public static class EntityLocalTexts
    {
        public static void Initialize(string languageID = LocalText.InvariantLanguageID)
        {
            var provider = Dependency.Resolve<ILocalTextProvider>();

            foreach (var row in RowRegistry.EnumerateRows())
            {
                var fields = row.GetFields();
                var prefix = fields.LocalTextPrefix;

                foreach (var field in row.GetFields())
                {
                    LocalText lt = field.Caption;
                    if (lt != null &&
                        !lt.Key.IsEmptyOrNull() &&
                        !lt.Key.StartsWith("Db."))
                    {
                        var key = "Db." + prefix + "." + (field.PropertyName ?? field.Name);
                        provider.Add(new LocalTextEntry(languageID, key, lt.Key), false);
                        field.Caption = new LocalText(key);
                    }
                }

                var displayName = row.GetType().GetCustomAttribute<DisplayNameAttribute>();
                if (displayName != null)
                    provider.Add(new LocalTextEntry(languageID, "Db." + prefix + ".EntityPlural", displayName.DisplayName), false);

                var instanceName = row.GetType().GetCustomAttribute<InstanceNameAttribute>();
                if (instanceName != null)
                    provider.Add(new LocalTextEntry(languageID, "Db." + prefix + ".EntitySingular", instanceName.InstanceName), false);
            }
        }

    }
}