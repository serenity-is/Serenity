using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Serenity.Web
{
    public interface IScriptName
    {
        string ScriptName { get; }
    }

    public interface IDynamicScript
    {
        string GetScript();
        void CheckRights();
        void Changed();
        bool NonCached { get; }
        event EventHandler ScriptChanged;
    }

    public interface ITwoLevelCached
    {
        string GlobalGenerationKey { get; }
        TimeSpan LocalExpiration { get; }
        TimeSpan RemoteExpiration { get; }
    }

    public interface INamedDynamicScript : IDynamicScript, IScriptName
    {
    }

    public abstract class DynamicScriptNamespace
    {
    }

    public static class DynamicScriptManager
    {
        private static Hashtable _registeredScripts;

        private class Item
        {
            public string Name;
            public IDynamicScript Generator;
            public string PackageName;
            public bool NonCached;
            public Script CurrentScript;

            public Item(string name, IDynamicScript generator)
            {
                Name = name;
                Generator = generator;
                CurrentScript = new Script
                {
                    Time = DateTime.UtcNow,
                    Hash = DateTime.Now.Ticks.ToString(),
                    UncompressedBytes = null,
                    CompressedBytes = null
                };
                PackageName = null;
                NonCached = false;
                generator.ScriptChanged += ScriptChanged;
            }

            private void ScriptChanged(object sender, EventArgs e)
            {
                UnsafeReset();
            }

            private static string GetMD5HashString(byte[] bytes)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                foreach (var c in result)
                    sb.Append(c.ToString("X2"));

                return sb.ToString();
            }

            private Script GenerateCurrentScript()
            {
                byte[] ub = null;
                byte[] cb = null;

                string script = Generator.GetScript();
                using (var ms = new MemoryStream(script.Length))
                {
                    using (var sw = new StreamWriter(ms, new UTF8Encoding(true)))
                    {
                        sw.Write(script);
                        sw.Flush();

                        ub = ms.ToArray();
                        ms.Seek(0, SeekOrigin.Begin);

                        if (ms.Length > 4096)
                        {
                            using (var cs = new MemoryStream((int)ms.Length))
                            {
                                using (var gz = new GZipStream(cs, CompressionMode.Compress))
                                {
                                    ms.CopyTo(gz);
                                    gz.Flush();
                                }

                                cb = cs.ToArray();
                            }
                        }
                    }

                    return new Script
                    {
                        Hash = GetMD5HashString(ub),
                        Time = DateTime.UtcNow,
                        CompressedBytes = cb,
                        UncompressedBytes = ub
                    };
                }
            }

            internal void UnsafeEnsure()
            {
                var twoLevel = Generator as ITwoLevelCached;
                if (twoLevel != null &&
                    twoLevel.GlobalGenerationKey != null)
                {
                    CurrentScript = TwoLevelCache.Get("DynamicScript_" + this.Name,
                        twoLevel.LocalExpiration, twoLevel.RemoteExpiration, twoLevel.GlobalGenerationKey, GenerateCurrentScript);
                }
                else if (CurrentScript == null || CurrentScript.UncompressedBytes == null || NonCached)
                        CurrentScript = GenerateCurrentScript();
            }

            private void UnsafeReset()
            {
                CurrentScript = new Script
                {
                    Time = DateTime.UtcNow,
                    Hash = DateTime.Now.Ticks.ToString(),
                    UncompressedBytes = null,
                    CompressedBytes = null
                };

                var twoLevel = Generator as ITwoLevelCached;
                if (twoLevel != null &&
                    twoLevel.GlobalGenerationKey != null)
                    TwoLevelCache.Remove("DynamicScript_" + this.Name);

                Item package;
                if (PackageName != null)
                {
                    package = _registeredScripts[PackageName] as Item;
                    if (package != null)
                        package.UnsafeReset();
                }
            }
        }

        static DynamicScriptManager()
        {
            _registeredScripts = new Hashtable(StringComparer.OrdinalIgnoreCase);

            Register(new RegisteredScripts());
        }

        public static bool IsRegistered(string name)
        {
            return _registeredScripts.ContainsKey(name);
        }

        public static void Changed(string name)
        {
            Item item = _registeredScripts[name] as Item;
            if (item != null)
                item.Generator.Changed();
        }

        public static void IfNotRegistered(string name, Action callback)
        {
            if (!_registeredScripts.ContainsKey(name))
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
            var reg = Hashtable.Synchronized(_registeredScripts);
            reg[name] = item;
        }

        public static Dictionary<string, string> GetRegisteredScripts()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (DictionaryEntry s in _registeredScripts)
            {
                var key = (string)s.Key;
                if (key != RegisteredScripts._scriptName)
                {
                    var value = s.Value as Item;
                    result[key] = value.NonCached ? DateTime.Now.Ticks.ToString() : value.CurrentScript.Hash;
                }
            }
            return result;
        }

        public class Script
        {
            public string Hash;
            public DateTime Time;
            public byte[] UncompressedBytes;
            public byte[] CompressedBytes;
            public string PackageName;
        }

        public static string GetScriptInclude(string name)
        {
            Item item = _registeredScripts[name] as Item;
            if (item == null)
                return name;

            item.UnsafeEnsure();

            return name + ".js?" + item.CurrentScript.Hash;
        }

        public static void Reset()
        {
            foreach (Item script in _registeredScripts.Values)
                script.Generator.Changed();
        }

        public static Script GetScript(string name)
        {
            Item item = _registeredScripts[name] as Item;
            if (item == null)
                return null;

            item.Generator.CheckRights();
            item.UnsafeEnsure();

            return item.CurrentScript;
        }
    }
}