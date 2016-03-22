using Newtonsoft.Json;
using Serenity.Abstractions;
using Serenity.Localization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Serenity.Web
{
    public class LocalTextScript : DynamicScript, INamedDynamicScript
    {
        private string scriptName;
        private string package;
        private string languageId;
        private bool isPending;
        private static Dictionary<string, string[]> packages;

        public LocalTextScript(string package, string languageId, bool isPending)
        {
            this.package = package;
            this.languageId = languageId;
            this.isPending = isPending;
            this.scriptName = GetScriptName(package, languageId, isPending);
        }

        public string ScriptName { get { return scriptName; } }

        public static string GetScriptName(string package, string languageId, bool isPending)
        {
            return String.Format("LocalText.{0}.{1}.{2}", package, languageId, isPending ? "Pending" : "Public");
        }

        public static string GetLocalTextPackageScript(string package, string languageId, bool isPending)
        {
            if (packages == null)
            {
                packages = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(
                    ConfigurationManager.AppSettings["LocalTextPackages"].TrimToNull() ?? "{}", JsonSettings.Tolerant);
            }

            string[] packageItems;
            if (!packages.TryGetValue(package, out packageItems) ||
                packageItems.Length == 0)
                return String.Empty;

            StringBuilder sb = new StringBuilder("^(");
            bool append = false;
            foreach (object obj in packageItems)
            {
                if (append)
                    sb.Append('|');
                string item = Convert.ToString(obj);
                if (item.Length > 0)
                {
                    if (item[0] == '^' && item[item.Length - 1] == '$')
                        sb.Append(item.Substring(1, item.Length - 2));
                    else sb.Append(item.Replace(".", "\\.") + ".*");
                    append = true;
                }
            }
            sb.Append(")$");
            var regex = new Regex(sb.ToString(), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var texts = ((LocalTextRegistry)Dependency.Resolve<ILocalTextRegistry>())
                .GetAllAvailableTextsInLanguage(languageId, isPending);

            var list = new List<KeyValuePair<string, string>>();

            foreach (var pair in texts)
                if (regex.IsMatch(pair.Key))
                    list.Add(pair);

            list.Sort((i1, i2) => String.CompareOrdinal(i1.Key, i2.Key));

            StringBuilder jwBuilder = new StringBuilder("Q$LT.add(");
            JsonWriter jw = new JsonTextWriter(new StringWriter(jwBuilder));
                
            jw.WriteStartObject();
            List<string> stack = new List<string>();
            int stackCount = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var pair = list[i];
                // we can't handle keys that ends with '.' for now (possible clash between "site." and "site")
                if (pair.Key.Length == 0 || pair.Key[pair.Key.Length - 1] == '.')
                    continue;

                var parts = pair.Key.Split('.');

                if (parts.Length == 0)
                    continue;

                int same = 0;

                if (stackCount > 0)
                {
                    while (same < stackCount && same < parts.Length && stack[same] == parts[same])
                        same++;

                    for (int level = same; level < stackCount; level++)
                        jw.WriteEndObject();

                    stackCount = same;
                }

                for (int level = same; level < parts.Length - 1; level++)
                {
                    string part = parts[level];
                    if (stack.Count > level)
                        stack[level] = part;
                    else
                        stack.Add(part);
                    jw.WritePropertyName(part);
                    jw.WriteStartObject();
                }
                stackCount = parts.Length - 1;

                if (same != parts.Length)
                {
                    string part = parts[parts.Length - 1];

                    bool nextStartsWithThis = false;
                    if (i + 1 < list.Count)
                    {
                        var next = list[i + 1];
                        if (next.Key.StartsWith(pair.Key, StringComparison.OrdinalIgnoreCase) &&
                            next.Key.Length > pair.Key.Length &&
                            next.Key[pair.Key.Length] == '.')
                            nextStartsWithThis = true;
                    }

                    if (nextStartsWithThis)
                    {
                        stackCount = parts.Length;
                        if (stack.Count > stackCount - 1)
                            stack[stackCount - 1] = part;
                        else
                            stack.Add(part);
                        jw.WritePropertyName(part);
                        jw.WriteStartObject();
                        jw.WritePropertyName("");
                        jw.WriteValue(pair.Value);
                    }
                    else
                    {
                        jw.WritePropertyName(part);
                        jw.WriteValue(pair.Value);
                    }
                }
            }
            for (int i = 0; i < stackCount; i++)
                jw.WriteEndObject();
            jw.WriteEndObject();
            jwBuilder.Append(");");

            return jwBuilder.ToString();
        }

        public override string GetScript()
        {
            return GetLocalTextPackageScript(package, languageId, isPending);
        }
    }
}