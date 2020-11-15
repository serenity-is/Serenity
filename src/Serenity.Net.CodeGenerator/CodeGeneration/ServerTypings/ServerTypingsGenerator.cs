using Mono.Cecil;
using Serenity.Data;
using Serenity.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : CecilImportGenerator
    {
        public bool LocalTexts { get; set; }
        public readonly HashSet<string> LocalTextFilters = new HashSet<string>();

        public ServerTypingsGenerator(params Assembly[] assemblies)
            : base(assemblies)
        {
        }

        public ServerTypingsGenerator(params string[] assemblyLocations)
            : base(assemblyLocations)
        {
        }

        protected override bool IsTS()
        {
            return true;
        }

        protected override void GenerateAll()
        {
            base.GenerateAll();
            if (LocalTexts)
                GenerateTexts();
            GenerateSSDeclarations();
        }

        protected override void GenerateCodeFor(TypeDefinition type)
        {
            var codeNamespace = GetNamespace(type);

            Action<Action<TypeDefinition>> run = action =>
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
            else if (CecilUtils.IsSubclassOf(type, "Microsoft.AspNetCore.Mvc", "Controller") ||
                CecilUtils.IsSubclassOf(type, "System.Web.Mvc", "Controller"))
                run(GenerateService);
            else
            {
                var formScriptAttr = CecilUtils.GetAttr(type, "Serenity.ComponentModel", "FormScriptAttribute");
                if (formScriptAttr != null)
                {
                    run(t => GenerateForm(t, formScriptAttr));
                    EnqueueTypeMembers(type);

                    if (CecilUtils.IsSubclassOf(type, "Serenity.Services", "ServiceRequest"))
                    {
                        AddFile(RemoveRootNamespace(codeNamespace,
                            this.fileIdentifier + (IsTS() ? ".ts" : ".cs")));

                        this.fileIdentifier = type.Name;
                        run(GenerateBasicType);
                    }

                    return;
                }
                else if (CecilUtils.GetAttr(type, "Serenity.ComponentModel", "ColumnsScriptAttribute") != null)
                {
                    //GenerateColumns(type);
                    run(EnqueueTypeMembers);
                    return;
                }
                else if (CecilUtils.GetAttr(type, "Serenity.Extensibility", "NestedPermissionKeysAttribute") != null)
                {
                    run(GeneratePermissionKeys);
                }
                else
                    run(GenerateBasicType);
            }
        }

        protected void GenerateBasicType(TypeDefinition type)
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
                if (CecilUtils.IsSubclassOf(type, "Serenity.Data", "Row"))
                    GenerateRowMembers(type);
                else
                {
                    Action<TypeReference, string, IEnumerable<CustomAttribute>> handleMember = (memberType, memberName, a) =>
                    {
                        if (!CanHandleType(memberType.Resolve()))
                            return;

                        var jsonProperty = a != null ? CecilUtils.FindAttr(a, "Newtonsoft.Json", "JsonPropertyAttribute") : null;
                        if (jsonProperty != null &&
                            jsonProperty.HasConstructorArguments)
                        {
                            var arg = jsonProperty.ConstructorArguments.First();
                            if (arg.Type.FullName == "System.String" &&
                                !string.IsNullOrEmpty(arg.Value as string))
                            {
                                memberName = arg.Value as string;
                            }
                        }

                        cw.Indented(memberName);
                        sb.Append("?: ");
                        HandleMemberType(memberType, codeNamespace);
                        sb.Append(';');
                        sb.AppendLine();
                    };

                    var current = type;
                    do
                    {
                        foreach (var field in current.Fields)
                        {
                            if (field.IsStatic | !field.IsPublic)
                                continue;

                            if (CecilUtils.FindAttr(field.CustomAttributes, "Newtonsoft.Json", "JsonIgnoreAttribute") != null)
                                continue;

                            handleMember(field.FieldType, field.Name, field.CustomAttributes);
                        }

                        foreach (var property in current.Properties)
                        {
                            if (!CecilUtils.IsPublicInstanceProperty(property))
                                continue;

                            if (property.HasCustomAttributes &&
                                CecilUtils.FindAttr(property.CustomAttributes, "Newtonsoft.Json", "JsonIgnoreAttribute") != null)
                                continue;

                            handleMember(property.PropertyType, property.Name, property.CustomAttributes);
                            continue;
                        }
                    }
                    while ((current = current.BaseType?.Resolve()) != null && 
                        (baseClass == null ||!CecilUtils.IsAssignableFrom(current, baseClass)));
                }
            });

            if (CecilUtils.IsSubclassOf(type, "Serenity.Data", "Row"))
                GenerateRowMetadata(type);
        }
    }
}