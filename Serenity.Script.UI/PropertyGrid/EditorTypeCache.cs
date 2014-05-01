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

    public class EditorTypeCache
    {
        private static JsDictionary<string, bool> visited;
        private static JsDictionary<string, EditorTypeInfo> registeredTypes;

        private static void RegisterTypesInNamespace(string ns)
        {
            JsDictionary nsObj = Window.Instance.As<JsDictionary>();
            foreach (var x in ns.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
            {
                nsObj = nsObj[x].As<JsDictionary>();
                if (nsObj == null)
                    return;
            }

            foreach (var k in Object.Keys(nsObj))
            {
                var obj = nsObj.As<JsDictionary>()[k];

                if (obj == null)
                    continue;

                string name = ns + "." + k;

                visited[name] = true;

                if (Script.TypeOf(obj) == "function")
                {
                    var type = Type.GetType(name);
                    if (type == null)
                        continue;

                    var attr = type.GetCustomAttributes(typeof(EditorAttribute), false);
                    if (attr != null && attr.Length > 0)
                        RegisterType(type);
                }
                else
                {
                    RegisterTypesInNamespace(name);
                    continue;
                }
            }
        }

        private static void RegisterType(Type type)
        {
            string name = type.FullName;
            var idx = name.IndexOf('.');
            if (idx >= 0)
                name = name.Substr(idx + 1);

            var info = new EditorTypeInfo
            {
                Type = type
            };

            var displayAttr = type.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            if (displayAttr != null)
                info.DisplayName = ((DisplayNameAttribute)displayAttr[0]).DisplayName;
            else
                info.DisplayName = type.FullName;

            var optionsAttr = type.GetCustomAttributes(typeof(OptionsTypeAttribute), false);
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
                    visited = new JsDictionary<string, bool>();
                    registeredTypes = new JsDictionary<string, EditorTypeInfo>();

                    foreach (var ns in Q.Config.RootNamespaces)
                        RegisterTypesInNamespace(ns);
                }

                return registeredTypes;
            }
        }
    }
}