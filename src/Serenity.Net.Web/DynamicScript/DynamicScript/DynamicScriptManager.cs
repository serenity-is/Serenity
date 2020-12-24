using Serenity.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Serenity.Web
{
    public partial class DynamicScriptManager : IDynamicScriptManager
    {
        private readonly ConcurrentDictionary<string, IDynamicScript> registeredScripts;
        private readonly ConcurrentDictionary<string, DateTime> scriptLastChange;
        private Action<string> scriptChanged;

        private readonly ITwoLevelCache cache;
        private readonly IPermissionService permissions;
        private readonly ITextLocalizer localizer;

        private static readonly UTF8Encoding utf8Encoding = new UTF8Encoding(true);

        public DynamicScriptManager(ITwoLevelCache cache, IPermissionService permissions, ITextLocalizer localizer)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
            this.localizer = localizer;
            registeredScripts = new ConcurrentDictionary<string, IDynamicScript>(StringComparer.OrdinalIgnoreCase);
            scriptLastChange = new ConcurrentDictionary<string, DateTime>();
            Register(new RegisteredScripts(this));
        }

        public bool IsRegistered(string name)
        {
            return registeredScripts.ContainsKey(name);
        }

        public void Changed(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            scriptLastChange[name] = DateTime.UtcNow;
            scriptChanged?.Invoke(name);
        }

        public void IfNotRegistered(string name, Func<IDynamicScript> callback)
        {
            registeredScripts.GetOrAdd(name, (name) => callback());
        }

        public void Register(INamedDynamicScript script)
        {
            Register(script.ScriptName, script);
        }

        public void Register(string name, IDynamicScript script)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            registeredScripts[name] = script ?? throw new ArgumentNullException(nameof(script));
        }

        public Dictionary<string, string> GetRegisteredScripts()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in registeredScripts)
            {
                var key = s.Key;
                if (key != "RegisteredScripts")
                {
                    result[key] = PeekScriptHash(key, s.Value);
                }
            }
            return result;
        }

        public IEnumerable<string> GetRegisteredScriptNames()
        {
            return registeredScripts.Keys;
        }

        public string PeekScriptHash(string name, IDynamicScript script)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var cacheKey = "DynamicScript:" + name;
            if (script is ICacheSuffix ics)
                cacheKey = cacheKey + ":" + ics.CacheSuffix;

            var groupKey = script.GroupKey;

            ScriptContent scriptContent;
            if (groupKey == null)
                scriptContent = cache.Memory.Get<ScriptContent>(cacheKey, TimeSpan.Zero, null);
            else
                scriptContent = cache.GetLocalStoreOnly<ScriptContent>(cacheKey, TimeSpan.Zero, groupKey, null);

            if (scriptContent != null &&
                scriptLastChange.TryGetValue(name, out DateTime lastChange) &&
                lastChange >= scriptContent.Time)
            {
                if (groupKey == null)
                    cache.Memory.Remove(cacheKey);
                else
                    cache.Remove(cacheKey);

                scriptContent = null;
            }

            var hash = scriptContent?.Hash;
            if (hash == null)
            {
                hash = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture);
                if (script is ICacheSuffix ics2)
                    hash = "-" + ics2.CacheSuffix.GetHashCode(StringComparison.Ordinal).ToString(CultureInfo.InvariantCulture);
            }

            return hash;
        }

        private IScriptContent EnsureScriptContent(string name, IDynamicScript script)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var cacheKey = "DynamicScript:" + name;
            if (script is ICacheSuffix ics)
                cacheKey = cacheKey + ":" + ics.CacheSuffix;

            ScriptContent factory() {
                var content = utf8Encoding.GetBytes(script.GetScript());
                return new ScriptContent(content, DateTime.UtcNow, content.Length > 4096);
            }
            
            var groupKey = script.GroupKey;

            ScriptContent getOrCreate()
            {
                if (groupKey == null)
                    return cache.Memory.Get(cacheKey, script.Expiration, factory);
                else
                    return cache.GetLocalStoreOnly(cacheKey, script.Expiration, groupKey, factory);
            };

            var scriptContent = getOrCreate();
            if (scriptLastChange.TryGetValue(name, out DateTime lastChange) &&
                lastChange >= scriptContent.Time)
            {
                if (groupKey == null)
                    cache.Memory.Remove(cacheKey);
                else
                    cache.Remove(cacheKey);

                return getOrCreate();
            }

            return scriptContent;
        }

        public void Reset()
        {
            foreach (var name in registeredScripts.Keys)
                scriptLastChange[name] = DateTime.UtcNow;
        }

        public void CheckScriptRights(string name)
        {
            if (registeredScripts.TryGetValue(name, out var script))
                script.CheckRights(permissions, localizer);
        }

        public string GetScriptText(string name)
        {
            if (!registeredScripts.TryGetValue(name, out var script))
                return null;

            var content = EnsureScriptContent(name, script).Content;
            return utf8Encoding.GetString(content);
        }

        public string GetScriptInclude(string name, string extension = ".js")
        {
            if (!registeredScripts.TryGetValue(name, out var script))
                return name;

            var hash = PeekScriptHash(name, script);

            return name + extension + "?v=" + hash;
        }

        public IScriptContent ReadScriptContent(string name)
        {
            if (!registeredScripts.TryGetValue(name, out var script))
                return null;

            script.CheckRights(permissions, localizer);

            return EnsureScriptContent(name, script);
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