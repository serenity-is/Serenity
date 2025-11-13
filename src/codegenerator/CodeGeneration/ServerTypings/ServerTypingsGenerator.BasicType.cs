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

    void GenerateBasicTypeMembers(TypeDefinition type, TypeReference baseClass, 
        string codeNamespace)
    {
        void handleMember(TypeReference memberType, string memberName, IEnumerable<CustomAttribute> a)
        {
            if (!CanHandleType(memberType.Resolve()))
                return;

            var jsonProperty = a != null ? (
                TypingsUtils.FindAttr(a, "Newtonsoft.Json", "JsonPropertyAttribute") ??
                TypingsUtils.FindAttr(a, "System.Text.Json.Serialization", "JsonPropertyNameAttribute")) : null;
            if (jsonProperty != null &&
                jsonProperty.HasConstructorArguments())
            {
                var arg = jsonProperty.ConstructorArguments.First();
                if (arg.Type.FullNameOf() == "System.String" &&
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

    protected static TypeReference GetBasicTypeBaseClass(TypeDefinition type)
    {
        foreach (var t in TypingsUtils.SelfAndBaseClasses(type))
        {
            if (t.BaseType != null &&
                t.BaseType.IsGenericInstance() &&
#if ISSOURCEGENERATOR
                t.BaseType.OriginalDefinition.NamespaceOf() == "Serenity.Services")
#else
                (t.BaseType as GenericInstanceType).ElementType.Namespace == "Serenity.Services")
#endif
            {
#if ISSOURCEGENERATOR
                var n = t.BaseType.OriginalDefinition.MetadataName();
#else
                var n = (t.BaseType as GenericInstanceType).ElementType.Name;
#endif
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