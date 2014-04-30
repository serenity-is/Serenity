using System;
using System.Collections;
using System.Collections.Generic;

namespace Serenity.Web
{
    public static partial class DynamicScriptManager
    {
        private static Hashtable registeredScripts;

        static DynamicScriptManager()
        {
            registeredScripts = new Hashtable(StringComparer.OrdinalIgnoreCase);

            Register(new RegisteredScripts());
        }

        public static bool IsRegistered(string name)
        {
            return registeredScripts.ContainsKey(name);
        }

        public static void Changed(string name)
        {
            Item item = registeredScripts[name] as Item;
            if (item != null)
                item.Generator.Changed();
        }

        public static void IfNotRegistered(string name, Action callback)
        {
            if (!registeredScripts.ContainsKey(name))
                callback();
        }

        public static void Register(INamedDynamicScript script)
        {
            Register(script.ScriptName, script);
        }

        public static void Register(string name, IDynamicScript script)
        {
            var item = new Item(name, script);
            item.NonCached = script.NonCached;
            var reg = Hashtable.Synchronized(registeredScripts);
            reg[name] = item;
        }

        public static Dictionary<string, string> GetRegisteredScripts()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (DictionaryEntry s in registeredScripts)
            {
                var key = (string)s.Key;
                if (key != RegisteredScripts._scriptName)
                {
                    var value = s.Value as Item;
                    result[key] = value.NonCached ? DateTime.Now.Ticks.ToString() : value.Content.Hash;
                }
            }
            return result;
        }

        public static void Reset()
        {
            foreach (Item script in registeredScripts.Values)
                script.Generator.Changed();
        }

        public static string GetScriptInclude(string name)
        {
            Item item = registeredScripts[name] as Item;
            if (item == null)
                return name;

            var script = item.EnsureContentBytes();

            return name + ".js?v=" + script.Hash;
        }

        internal static Script GetScript(string name)
        {
            Item item = registeredScripts[name] as Item;
            if (item == null)
                return null;

            item.Generator.CheckRights();
            return item.EnsureContentBytes();
        }
    }
}