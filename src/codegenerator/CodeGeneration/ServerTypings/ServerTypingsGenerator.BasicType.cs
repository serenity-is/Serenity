namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    protected void GenerateBasicType(TypeDefinition type)
    {
        cw.Indented("export interface ");

        var identifier = MakeFriendlyName(type, codeNamespace: null);
        var baseClass = GetBasicTypeBaseClass(type);

        RegisterGeneratedType(ns: null, identifier, typeOnly: true);

        if (baseClass != null)
        {
            sb.Append(" extends ");
            MakeFriendlyReference(baseClass, codeNamespace: null);
        }

        cw.InBrace(delegate
        {
            GenerateBasicTypeMembers(type, baseClass, codeNamespace: null);
        });
    }

    void GenerateBasicTypeMembers(TypeDefinition type, TypeReference? baseClass, 
        string? codeNamespace)
    {
        void handleMember(TypeReference memberType, string memberName, IEnumerable<CustomAttribute> a)
        {
            var jsonProperty = a != null ? (
                TypingsUtils.FindAttr(a, "Newtonsoft.Json", "JsonPropertyAttribute") ??
                TypingsUtils.FindAttr(a, "System.Text.Json.Serialization", "JsonPropertyNameAttribute")) : null;
            if (jsonProperty != null &&
                jsonProperty.HasConstructorArguments())
            {
                var arg = jsonProperty.ConstructorArguments.First();
                if (arg.Type?.FullNameOf() == "System.String" &&
                    !string.IsNullOrEmpty(arg.Value as string))
                {
                    memberName = (arg.Value as string)!;
                }
            }

            cw.Indented(memberName);
            sb.Append("?: ");
            AppendMappedType(memberType, codeNamespace);
            sb.Append(';');
            sb.AppendLine();

            // a member typed string that names its set with [EnumEditor(typeof(X))] still depends on it
            EnqueueStringEnumAttributes(a);
        }

        var current = type;
        do
        {
            foreach (var field in current.FieldsOf())
            {
                if (field.IsStatic | !field.IsPublic())
                    continue;

                if (TypingsUtils.FindAttr(field.GetAttributes(), "Newtonsoft.Json", "JsonIgnoreAttribute") != null ||
                    TypingsUtils.FindAttr(field.GetAttributes(), "System.Text.Json.Serialization", "JsonIgnoreAttribute") != null)
                    continue;

                handleMember(field.FieldType(), field.Name, field.GetAttributes());
            }

            foreach (var property in current.PropertiesOf())
            {
                if (!TypingsUtils.IsPublicInstanceProperty(property))
                    continue;

                if (property.HasCustomAttributes() &&
                    (TypingsUtils.FindAttr(property.GetAttributes(), "Newtonsoft.Json", "JsonIgnoreAttribute") != null ||
                     TypingsUtils.FindAttr(property.GetAttributes(), "System.Text.Json.Serialization", "JsonIgnoreAttribute") != null))
                    continue;

                handleMember(property.PropertyType(), property.Name, property.GetAttributes());
                continue;
            }
        }
        while ((current = current.BaseType?.Resolve()) != null &&
            (baseClass == null || !TypingsUtils.IsAssignableFrom(current, baseClass)));
    }

    /// <summary>
    /// Gets the base class a basic type extends, whose members it doesn't repeat: a request or a response type of
    /// Serenity.Services, or else the nearest base class of a Serenity.Net assembly that corelib has a type of the same
    /// name for, e.g. PropertyItem. Other base classes, like the ones of the project, have their members listed in the
    /// type, as they have no script type of their own to extend.
    /// </summary>
    /// <param name="type">The basic type.</param>
    protected TypeReference? GetBasicTypeBaseClass(TypeDefinition type)
    {
        return GetServiceBaseClass(type) ?? GetCorelibBaseClass(type);
    }

    /// <summary>
    /// Gets the nearest base class of a type that is of a Serenity.Net assembly and has a type of the same name in corelib,
    /// which is not generic, as its script type can't be told to have the same type parameters, nor an enum.
    /// </summary>
    /// <param name="type">The type.</param>
    protected TypeReference? GetCorelibBaseClass(TypeDefinition type)
    {
        foreach (var t in type.SelfAndBaseClasses().Skip(1))
        {
            if (t.FullNameOf() == "System.Object")
                break;

            var assembly = GetAssemblyNameFor(t);
            if (assembly is null || !SerenityNetAssemblies.Contains(assembly))
                continue;

#if ISSOURCEGENERATOR
            if (t is INamedTypeSymbol { IsGenericType: true })
#else
            if (t.IsGenericInstanceType(out _) || t.HasGenericParameters)
#endif
                continue;

            if (GetScriptType("@serenity-is/corelib:" + t.Name) is { IsEnum: not true })
                return t;
        }

        return null;
    }

    private static TypeReference? GetServiceBaseClass(TypeDefinition type)
    {
        foreach (var t in type.SelfAndBaseClasses())
        {
            if (t.BaseType != null &&
                t.BaseType.IsGenericInstanceType(out var originalDefinition) &&
                originalDefinition!.NamespaceOf() == "Serenity.Services")
            {
                var n = originalDefinition!.MetadataName();
                if (n == "ListResponse`1" || n == "RetrieveResponse`1" || n == "SaveRequest`1")
                    return t.BaseType;
            }

            if (t.NamespaceOf() != "Serenity.Services")
                continue;

            if (t.Name == "ListRequest" ||
                t.Name == "RetrieveRequest" ||
                t.Name == "DeleteRequest" ||
                t.Name == "DeleteResponse" ||
                t.Name == "UndeleteRequest" ||
                t.Name == "UndeleteResponse" ||
                t.Name == "SaveResponse" ||
                t.Name == "ServiceRequest" ||
                t.Name == "ServiceResponse")
                return t;
        }

        return null;
    }
}