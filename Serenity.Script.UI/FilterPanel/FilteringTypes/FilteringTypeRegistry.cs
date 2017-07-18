using System;
using System.Collections.Generic;

namespace Serenity
{
    public static class FilteringTypeRegistry
    {
        internal static JsDictionary<string, Type> knownTypes;

        public static Type Get(string key)
        {
            if (key.IsEmptyOrNull())
                throw new ArgumentNullException("key");

            Initialize();

            var formatterType = knownTypes[key.ToLower()];
            if (formatterType == null)
                throw new Exception(String.Format("Can't find {0} filter handler type!", key));

            return formatterType;
        }

        internal static void Initialize()
        {
            if (knownTypes != null)
                return;

            knownTypes = new JsDictionary<string, Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(IFiltering).IsAssignableFrom(type))
                        continue;

                    if (type.IsGenericTypeDefinition)
                        continue;

                    var fullName = type.FullName.ToLower();
                    knownTypes[fullName] = type;

                    foreach (var k in Q.Config.RootNamespaces)
                    {
                        if (fullName.StartsWith(k.ToLower() + "."))
                        {
                            var kx = fullName.Substr(k.Length + 1).ToLower();
                            if (knownTypes[kx] == null)
                                knownTypes[kx] = type;
                        }
                    }
                }
            }

            SetTypeKeysWithoutFilterHandlerSuffix();
        }

        public static void Reset()
        {
            knownTypes = null;
        }

        private static void SetTypeKeysWithoutFilterHandlerSuffix()
        {
            const string suffix = "filtering";

            foreach (var k in Object.Keys(knownTypes))
            {
                if (!k.EndsWith(suffix))
                    continue;

                var p = k.Substr(0, k.Length - suffix.Length);
                if (p.IsEmptyOrNull())
                    continue;

                if (knownTypes[p] != null)
                    continue;

                knownTypes[p] = knownTypes[k];
            }
        }
    }
}