
namespace Serenity.Localization
{
    using Localization;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class NestedLocalTexts
    {
        public static void Initialize(Type type, long languageID = LocalText.DefaultLanguageID)
        {
            Initialize(type, languageID, "");
        }

        private static void Initialize(Type type, long languageID, string prefix)
        {
            var provider = Dependency.Resolve<ILocalTextProvider>();
            foreach (var member in type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var fi = member as FieldInfo;
                if (fi != null &&
                    fi.FieldType == typeof(LocalText))
                {
                    var value = fi.GetValue(null) as LocalText;
                    if (value != null)
                    {
                        provider.Add(new LocalTextEntry(languageID, prefix + fi.Name, value.Key), false);
                        fi.SetValue(null, new LocalText(prefix + fi.Name));
                    }
                }
            }

            foreach (var nested in type.GetNestedTypes(BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var name = nested.Name;
                if (name.EndsWith("_"))
                    name = name.Substring(0, name.Length - 1);

                Initialize(nested, languageID, prefix + name + ".");
            }
        }
    }
}