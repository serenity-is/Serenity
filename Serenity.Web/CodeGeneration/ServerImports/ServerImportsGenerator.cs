using Newtonsoft.Json;
using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Serenity.CodeGeneration
{
    public partial class ServerImportsGenerator : ServerImportGeneratorBase
    {
        public ServerImportsGenerator(params Assembly[] assemblies)
            : base(assemblies)
        {
        }

        private string[] UsingNamespaces = new string[]
        {
            "jQueryApi",
            "Serenity",
            "Serenity.ComponentModel",
            "System",
            "System.Collections",
            "System.Collections.Generic",
            "System.ComponentModel",
            "System.Runtime.CompilerServices"
        };

        protected override bool IsTS()
        {
            return false;
        }

        protected override bool IsUsingNamespace(string ns)
        {
            return UsingNamespaces.Contains(ns);
        }

        protected override void GenerateAll()
        {
            base.GenerateAll();
        }

        protected override void GenerateCodeFor(Type type)
        {
            var codeNamespace = GetNamespace(type);

            AppendUsings(UsingNamespaces);
            sb.AppendLine();

            cw.Indented("namespace ");
            sb.AppendLine(codeNamespace);

            cw.InBrace(delegate
            {
                if (type.IsEnum)
                {
                    GenerateEnum(type);
                    return;
                }

                if (type.IsSubclassOf(typeof(Controller)))
                {
                    GenerateService(type);
                    return;
                }

                var formScriptAttr = type.GetCustomAttribute<FormScriptAttribute>();
                if (formScriptAttr != null)
                {
                    GenerateForm(type, formScriptAttr);
                    EnqueueTypeMembers(type);
                    return;
                }

                if (type.GetCustomAttribute<ColumnsScriptAttribute>() != null)
                {
                    //GenerateColumns(type);
                    EnqueueTypeMembers(type);
                    return;
                }

                cw.IndentedLine("[Imported, Serializable, PreserveMemberCase]");
                cw.Indented("public partial class ");

                var generatedName = MakeFriendlyName(type, codeNamespace);
                generatedTypes.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + generatedName);

                var baseClass = GetBaseClass(type);
                if (baseClass != null)
                {
                    sb.Append(" : ");
                    MakeFriendlyReference(baseClass, GetNamespace(type));
                }

                sb.AppendLine();

                cw.InBrace(delegate
                {
                    if (type.IsSubclassOf(typeof(Row)))
                        GenerateRowMembers(type);
                    else
                    {
                        foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (member.GetCustomAttribute<JsonIgnoreAttribute>(false) != null)
                                continue;

                            if (baseClass != null && member.DeclaringType.IsAssignableFrom(baseClass))
                                continue;

                            var pi = member as PropertyInfo;
                            var fi = member as FieldInfo;
                            if (pi == null && fi == null)
                                continue;

                            var memberType = pi != null ? pi.PropertyType : fi.FieldType;

                            if (!CanHandleType(memberType))
                                continue;

                            var memberName = pi != null ? pi.Name : fi.Name;

                            var jsonProperty = member.GetCustomAttribute<JsonPropertyAttribute>(false);
                            if (jsonProperty != null && !jsonProperty.PropertyName.IsEmptyOrNull())
                            {
                                cw.Indented("[ScriptName(\"");
                                sb.Append(jsonProperty.PropertyName);
                                sb.AppendLine("\")]");
                            }

                            cw.Indented("public ");

                            HandleMemberType(memberType, codeNamespace);

                            sb.Append(" ");
                            sb.Append(memberName);
                            sb.AppendLine(" { get; set; }");
                        }
                    }
                });
            });
        }
    }
}