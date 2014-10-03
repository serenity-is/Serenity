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

            [Obsolete("Prefer asynchronous version")]
            private static void LoadScriptData(string name)
            {
                if (!registered.ContainsKey(name))
                    throw new Exception(String.Format("Script data {0} is not found in registered script list!", name));

                name = name + ".js?" + registered[name];

                #pragma warning disable 618
                SyncLoadScript(Q.ResolveUrl("~/DynJS.axd/") + name);
                #pragma warning restore 618
            }

            private static async Task LoadScriptDataAsync(string name)
            {
                if (!registered.ContainsKey(name))
                    throw new Exception(String.Format("Script data {0} is not found in registered script list!", name));

                name = name + ".js?" + registered[name];

                await LoadScriptAsync(Q.ResolveUrl("~/DynJS.axd/") + name);
            }

            [Obsolete("Prefer asynchronous version")]
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

            private static async Task LoadScriptAsync(string url)
            {
                Q.BlockUI();

                await Task.FromPromise(jQuery.Ajax(new jQueryAjaxOptions
                {
                    Async = true,
                    Cache = true,
                    Type = "GET",
                    Url = url,
                    Data = null,
                    DataType = "script"
                }).Always(delegate() {
                    Q.BlockUndo();
                }));
            }

            [Obsolete("Prefer asynchronous version")]
            public static object Ensure(string name)
            {
                var data = loadedData[name];

                #pragma warning disable 618
                if (!Script.IsValue(data))
                    LoadScriptData(name);
                #pragma warning restore 618

                data = loadedData[name];

                if (!Script.IsValue(data))
                    throw new NotSupportedException(String.Format("Can't load script data: {0}!", name));

                return data;
            }

            public static async Task<object> EnsureAsync(string name)
            {
                var data = loadedData[name];

                if (!Script.IsValue(data))
                {
                    await LoadScriptDataAsync(name);
                    data = loadedData[name];

                    if (!Script.IsValue(data))
                        throw new NotSupportedException(String.Format("Can't load script data: {0}!", name));
                }

                return data;
            }

            [Obsolete("Prefer asynchronous version")]
            public static object Reload(string name)
            {
                if (!registered.ContainsKey(name))
                    throw new NotSupportedException(String.Format("Script data {0} is not found in registered script list!"));

                registered[name] = new JsDate().GetTime().ToString();

                LoadScriptData(name);

                var data = loadedData[name];

                return data;
            }

            public static async Task<object> ReloadAsync(string name)
            {
                if (!registered.ContainsKey(name))
                    throw new NotSupportedException(String.Format("Script data {0} is not found in registered script list!"));

                registered[name] = new JsDate().GetTime().ToString();

                await LoadScriptDataAsync(name);
                
                return loadedData[name];
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
        [Obsolete("Prefer asynchronous version")]
        public static TData GetRemoteData<TData>(string key)
        {
            #pragma warning disable 618
            return ScriptData.Ensure("RemoteData." + key).As<TData>();
            #pragma warning restore 618
        }

        [IncludeGenericArguments(false)]
        public static async Task<TData> GetRemoteDataAsync<TData>(string key)
        {
            return (await ScriptData.EnsureAsync("RemoteData." + key)).As<TData>();
        }

        [IncludeGenericArguments(false)]
        [Obsolete("Prefer asynchronous version")]
        public static Lookup<TItem> GetLookup<TItem>(string key)
        {
            #pragma warning disable 618
            return ScriptData.Ensure("Lookup." + key).As<Lookup<TItem>>();
            #pragma warning restore 618
        }

        [IncludeGenericArguments(false)]
        public static async Task<Lookup<TItem>> GetLookupAsync<TItem>(string key)
        {
            return (await ScriptData.EnsureAsync("Lookup." + key)).As<Lookup<TItem>>();
        }

        [Obsolete("Prefer asynchronous version")]
        public static void ReloadLookup(string key)
        {
            #pragma warning disable 618
            ScriptData.Reload("Lookup." + key);
            #pragma warning restore 618
        }

        public static async Task<object> ReloadLookupAsync(string key)
        {
            return await ScriptData.ReloadAsync("Lookup." + key);
        }

        [IncludeGenericArguments(false)]
        [Obsolete("Prefer asynchronous version")]
        public static List<PropertyItem> GetForm(string key)
        {
            #pragma warning disable 618
            return ScriptData.Ensure("Form." + key).As<List<PropertyItem>>();
            #pragma warning restore 618
        }

        [IncludeGenericArguments(false)]
        public static async Task<List<PropertyItem>> GetFormAsync(string key)
        {
            return (await ScriptData.EnsureAsync("Form." + key)).As<List<PropertyItem>>();
        }

        [Obsolete("Prefer asynchronous version")]
        public static string GetTemplate(string key)
        {
            #pragma warning disable 618
            return ScriptData.Ensure("Template." + key).As<string>();
            #pragma warning restore 618
        }

        public static async Task<string> GetTemplateAsync(string key)
        {
            return (await ScriptData.EnsureAsync("Template." + key)).As<string>();
        }

        public static bool CanLoadScriptData(string name)
        {
            return ScriptData.CanLoad(name);
        }
    }
}