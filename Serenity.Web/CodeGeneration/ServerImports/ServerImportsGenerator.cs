using Newtonsoft.Json;
using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Services;
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

            Action<Action<Type>> run = action =>
            {
                AppendUsings(UsingNamespaces);
                sb.AppendLine();

                cw.Indented("namespace ");
                sb.AppendLine(codeNamespace);
                cw.InBrace(delegate {
                    action(type);
                });
            };

            if (type.IsEnum)
                run(GenerateEnum);
            else if (type.IsSubclassOf(typeof(Controller)))
                run(GenerateService);
            else
            {
                var formScriptAttr = type.GetCustomAttribute<FormScriptAttribute>();
                if (formScriptAttr != null)
                {
                    run(t => GenerateForm(t, formScriptAttr));
                    EnqueueTypeMembers(type);

                    if (type.IsSubclassOf(typeof(ServiceRequest)))
                    {
                        AddFile(RemoveRootNamespace(codeNamespace,
                            this.fileIdentifier + (IsTS() ? ".ts" : ".cs")));

                        this.fileIdentifier = type.Name;
                        run(GenerateBasicType);
                    }

                    return;
                }
                else if (type.GetCustomAttribute<ColumnsScriptAttribute>() != null)
                {
                    //GenerateColumns(type);
                    run(EnqueueTypeMembers);
                    return;
                }
                else
                    run(GenerateBasicType);
            }
        }

        protected void GenerateBasicType(Type type)
        {
            var codeNamespace = GetNamespace(type);
            cw.IndentedLine("[Imported, Serializable, PreserveMemberCase]");
            cw.Indented("public partial class ");

            var identifier = MakeFriendlyName(type, codeNamespace);
            generatedTypes.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + identifier);

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
        }
    }
}