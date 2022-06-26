using Mono.Cecil;
using Serenity.Reflection;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : TypingsGeneratorBase
    {
        public bool LocalTexts { get; set; }
        public readonly HashSet<string> LocalTextFilters = new();

        public ServerTypingsGenerator(params Assembly[] assemblies)
            : base(assemblies)
        {
        }

        public ServerTypingsGenerator(params string[] assemblyLocations)
            : base(assemblyLocations)
        {
        }

        protected override void GenerateAll()
        {
            base.GenerateAll();
            if (LocalTexts)
                GenerateTexts();
        }

        protected override void GenerateCodeFor(TypeDefinition type)
        {
            var codeNamespace = GetNamespace(type);

            void run(Action<TypeDefinition> action)
            {
                cw.Indented("namespace ");
                sb.Append(codeNamespace);
                cw.InBrace(delegate
                {
                    action(type);
                });
            }

            if (type.IsEnum)
                run(GenerateEnum);
            else if (TypingsUtils.IsSubclassOf(type, "Microsoft.AspNetCore.Mvc", "Controller") ||
                TypingsUtils.IsSubclassOf(type, "System.Web.Mvc", "Controller"))
                run(GenerateService);
            else
            {
                var formScriptAttr = TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "FormScriptAttribute");
                if (formScriptAttr != null)
                {
                    run(t => GenerateForm(t, formScriptAttr));
                    EnqueueTypeMembers(type);

                    if (TypingsUtils.IsSubclassOf(type, "Serenity.Services", "ServiceRequest"))
                    {
                        AddFile(RemoveRootNamespace(codeNamespace,
                            fileIdentifier + ".ts"));

                        fileIdentifier = type.Name;
                        run(GenerateBasicType);
                    }

                    return;
                }

                var columnsScriptAttr = TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "ColumnsScriptAttribute");
                if (columnsScriptAttr != null)
                {
                    run(t => GenerateColumns(t, columnsScriptAttr));
                    EnqueueTypeMembers(type);
                    return;
                }

                if (TypingsUtils.GetAttr(type, "Serenity.Extensibility", "NestedPermissionKeysAttribute") != null ||
                    TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "NestedPermissionKeysAttribute") != null)
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
                if (TypingsUtils.IsSubclassOf(type, "Serenity.Data", "Row") ||
                    TypingsUtils.IsSubclassOf(type, "Serenity.Data", "Row`1"))
                    GenerateRowMembers(type);
                else
                {
                    void handleMember(TypeReference memberType, string memberName, IEnumerable<CustomAttribute> a)
                    {
                        if (!CanHandleType(memberType.Resolve()))
                            return;

                        var jsonProperty = a != null ? TypingsUtils.FindAttr(a, "Newtonsoft.Json", "JsonPropertyAttribute") : null;
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
                    }

                    var current = type;
                    do
                    {
                        foreach (var field in current.Fields)
                        {
                            if (field.IsStatic | !field.IsPublic)
                                continue;

                            if (TypingsUtils.FindAttr(field.CustomAttributes, "Newtonsoft.Json", "JsonIgnoreAttribute") != null)
                                continue;

                            handleMember(field.FieldType, field.Name, field.CustomAttributes);
                        }

                        foreach (var property in current.Properties)
                        {
                            if (!TypingsUtils.IsPublicInstanceProperty(property))
                                continue;

                            if (property.HasCustomAttributes &&
                                TypingsUtils.FindAttr(property.CustomAttributes, "Newtonsoft.Json", "JsonIgnoreAttribute") != null)
                                continue;

                            handleMember(property.PropertyType, property.Name, property.CustomAttributes);
                            continue;
                        }
                    }
                    while ((current = current.BaseType?.Resolve()) != null && 
                        (baseClass == null ||!TypingsUtils.IsAssignableFrom(current, baseClass)));
                }
            });

            if (TypingsUtils.IsSubclassOf(type, "Serenity.Data", "Row") ||
                TypingsUtils.IsSubclassOf(type, "Serenity.Data", "Row`1"))
                GenerateRowMetadata(type);
        }
    }
}