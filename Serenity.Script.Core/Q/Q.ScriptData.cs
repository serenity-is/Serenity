using jQueryApi;
using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Serenity
{
    public static partial class Q
    {
        public static class ScriptData
        {
            private static JsDictionary<string, string> registered = new JsDictionary<string, string>();
            private static JsDictionary<string, object> loadedData = new JsDictionary<string, object>();

            public static void BindToChange(string name, string regClass, Action onChange)
            {
                jQuery.FromObject(Document.Body).As<dynamic>()
                    .bind("scriptdatachange." + regClass, new Action<jQueryEvent, string>((e, s) =>
                {
                    if (s == name)
                        onChange();
                }));
            }

            public static void TriggerChange(string name)
            {
                J(Document.Body).TriggerHandler("scriptdatachange", new object[] { name });
            }

            public static void UnbindFromChange(string regClass)
            {
                jQuery.FromObject(Document.Body)
                    .Unbind("scriptdatachange." + regClass);
            }

            private static void SyncLoadScript(string url)
            {
                jQuery.Ajax(new jQueryAjaxOptions
                {
                    Async = false,
                    Cache = true,
                    Type = "GET",
                    Url = url,
                    Data = null,
                    DataType = "script"
                });
            }

            private static Promise LoadScriptAsync(string url)
            {
                return Promise.Void.ThenAwait(() => 
                {
                    Q.BlockUI();

                    return Promise.Resolve(jQuery.Ajax(new jQueryAjaxOptions
                    {
                        Async = true,
                        Cache = true,
                        Type = "GET",
                        Url = url,
                        Data = null,
                        DataType = "script"
                    }).Always(Q.BlockUndo));
                });
            }

            private static void LoadScriptData(string name)
            {
                if (!registered.ContainsKey(name))
                    throw new Exception(String.Format("Script data {0} is not found in registered script list!", name));

                name = name + ".js?" + registered[name];

                SyncLoadScript(Q.ResolveUrl("~/DynJS.axd/") + name);
            }

            private static Promise LoadScriptDataAsync(string name)
            {
                return Promise.Void.ThenAwait(() =>
                {
                    if (!registered.ContainsKey(name))
                        throw new Exception(String.Format("Script data {0} is not found in registered script list!", name));

                    name = name + ".js?" + registered[name];

                    return LoadScriptAsync(Q.ResolveUrl("~/DynJS.axd/") + name);
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

            [IncludeGenericArguments(false)]
            public static Promise<TData> EnsureAsync<TData>(string name)
            {
                return Promise.Void.ThenAwait(() =>
                {
                    var data = loadedData[name].As<TData>();

                    if (Script.IsValue(data))
                        return Promise.FromValue(data);

                    return LoadScriptDataAsync(name).ThenSelect(() =>
                    {
                        data = loadedData[name].As<TData>();

                        if (!Script.IsValue(data))
                            throw new NotSupportedException(String.Format("Can't load script data: {0}!", name));

                        return data;
                    });
                });
            }

            public static object Reload(string name)
            {
                if (!registered.ContainsKey(name))
                    throw new NotSupportedException(String.Format("Script data {0} is not found in registered script list!"));

                registered[name] = new JsDate().GetTime().ToString();

                LoadScriptData(name);

                var data = loadedData[name];

                return data;
            }

            [IncludeGenericArguments(false)]
            public static Promise<TData> ReloadAsync<TData>(string name)
            {
                return Promise.Void.ThenAwait(() =>
                {
                    if (!registered.ContainsKey(name))
                        throw new NotSupportedException(String.Format("Script data {0} is not found in registered script list!"));

                    registered[name] = new JsDate().GetTime().ToString();

                    return LoadScriptDataAsync(name)
                        .ThenSelect(() => loadedData[name].As<TData>());
                });
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
                TriggerChange(name);
            }
        }

        [IncludeGenericArguments(false)]
        public static TData GetRemoteData<TData>(string key)
        {
            return ScriptData.Ensure("RemoteData." + key).As<TData>();
        }

        [IncludeGenericArguments(false)]
        public static Promise<TData> GetRemoteDataAsync<TData>(string key)
        {
            return ScriptData.EnsureAsync<TData>("RemoteData." + key);
        }

        [IncludeGenericArguments(false)]
        public static Lookup<TItem> GetLookup<TItem>(string key)
        {
            return ScriptData.Ensure("Lookup." + key).As<Lookup<TItem>>();
        }

        [IncludeGenericArguments(false)]
        public static Promise<Lookup<TItem>> GetLookupAsync<TItem>(string key)
        {
            return ScriptData.EnsureAsync<Lookup<TItem>>("Lookup." + key);
        }

        public static void ReloadLookup(string key)
        {
            ScriptData.Reload("Lookup." + key);
        }

        public static Promise ReloadLookupAsync(string key)
        {
            return ScriptData.ReloadAsync<object>("Lookup." + key);
        }

        [IncludeGenericArguments(false)]
        public static List<PropertyItem> GetColumns(string key)
        {
            return ScriptData.Ensure("Columns." + key).As<List<PropertyItem>>();
        }

        public static Promise<List<PropertyItem>> GetColumnsAsync(string key)
        {
            return ScriptData.EnsureAsync<List<PropertyItem>>("Columns." + key);
        }

        [IncludeGenericArguments(false)]
        public static List<PropertyItem> GetForm(string key)
        {
            return ScriptData.Ensure("Form." + key).As<List<PropertyItem>>();
        }

        public static Promise<List<PropertyItem>> GetFormAsync(string key)
        {
            return ScriptData.EnsureAsync<List<PropertyItem>>("Form." + key);
        }

        public static string GetTemplate(string key)
        {
            return ScriptData.Ensure("Template." + key).As<string>();
        }

        public static Promise<string> GetTemplateAsync(string key)
        {
            return ScriptData.EnsureAsync<string>("Template." + key);
        }

        public static bool CanLoadScriptData(string name)
        {
            return ScriptData.CanLoad(name);
        }
    }
}