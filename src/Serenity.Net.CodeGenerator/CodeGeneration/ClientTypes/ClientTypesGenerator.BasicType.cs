namespace Serenity.CodeGeneration;

public partial class ClientTypesGenerator : ImportGeneratorBase
{
    private void GenerateBasicType(ExternalType type)
    {
        cw.Indented("public partial class ");
        sb.AppendLine(type.Name);
        
        cw.InBrace(delegate
        {
            GenerateBasicTypeMembers(type, skip: null);
        });
    }
    
    private void GenerateBasicTypeMembers(ExternalType type, HashSet<string> skip)
    {
        var options = GetBasicTypeMembers(type);
        
        var index = 0;
        foreach (var option in options.Values)
        {
            if (skip != null &&
                skip.Contains(option.Name))
                continue;

            if (!CSharpSyntaxRules.IsValidIdentifier(option.Name, ignoreKeywords: true))
                continue;

            var typeName = GetMemberTypeName(option.Type);

            if (index++ > 0)
                sb.AppendLine();
            cw.Indented("public ");
            sb.Append(typeName);
            sb.Append(' ');
            if (CSharpSyntaxRules.IsKeyword(option.Name))
                sb.Append('@');
            sb.Append(option.Name);
            sb.Append(" { get; set; }");
            sb.AppendLine();
        }
    }

    private SortedDictionary<string, ExternalMember> GetBasicTypeMembers(ExternalType basicType)
    {
        var result = new SortedDictionary<string, ExternalMember>();
        AddBasicTypeMembers(result, basicType, basicType);

        int loop = 0;
        do
        {
            if (basicType.Namespace?.StartsWith("System", StringComparison.Ordinal) == true)
                break;

            AddBasicTypeMembers(result, basicType, basicType);
        }
        while ((basicType = GetBaseType(basicType)) != null && loop++ < 100);

        return result;
    }

    private void AddBasicTypeMembers(SortedDictionary<string, ExternalMember> dict,
        ExternalType fromType, ExternalType basicType)
    {
        List<ExternalMember> members = [];

        if (basicType == null)
            return;

        if (basicType.IsIntersectionType == true && basicType.Interfaces != null)
        {
            foreach (var intersectedTypeName in basicType.Interfaces)
            {
                var intersectedType = GetScriptTypeFrom(fromType ?? basicType, intersectedTypeName);
                if (intersectedType != null)
                {
                    AddBasicTypeMembers(dict, basicType, intersectedType);
                }
            }
        }

        if (basicType.Fields != null)
            members.AddRange(basicType.Fields);

        foreach (var member in members)
        {
            if (dict.ContainsKey(member.Name))
                continue;

            if (IsComplexMemberTypeName(member.Type))
                continue;

            dict[member.Name] = member;
        }
    }
}
