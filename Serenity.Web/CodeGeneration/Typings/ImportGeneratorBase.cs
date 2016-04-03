using System.Collections.Generic;
using System.Linq;

namespace Serenity.CodeGeneration
{
    public abstract class ImportGeneratorBase : CodeGeneratorBase
    {
        protected Dictionary<string, ExternalType> ssTypes;
        protected Dictionary<string, ExternalType> ssTypeMapping;
        protected Dictionary<string, ExternalType> tsTypes;

        public ImportGeneratorBase()
        {
            RootNamespaces = new HashSet<string>
            {
                "Serenity"
            };

            ssTypes = new Dictionary<string, ExternalType>();
            ssTypeMapping = new Dictionary<string, ExternalType>();
            tsTypes = new Dictionary<string, ExternalType>();
        }

        public HashSet<string> RootNamespaces { get; private set; }

        public void AddSSType(ExternalType type)
        {
            type.Origin = ExternalTypeOrigin.SS;

            var oldFullName = type.FullName;
            var ignoreNS = type.Attributes.FirstOrDefault(x =>
                x.Type == "System.Runtime.CompilerServices.IgnoreNamespaceAttribute");

            if (ignoreNS != null)
                type.Namespace = "";

            var scriptNS = type.Attributes.FirstOrDefault(x =>
                x.Type == "System.Runtime.CompilerServices.ScriptNamespaceAttribute");

            if (scriptNS != null)
                type.Namespace = scriptNS.Arguments[0].Value as string;

            var scriptName = type.Attributes.FirstOrDefault(x =>
                x.Type == "System.Runtime.CompilerServices.ScriptNameAttribute");

            if (scriptName != null)
                type.Name = scriptName.Arguments[0].Value as string;

            ExternalType overriding;
            if (ssTypes.TryGetValue(type.FullName, out overriding) &&
                type.GenericParameters.Count <= overriding.GenericParameters.Count)
            {
                if (oldFullName != type.FullName)
                    ssTypeMapping[oldFullName] = overriding;

                return;
            }

            if (oldFullName != type.FullName)
                ssTypeMapping[oldFullName] = type;

            ssTypes[type.FullName] = type;
        }

        public void AddTSType(ExternalType type)
        {
            type.Origin = ExternalTypeOrigin.TS;
            tsTypes[type.FullName] = type;
        }

        protected ExternalType GetScriptType(string fullName)
        {
            ExternalType type;
            if (tsTypes.TryGetValue(fullName, out type))
                return type;

            if (ssTypes.TryGetValue(fullName, out type))
                return type;

            if (ssTypeMapping.TryGetValue(fullName, out type))
                return type;

            return null;
        }

        protected bool IsGenericTypeName(string typeName)
        {
            return typeName.IndexOf("`") >= 0;
        }

        protected string[] SplitGenericArguments(ref string typeName)
        {
            if (!IsGenericTypeName(typeName))
                return new string[0];

            var pos = typeName.IndexOf("<");
            var last = typeName.LastIndexOf(">");
            if (pos >= 0 && last > pos)
            {
                char[] c = typeName.Substring(pos + 1, last - pos - 1).ToCharArray();
                typeName = typeName.Substring(0, pos);

                int nestingLevel = 0;
                for (int i = 0; i < c.Length; i++)
                {
                    if (c[i] == '<')
                        nestingLevel++;
                    else if (c[i] == '>')
                        nestingLevel--;
                    else if ((c[i] == ',') && (nestingLevel == 0))
                        c[i] = '€';
                }

                return new string(c).Split(new char[] { '€' });
            }
            else
                return new string[0];
        }

        protected string RemoveRootNamespace(string ns, string name)
        {
            if (!string.IsNullOrEmpty(ns))
                name = ns + "." + name;

            foreach (var rn in RootNamespaces)
            {
                if (name.StartsWith(rn + "."))
                    return name.Substring(rn.Length + 1);
            }

            return name;
        }

        protected string GetPropertyScriptName(ExternalProperty prop, bool preserveMemberCase)
        {
            var scriptNameAttr = prop.Attributes.FirstOrDefault(x =>
                x.Type == "System.Runtime.CompilerServices.ScriptNameAttribute");

            if (scriptNameAttr != null)
                return scriptNameAttr.Arguments[0].Value as string;
            else if (!preserveMemberCase && !prop.Attributes.Any(x =>
                    x.Type == "System.Runtime.CompilerServices.PreserveCaseAttribute"))
            {
                var propField = prop.Name;
                if (propField == "ID")
                    return "id";
                else
                    return propField.Substring(0, 1).ToLowerInvariant()
                        + propField.Substring(1);
            }
            else
                return prop.Name;
        }

        protected string GetBaseTypeName(ExternalType type)
        {
            if (type.BaseType == null)
                return null;

            var baseType = type.BaseType;
            SplitGenericArguments(ref baseType);
            return baseType;
        }

        protected ExternalType GetBaseType(ExternalType type)
        {
            return GetScriptType(GetBaseTypeName(type));
        }

        protected void AppendUsings(IEnumerable<string> namespaces)
        {
            foreach (var ns in namespaces)
            {
                cw.Indented("using ");
                sb.Append(ns);
                sb.AppendLine(";");
            }
        }

        protected bool HasBaseType(ExternalType type, string typeName)
        {
            int loop = 0;

            while (type != null)
            {
                if (loop++ > 100)
                    break;

                var baseTypeName = GetBaseTypeName(type);
                if (baseTypeName == typeName)
                    return true;

                type = GetScriptType(baseTypeName);
            };

            return false;
        }

        protected ExternalAttribute GetAttribute(ExternalType type, string attributeName, bool inherited)
        {
            var attr = type.Attributes.FirstOrDefault(x => x.Type == attributeName);
            if (attr != null)
                return attr;

            if (!inherited)
                return null;

            int loop = 0;
            while ((type = GetBaseType(type)) != null)
            {
                attr = type.Attributes.FirstOrDefault(x => x.Type == attributeName);
                if (attr != null)
                    return attr;

                if (loop++ > 100)
                    return null;
            };

            return null;
        }
    }
}