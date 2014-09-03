
namespace Serenity
{
    using Localization;
    using System;
    using System.Reflection;

    public static class LocalTextRegistryExtensions
    {
        public static LocalTextPackage InitializeTextClass(this LocalTextPackage package, Type type)
        {
            InitializeTextClass(package, type, "");
            return package;
        }

        private static void InitializeTextClass(LocalTextPackage package, Type type, string prefix)
        {
            foreach (var member in type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var fi = member as FieldInfo;
                if (fi != null &&
                    fi.FieldType == typeof(LocalText))
                {
                    var value = fi.GetValue(null) as LocalText;
                    if (value != null)
                    {
                        package.Add(prefix + fi.Name, value.Key);
                        fi.SetValue(null, new LocalText(prefix + fi.Name));
                    }
                }
            }

            foreach (var nested in type.GetNestedTypes(BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var name = nested.Name;
                if (name.EndsWith("_"))
                    name = name.Substring(0, name.Length - 1);

                InitializeTextClass(package, nested, prefix + name + ".");
            }
        }

        public static void Register(this LocalTextPackage package, bool pendingApproval = false)
        {
            var provider = Dependency.Resolve<ILocalTextProvider>();
            foreach (var entry in package.Entries)
                provider.Add(entry, pendingApproval);
        }
    }
}