namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    protected void GenerateBasicType(TypeDefinition type, bool module)
    {
        var codeNamespace = module ? null : GetNamespace(type);

        cw.Indented("export interface ");

        var identifier = MakeFriendlyName(type, codeNamespace, module);
        var baseClass = GetBaseClass(type);

        RegisterGeneratedType(codeNamespace, identifier, module, typeOnly: true);

        if (baseClass != null)
        {
            sb.Append(" extends ");
            MakeFriendlyReference(baseClass, codeNamespace, module);
        }

        cw.InBrace(delegate
        {
            GenerateBasicTypeMembers(type, baseClass, codeNamespace, module);
        });
    }

    void GenerateBasicTypeMembers(TypeDefinition type, TypeReference baseClass, 
        string codeNamespace, bool module)
    {
        void handleMember(TypeReference memberType, string memberName, IEnumerable<CustomAttribute> a)
        {
            if (!CanHandleType(memberType.Resolve()))
                return;

            var jsonProperty = a != null ? TypingsUtils.FindAttr(a, "Newtonsoft.Json", "JsonPropertyAttribute") : null;
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
            HandleMemberType(memberType, codeNamespace, module);
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

                if (TypingsUtils.FindAttr(field.GetAttributes(), "Newtonsoft.Json", "JsonIgnoreAttribute") != null)
                    continue;

                handleMember(field.FieldType(), field.Name, field.GetAttributes());
            }

            foreach (var property in current.PropertiesOf())
            {
                if (!TypingsUtils.IsPublicInstanceProperty(property))
                    continue;

                if (property.HasCustomAttributes() &&
                    TypingsUtils.FindAttr(property.GetAttributes(), "Newtonsoft.Json", "JsonIgnoreAttribute") != null)
                    continue;

                handleMember(property.PropertyType(), property.Name, property.GetAttributes());
                continue;
            }
        }
        while ((current = current.BaseType?.Resolve()) != null &&
            (baseClass == null || !TypingsUtils.IsAssignableFrom(current, baseClass)));
    }
}