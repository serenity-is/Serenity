
namespace Serenity.Localization
{
    using Localization;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    public static class EnumLocalTexts
    {
        public static void Initialize(IEnumerable<Assembly> assemblies,
            long languageID = LocalText.DefaultLanguageID)
        {
            var provider = Dependency.Resolve<ILocalTextProvider>();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsEnum)
                    {
                        foreach (var name in Enum.GetNames(type))
                        {
                            var member = type.GetMember(name);
                            if (member.Length == 0)
                                continue;

                            var descAttr = member[0].GetCustomAttribute<DescriptionAttribute>();
                            if (descAttr != null)
                                provider.Add(new LocalTextEntry(languageID, "Enums." + type.Name + "." + name, descAttr.Description), false);
                        }
                    }
                }
            }
        }

    }
}