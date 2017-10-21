using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Serenity.Web
{
    public static partial class DynamicScriptManager
    {
        private static ConcurrentDictionary<string, Item> registeredScripts;
        private static event Action<string> scriptChanged;

        static DynamicScriptManager()
        {
            registeredScripts = new ConcurrentDictionary<string, Item>(StringComparer.OrdinalIgnoreCase);

            Register(new RegisteredScripts());
        }

        public static bool IsRegistered(string name)
        {
            return registeredScripts.ContainsKey(name);
        }

        public static void Changed(string name)
        {
            Item item;
            if (registeredScripts.TryGetValue(name, out item) &&
                item != null)
            {
                item.Generator.Changed();
            }
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
            registeredScripts[name] = item;
        }

        public static Dictionary<string, string> GetRegisteredScripts()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in registeredScripts)
            {
                var key = (string)s.Key;
                if (key != "RegisteredScripts")
                {
                    var value = s.Value as Item;
                    result[key] = value.Content.Hash;
                }
            }
            return result;
        }

        public static void Reset()
        {
            foreach (Item script in registeredScripts.Values)
                script.Generator.Changed();
        }

        public static string GetScriptText(string name, bool checkRights = false)
        {
            Item item;
            if (!registeredScripts.TryGetValue(name, out item)
                || item == null)
            {
                return null;
            }

            if (checkRights)
                item.Generator.CheckRights();

            var script = item.EnsureContentBytes();
            return script.ScriptText;
        }

        public static string GetScriptInclude(string name, string extension = ".js")
        {
            Item item;
            if (!registeredScripts.TryGetValue(name, out item)
                || item == null)
            {
                return name;
            }

            var script = item.EnsureContentBytes();

            return name + extension + "?v=" + (script.Hash ?? script.Time.Ticks.ToString());
        }

        internal static Script GetScript(string name)
        {
            Item item;
            if (!registeredScripts.TryGetValue(name, out item) ||
                item == null)
            {
                return null;
            }

            item.Generator.CheckRights();
            return item.EnsureContentBytes();
        }

        private static void RaiseScriptChanged(string name)
        {
            var handlers = scriptChanged;
            if (handlers != null)
                handlers(name);
        }

        public static event Action<string> ScriptChanged
        {
            add
            {
                lock (registeredScripts)
                    scriptChanged += value;
            }
            remove
            {
                lock (registeredScripts)
                    scriptChanged -= value;
            }
        }
    }
}