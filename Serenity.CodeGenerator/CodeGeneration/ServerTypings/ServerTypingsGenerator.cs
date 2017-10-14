using Newtonsoft.Json;
using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Reflection;
using Serenity.Services;
using System;
using System.Reflection;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : ServerImportGeneratorBase
    {
        public ServerTypingsGenerator(params Assembly[] assemblies)
            : base(assemblies)
        {
        }

        protected override bool IsTS()
        {
            return true;
        }

        protected override void GenerateAll()
        {
            base.GenerateAll();
            GenerateSSDeclarations();
        }

        protected override void GenerateCodeFor(Type type)
        {
            var codeNamespace = GetNamespace(type);

            Action<Action<Type>> run = action =>
            {
                cw.Indented("namespace ");
                sb.Append(codeNamespace);
                cw.InBrace(delegate
                {
                    action(type);
                });
            };

            if (type.IsEnum)
                run(GenerateEnum);
            else if (GeneratorUtils.IsSubclassOf(type, "Microsoft.AspNetCore.Mvc.Controller") ||
                GeneratorUtils.IsSubclassOf(type, "System.Web.Mvc.Controller"))
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

            cw.Indented("export interface ");

            var identifier = MakeFriendlyName(type, codeNamespace);
            generatedTypes.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + identifier);

            var baseClass = GetBaseClass(type);
            if (baseClass != null)
            {
                sb.Append(" extends ");
                MakeFriendlyReference(baseClass, GetNamespace(type));
            }

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
                            memberName = jsonProperty.PropertyName;

                        cw.Indented(memberName);
                        sb.Append("?: ");
                        HandleMemberType(memberType, codeNamespace);
                        sb.Append(';');
                        sb.AppendLine();
                    }
                }
            });

            if (type.IsSubclassOf(typeof(Row)))
                GenerateRowMetadata(type);
        }
    }
}