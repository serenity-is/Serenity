
namespace Serenity.Localization
{
    using Serenity.Abstractions;
    using Serenity.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>
    /// Contains initialization method for adding local text translations defined by
    /// Description attributes in enumeration classes.
    /// </summary>
    public static class EnumLocalTextRegistration
    {
        /// <summary>
        /// Adds local text translations defined implicitly by Description attributes in 
        /// enumeration classes.
        /// </summary>
        /// <param name="assemblies">Assemblies to search for enumeration classes in</param>
        /// <param name="languageID">Language ID texts will be added (default is invariant language)</param>
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
                        var enumKeyAttr = type.GetCustomAttribute<EnumKeyAttribute>();
                        var enumKey = enumKeyAttr != null ? enumKeyAttr.Value : type.FullName;

                        foreach (var name in Enum.GetNames(type))
                        {
                            var member = type.GetMember(name);
                            if (member.Length == 0)
                                continue;

                            var descAttr = member[0].GetCustomAttribute<DescriptionAttribute>();
                            if (descAttr != null)
                                provider.Add(languageID, "Enums." + enumKey + "." + name, descAttr.Description);
                        }
                    }
                }
            }
        }

    }
}