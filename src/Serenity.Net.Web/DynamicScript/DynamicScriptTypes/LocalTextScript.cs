using Serenity.Localization;
using System.IO;

namespace Serenity.Web
{
    public class LocalTextScript : DynamicScript, INamedDynamicScript
    {
        private readonly string scriptName;
        private readonly string languageId;
        private readonly string includes;
        private readonly bool isPending;
        private readonly ILocalTextRegistry registry;

        public LocalTextScript(ILocalTextRegistry registry, string package, string includes, string languageId, bool isPending)
        {
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.includes = includes;
            this.languageId = languageId;
            this.isPending = isPending;
            scriptName = GetScriptName(package ?? throw new ArgumentNullException(nameof(package)), languageId, isPending);
        }

        public string ScriptName => scriptName;

        public static string GetScriptName(string package, string languageId, bool isPending)
        {
            return string.Format(CultureInfo.InvariantCulture, "LocalText.{0}.{1}.{2}", package, languageId, isPending ? "Pending" : "Public");
        }

        public static string GetLocalTextPackageScript(ILocalTextRegistry registry, 
            LocalTextPackages packages, string package, string languageId, bool isPending)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            if (packages == null)
                throw new ArgumentNullException(nameof(packages));

            if (!packages.TryGetValue(package, out string includes))
                includes = null;

            return GetLocalTextPackageScript(registry, includes, languageId, isPending);
        }

        public static string GetLocalTextPackageScript(ILocalTextRegistry registry, string includes, string languageId, bool isPending)
        {
            if (registry == null)
                throw new ArgumentNullException(nameof(registry));

            var list = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrEmpty(includes))
            {
                var regex = new Regex(includes, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var texts = registry is LocalTextRegistry ltr ? ltr.GetAllAvailableTextsInLanguage(languageId, isPending) : new Dictionary<string, string>();

                foreach (var pair in texts)
                    if (regex.IsMatch(pair.Key))
                        list.Add(pair);
            }
            list.Sort((i1, i2) => string.CompareOrdinal(i1.Key, i2.Key));

            StringBuilder jwBuilder = new("Q.LT.add(");
            JsonWriter jw = new JsonTextWriter(new StringWriter(jwBuilder));
                
            jw.WriteStartObject();
            List<string> stack = new();
            int stackCount = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var pair = list[i];
                // we can't handle keys that ends with '.' for now (possible clash between "site." and "site")
                if (pair.Key.Length == 0 || pair.Key[^1] == '.')
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
                    string part = parts[^1];

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
            return GetLocalTextPackageScript(registry, includes, languageId, isPending);
        }  
    }
}