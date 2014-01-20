using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    public static partial class Q
    {
        public static class ScriptData
        {
            private static JsDictionary<string, string> registered = new JsDictionary<string, string>();
            private static JsDictionary<string, object> loadedData = new JsDictionary<string, object>();

            private static void LoadScriptData(string name)
            {
                if (!registered.ContainsKey(name))
                    throw new Exception(String.Format("Script data {0} is not found in registered script list!", name));

                name = name + ".js?" + registered[name];

                SyncLoadScript(Q.ResolveUrl("~/DynJS.axd/") + name);
            }

            private static void SyncLoadScript(string url)
            {
                jQuery.Ajax(new jQueryAjaxOptions
                {
                    Async = false,
                    Type = "GET",
                    Url = url,
                    Data = null,
                    DataType = "script"
                });
            }

            public static object Ensure(string name)
            {
                var data = loadedData[name];

                if (!Script.IsValue(data))
                    LoadScriptData(name);

                data = loadedData[name];

                if (!Script.IsValue(data))
                    throw new NotSupportedException(String.Format("Can't load script data: {0}!", name));

                return data;
            }

            public static object Reload(string name)
            {
                if (!registered.ContainsKey(name))
                    throw new NotSupportedException(String.Format("Script data {0} is not found in registered script list!"));

                registered[name] = new JsDate().GetTime().ToString();

                LoadScriptData(name);

                var data = loadedData[name];

                if (!Script.IsValue(data))
                    throw new NotSupportedException(String.Format("Can't load script data: {0}!", name));

                return data;
            }

            public static bool CanLoad(string name)
            {
                var data = loadedData[name];

                if (Script.IsValue(data) ||
                    registered.ContainsKey(name))
                    return true;

                return false;
            }

            public static void SetRegisteredScripts(JsDictionary<string, string> scripts)
            {
                registered.Clear();
                foreach (var k in scripts)
                    Q.ScriptData.registered[k.Key] = k.Value.ToString();
            }

            public static void Set(string name, object value)
            {
                Q.ScriptData.loadedData[name] = value;
            }
        }

        [IncludeGenericArguments(false)]
        public static TData GetRemoteData<TData>(string key)
        {
            return ScriptData.Ensure("RemoteData." + key).As<TData>();
        }

        [IncludeGenericArguments(false)]
        public static Lookup<TItem> GetLookup<TItem>(string key)
        {
            return ScriptData.Ensure("Lookup." + key).As<Lookup<TItem>>();
        }

        public static List<PropertyItem> GetForm(string key)
        {
            return ScriptData.Ensure("Form." + key).As<List<PropertyItem>>();
        }

        public static string GetTemplate(string key)
        {
            return ScriptData.Ensure("Template." + key).As<string>();
        }

        public static bool CanLoadScriptData(string name)
        {
            return ScriptData.CanLoad(name);
        }
    }
}