using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public static class EditorTypeRegistry
    {
        internal static JsDictionary<string, Type> knownTypes;

        public static Type Get(string key)
        {
            if (key.IsEmptyOrNull())
                throw new ArgumentNullException("key");

            Initialize();

            var editorType = knownTypes[key.ToLower()];
            if (editorType == null)
                throw new Exception(String.Format("Can't find {0} editor type!", key));

            return editorType;
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
                    if (!type.IsSubclassOf(typeof(Widget)))
                        continue;

                    if (type.IsGenericTypeDefinition)
                        continue;

                    var fullName = type.FullName.ToLower();
                    knownTypes[fullName] = type;

                    var editorAttr = type.GetCustomAttributes(typeof(EditorAttribute), false);
                    if (editorAttr != null && editorAttr.Length > 0)
                    {
                        var attrKey = ((EditorAttribute)editorAttr[0]).Key;
                        if (!attrKey.IsEmptyOrNull())
                            knownTypes[attrKey.ToLower()] = type;
                    }
                            
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

            SetTypeKeysWithoutEditorSuffix();
        }

        public static void Reset()
        {
            knownTypes = null;
        }

        private static void SetTypeKeysWithoutEditorSuffix()
        {
            const string suffix = "editor";

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