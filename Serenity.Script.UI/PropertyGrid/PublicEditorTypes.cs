using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable]
    public class EditorTypeInfo
    {
        public Type Type;
        public string DisplayName;
        public Type OptionsType;
    }

    public class PublicEditorTypes
    {
        private static JsDictionary<string, EditorTypeInfo> registeredTypes;

        private static void RegisterType(Type type)
        {
            string name = type.FullName;

            var info = new EditorTypeInfo
            {
                Type = type
            };

            var displayAttr = type.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (displayAttr != null && displayAttr.Length > 0)
                info.DisplayName = ((DisplayNameAttribute)displayAttr[0]).DisplayName;
            else
                info.DisplayName = type.FullName;

            var optionsAttr = type.GetCustomAttributes(typeof(OptionsTypeAttribute), true);
            if (optionsAttr != null && optionsAttr.Length > 0)
                info.OptionsType = ((OptionsTypeAttribute)optionsAttr[0]).OptionsType;

            registeredTypes[name] = info;
        }

        public static JsDictionary<string, EditorTypeInfo> RegisteredTypes
        {
            get
            {
                if (registeredTypes == null)
                {
                    registeredTypes = new JsDictionary<string, EditorTypeInfo>();

                    EditorTypeRegistry.Initialize();
                    foreach (var pair in EditorTypeRegistry.knownTypes)
                    {
                        if (registeredTypes.ContainsKey(pair.Value.FullName))
                            continue;

                        RegisterType(pair.Value);
                    }
                }

                return registeredTypes;
            }
        }
    }
}