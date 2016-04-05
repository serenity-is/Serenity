using Newtonsoft.Json;
using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Reflection;
using System.Web.Mvc;

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

            cw.Indented("namespace ");
            sb.Append(codeNamespace);

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
                    return;
                }

                if (type.GetCustomAttribute<ColumnsScriptAttribute>() != null)
                {
                    //GenerateColumns(type);
                    return;
                }

                cw.Indented("export interface ");

                var generatedName = MakeFriendlyName(type, codeNamespace);
                generatedTypes.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + generatedName);

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
                            sb.AppendLine();
                        }
                    }
                });

                if (type.IsSubclassOf(typeof(Row)))
                    GenerateRowMetadata(type);
            });
        }
    }
}