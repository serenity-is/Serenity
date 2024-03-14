namespace Serenity.Web;

public abstract partial class PropertyItemsScript
{
    private static Func<object, object> CreatePropertyGetter(PropertyInfo propertyInfo)
    {
        MethodInfo getMethodInfo = propertyInfo.GetMethod;
        return delegate (object obj)
        {
            return getMethodInfo.Invoke(obj, null)!;
        };
    }

    private static string[] PropertyNames = null;
    private static Func<object, object>[] PropertyGetters = null;
    private static readonly char[] separators = ['.', '/', '_', ':'];

    private static readonly HashSet<string> JsReserved = [
        "abstract",
        "arguments",
        "as",
        "async",
        "await",
        "boolean",
        "break",
        "case",
        "catch",
        "class",
        "const",
        "continue",
        "debugger",
        "default",
        "delete",
        "do",
        "double",
        "else",
        "enum",
        "eval",
        "export",
        "extends",
        "false",
        "final",
        "finally",
        "float",
        "for",
        "function",
        "get",
        "goto",
        "if",
        "implements",
        "import",
        "in",
        "instanceof",
        "int",
        "interface",
        "let",
        "long",
        "native",
        "new",
        "null",
        "of",
        "package",
        "private",
        "protected",
        "public",
        "return",
        "set",
        "short",
        "static",
        "super",
        "switch",
        "synchronized",
        "this",
        "throw",
        "throws",
        "transient",
        "true",
        "try",
        "typeof",
        "var",
        "void",
        "volatile",
        "while",
        "with",
        "yield"
    ];

    internal static string Compact(IEnumerable<(string scriptName, PropertyItemsData data)> inputs)
    {
        ArgumentNullException.ThrowIfNull(inputs);
        Dictionary<string, string> strMap = [];
        int strMapCount = 0;

        var propertyNames = PropertyNames;
        var propertyGetters = PropertyGetters;
        if (propertyNames is null || propertyGetters is null)
        {
            var propertyInfos = typeof(PropertyItem).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.Name != nameof(PropertyItem.ExtensionData)).ToArray();
            propertyNames = propertyInfos.Select(x => x.GetAttribute<JsonPropertyNameAttribute>()?.Name ?? x.Name).ToArray();
            propertyGetters = propertyInfos.Select(CreatePropertyGetter).ToArray();
            PropertyNames = propertyNames;
            PropertyGetters = propertyGetters;
        }

        var sb = new StringBuilder();
        sb.Append('[');

        static char map64(int c)
        {
            if (c <= 25)
                return (char)('A' + c);
            if (c <= 51)
                return (char)('a' + (c - 26));
            if (c == 52)
                return '_';
            if (c == 53)
                return '$';
            return (char)('0' + (c - 54));
        }

        static string keyFor(int x)
        {
            string s = map64(x % 52).ToString();
            if (x >= 52)
            {
                x /= 52;
                do
                {
                    s += map64((x - 1) % 64);
                }
                while ((x = (x - 1) / 64) > 0);
            }
            return s;
        }

        string mapStr(string s)
        {
            if (strMap.TryGetValue(s, out string v))
                return v;
            string key;
            do
            {
                key = keyFor(strMapCount++);
            }
            while (JsReserved.Contains(key));
            return strMap[s] = key;
        }

        string mapStrValue(string s)
        {
            if (s == null)
                return "null";
            if (s.Length < 4)
                return StringHelper.ToSingleQuoted(s);
            return mapStr(s);
        }

        string mapPropKey(string s)
        {
            if (s == null)
                return "null";
            if (s.Length == 0)
                return "";
            if (s.Length < 4)
            {
                if ((s[0] == '_' || char.IsLetter(s[0])) &&
                    s.All(x => char.IsLetter(x) || x == '_' || x == '$' || char.IsAsciiDigit(x)))
                    return s;
                return StringHelper.ToSingleQuoted(s);
            }
            return "[" + mapStr(s) + "]";
        }

        void writePropertyName(string name)
        {
            sb.Append(mapPropKey(name));
            sb.Append(':');
        }

