using Serenity.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Serenity.Web
{
    public partial class DynamicScriptManager
    {
        private ConcurrentDictionary<string, Item> registeredScripts;
        private Action<string> scriptChanged;

        private readonly ITwoLevelCache cache;

        public DynamicScriptManager(ITwoLevelCache cache)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            
            registeredScripts = new ConcurrentDictionary<string, Item>(StringComparer.OrdinalIgnoreCase);
            Register(new RegisteredScripts());
        }

        public bool IsRegistered(string name)
        {
            return registeredScripts.ContainsKey(name);
        }

        public void Changed(string name)
        {
            if (registeredScripts.TryGetValue(name, out Item item) &&
                item != null)
            {
                item.Generator.Changed();
            }
        }

        public void IfNotRegistered(string name, Action callback)
        {
            if (!registeredScripts.ContainsKey(name))
                callback();
        }

        public void Register(INamedDynamicScript script)
        {
            Register(script.ScriptName, script);
        }

        public void Register(string name, IDynamicScript script)
        {
            var item = new Item(name, script);
            registeredScripts[name] = item;
        }

        public Dictionary<string, string> GetRegisteredScripts()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in registeredScripts)
            {
                var key = s.Key;
                if (key != "RegisteredScripts")
                {
                    var value = s.Value;
                    result[key] = value.Content.Hash;
                }
            }
            return result;
        }

        public void Reset()
        {
            foreach (Item script in registeredScripts.Values)
                script.Generator.Changed();
        }

        public void CheckScriptRights(string name)
        {
            Item item;
            if (registeredScripts.TryGetValue(name, out item) && item != null)
                item.Generator.CheckRights();
        }

        public string GetScriptText(string name)
        {
            Item item;
            if (!registeredScripts.TryGetValue(name, out item)
                || item == null)
            {
                return null;
            }

            var script = item.EnsureContentBytes();
            return script.ScriptText;
        }

        public string GetScriptInclude(string name, string extension = ".js")
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

        internal Script GetScript(string name)
        {
            if (!registeredScripts.TryGetValue(name, out Item item) ||
                item == null)
            {
                return null;
            }

            item.Generator.CheckRights();
            return item.EnsureContentBytes();
        }

        private void RaiseScriptChanged(string name)
        {
            scriptChanged?.Invoke(name);
        }

        public event Action<string> ScriptChanged
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