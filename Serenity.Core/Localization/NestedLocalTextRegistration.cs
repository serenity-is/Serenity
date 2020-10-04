
namespace Serenity.Localization
{
    using Extensibility;
    using Serenity.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Contains helper methods for registration of local texts in nested static classes.
    /// Nested static contains LocalText objects with actual translations as keys. This class locates
    /// them (with NestedLocalTextsAttribute at outermost class), determines keys by path from outermost
    /// to nested class name, replaces existing LocalText instance with a InitializedLocalText instance
    /// containing this generated key and initial translation, and registers this translation in 
    /// ILocalTextRegistry provider.
    /// </summary>
    public static class NestedLocalTextRegistration
    {
        /// <summary>
        /// Adds translations from static nested local text classes marked with NestedLocalTextAttribute.
        /// </summary>
#if !NET45
        public static void AddNestedTexts(this ILocalTextRegistry registry, IEnumerable<Assembly> assemblies = null)
#else
        public static void Initialize(IEnumerable<Assembly> assemblies, ILocalTextRegistry registry = null)
#endif
        {
            assemblies = assemblies ?? ExtensibilityHelper.SelfAssemblies;

            if (assemblies == null)
                throw new ArgumentNullException("assemblies");

            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<NestedLocalTextsAttribute>();
                    if (attr != null)
                    {
                        Initialize(registry, type, attr.LanguageID ?? LocalText.InvariantLanguageID, attr.Prefix ?? "");
                    }
                }
        }

        private static void Initialize(ILocalTextRegistry registry, Type type, string languageID, string prefix)
        {
            var provider = registry ?? Dependency.Resolve<ILocalTextRegistry>();
            foreach (var member in type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var fi = member as FieldInfo;
                if (fi != null &&
                    fi.FieldType == typeof(LocalText))
                {
                    var value = fi.GetValue(null) as LocalText;
                    if (value != null)
                    {
                        var initialized = value as InitializedLocalText;
                        if (initialized != null)
                        {
                            provider.Add(languageID, initialized.Key, initialized.InitialText);
                        }
                        else
                        {
                            provider.Add(languageID, prefix + fi.Name, value.Key);
                            fi.SetValue(null, new InitializedLocalText(prefix + fi.Name, value.Key));
                        }
                    }
                }
            }

            foreach (var nested in type.GetNestedTypes(BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var name = nested.Name;
                if (name.EndsWith("_"))
                    name = name.Substring(0, name.Length - 1);

                Initialize(registry, nested, languageID, prefix + name + ".");
            }
        }
    }
}