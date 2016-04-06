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
        [Imported, ScriptNamespace("Q"), ScriptName("ScriptData")]
        public static class ScriptData
        {
            [InlineCode("Q.ScriptData.bindToChange({name}, {regClass}, {onChange})")]
            public static void BindToChange(string name, string regClass, Action onChange)
            {
            }

            [InlineCode("Q.ScriptData.triggerChange({name})")]
            public static void TriggerChange(string name)
            {
            }

            [InlineCode("Q.ScriptData.unbindFromChange({regClass})")]
            public static void UnbindFromChange(string regClass)
            {
            }

            [InlineCode("Q.ScriptData.ensure({name})")]
            public static object Ensure(string name)
            {
                return null;
            }

            [IncludeGenericArguments(false)]
            [InlineCode("Q.ScriptData.ensureAsync({name})")]
            public static Promise<TData> EnsureAsync<TData>(string name)
            {
                return null;
            }

            [InlineCode("Q.ScriptData.reload({name})")]
            public static object Reload(string name)
            {
                return null;
            }

            [IncludeGenericArguments(false)]
            [InlineCode("Q.ScriptData.reloadAsync({name})")]
            public static Promise<TData> ReloadAsync<TData>(string name)
            {
                return null;
            }

            [InlineCode("Q.ScriptData.canLoad({name})")]
            public static bool CanLoad(string name)
            {
                return false;
            }

            [InlineCode("Q.ScriptData.setRegisteredScripts({scripts})")]
            public static void SetRegisteredScripts(JsDictionary<string, string> scripts)
            {
            }

            [InlineCode("Q.ScriptData.set({name}, {value})")]
            public static void Set(string name, object value)
            {
            }
        }

        [IncludeGenericArguments(false)]
        public static TData GetRemoteData<TData>(string key)
        {
            return default(TData);
        }

        [IncludeGenericArguments(false)]
        public static Promise<TData> GetRemoteDataAsync<TData>(string key)
        {
            return null;
        }

        [IncludeGenericArguments(false)]
        public static Lookup<TItem> GetLookup<TItem>(string key)
        {
            return null;
        }

        [IncludeGenericArguments(false)]
        public static Promise<Lookup<TItem>> GetLookupAsync<TItem>(string key)
        {
            return null;
        }

        public static void ReloadLookup(string key)
        {
        }

        public static Promise ReloadLookupAsync(string key)
        {
            return null;
        }

        [IncludeGenericArguments(false)]
        public static List<PropertyItem> GetColumns(string key)
        {
            return null;
        }

        public static Promise<List<PropertyItem>> GetColumnsAsync(string key)
        {
            return null;
        }

        [IncludeGenericArguments(false)]
        public static List<PropertyItem> GetForm(string key)
        {
            return null;
        }

        public static Promise<List<PropertyItem>> GetFormAsync(string key)
        {
            return null;
        }

        public static string GetTemplate(string key)
        {
            return null;
        }

        public static Promise<string> GetTemplateAsync(string key)
        {
            return null;
        }

        public static bool CanLoadScriptData(string name)
        {
            return false;
        }
    }
}