        void writeValue(object value)
        {
            if (value is bool b)
            {
                if (b)
                    sb.Append("!0");
                else
                    sb.Append("!1");
            }
            else if (value is string str)
            {
                bool first = true;
                int start = 0;
                do
                {
                    var idx = start + 4 >= str.Length ? -1 : str.IndexOfAny(separators, start + 4);
                    if (first)
                        first = false;
                    else
                        sb.Append('+');
                    if (idx >= 0 && idx < str.Length - 1)
                    {
                        sb.Append(mapStrValue(str[start..(idx + 1)]));
                        start = idx + 1;
                    }
                    else
                    {
                        sb.Append(mapStrValue(str[start..]));
                        break;
                    }
                }
                while (true);
            }
            else if (value is int intVal)
            {
                sb.Append(Invariants.ToInvariant(intVal));
            }
            else if (value is double dbl)
            {
                sb.Append(Invariants.ToInvariant(dbl));
            }
            else if (value is decimal dcm)
            {
                sb.Append(Invariants.ToInvariant(dcm));
            }
            else if (value is IDictionary<string, object> dict)
            {
                sb.Append('{');
                var first = true;
                foreach (var pair in dict)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(',');
                    writePropertyName(pair.Key);
                    writeValue(pair.Value);
                }
                sb.Append('}');
            }
            else if (value is System.Collections.IEnumerable en &&
                !value.GetType().GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
            {
                sb.Append('[');
                bool first = true;
                foreach (var x in en)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(',');
                    writeValue(x);
                }
                sb.Append(']');
            }
            else
            {
                sb.Append(JSON.Stringify(value, writeNulls: true));
            }
        }

        void serializeItem(PropertyItem item)
        {
            if (item == null)
            {
                sb.Append("{}");
                return;
            }

            sb.Append('{');
            var firstProp = true;
            for (var i = 0; i < propertyNames.Length; i++)
            {
                var value = propertyGetters[i](item);
                if (value == null)
                    continue;

                var propertyName = propertyNames[i];
                if (value is "String" && (propertyName == "editorType" || propertyName == "filteringType"))
                    continue;

                if (value is IDictionary<string, object> { Count: 0 })
                    continue;

                if (firstProp)
                    firstProp = false;
                else
                    sb.Append(',');

                writePropertyName(propertyName);
                writeValue(value);
            }

            if (item.ExtensionData != null && item.ExtensionData.Count > 0)
            {
                foreach (var pair in item.ExtensionData)
                {
                    if (firstProp)
                        firstProp = false;
                    else
                        sb.Append(',');
                    writePropertyName(pair.Key);
                    writeValue(pair.Value);
                }
            }

            sb.Append('}');
        }

        var firstInput = true;
        foreach (var input in inputs)
        {
            if (firstInput)
            {
                firstInput = false;
                sb.Append('[');
            }
            else
                sb.Append(",[");

            writeValue(input.scriptName);
            var items = input.data?.Items ?? [];
            sb.Append(",[");
            var firstItem = true;
            foreach (var item in items)
            {
                if (firstItem)
                    firstItem = false;
                else
                    sb.Append("\n,");
                serializeItem(item);
            }
            sb.Append(']');

            var addi = input.data?.AdditionalItems;
            if (addi?.Count > 0)
            {
                sb.Append("\n,[");
                firstItem = true;
                foreach (var item in addi)
                {
                    if (firstItem)
                        firstItem = false;
                    else
                        sb.Append("\n,");
                    serializeItem(item);
                }
                sb.Append(']');
            }
            sb.Append(']');
        }
        sb.Append("].forEach(d=>");
        sb.AppendFormat(DataScript.SetScriptDataFormat.TrimEnd(';'), "d[0]", "{items:d[1],additionalItems:(d[2]||[])}");
        sb.Append(")})();");
        return "(function(){var " + string.Join(",", strMap.OrderBy(x => x.Value.Length).ThenBy(x => x.Value, StringComparer.Ordinal)
                .Select((x, i) => $"{((i % 30) == 1 ? "\n" : "")}{x.Value}={StringHelper.ToSingleQuoted(x.Key)}")) + ";\n" +
            sb.ToString();
    }
}