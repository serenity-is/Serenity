
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
            string languageID = LocalText.InvariantLanguageID)
        {
            var provider = Dependency.Resolve<ILocalTextRegistry>();

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
                                provider.Add(languageID, "Enums." + type.Name + "." + name, descAttr.Description);
                        }
                    }
                }
            }
        }

    }
}