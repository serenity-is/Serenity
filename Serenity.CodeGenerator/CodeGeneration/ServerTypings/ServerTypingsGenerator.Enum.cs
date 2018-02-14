using System;
using Serenity.Reflection;
#if COREFX
using System.Reflection;
#endif
using Mono.Cecil;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : CecilImportGenerator
    {
        private void GenerateEnum(TypeDefinition enumType)
        {
            var codeNamespace = GetNamespace(enumType);
            string enumKey = enumType.FullName;
            var enumKeyAttr = CecilUtils.FindAttr(enumType.CustomAttributes, "Serenity.ComponentModel", "EnumKeyAttribute");
            if (enumKeyAttr != null &&
                enumKeyAttr.ConstructorArguments.Count >= 1 &&
                enumKeyAttr.ConstructorArguments[0].Type.FullName == "System.String")
                enumKey = enumKeyAttr.ConstructorArguments[0].Value as string;

            cw.Indented("export enum ");
            var identifier = MakeFriendlyName(enumType, codeNamespace);
            var fullName = (codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + identifier;
            generatedTypes.Add(fullName);

            cw.InBrace(delegate
            {
                var inserted = 0;
                foreach (var field in enumType.Fields)
                {
                    if (!field.IsStatic || field.IsSpecialName || field.Constant == null)
                        continue;

                    if (CecilUtils.GetAttr(enumType, "Serenity.ComponentModel", "IgnoreAttribute") != null)
                        continue;

                    if (inserted > 0)
                        sb.AppendLine(",");

                    cw.Indented(field.Name);
                    sb.Append(" = ");
                    sb.Append(Convert.ToInt64(field.Constant));
                    inserted++;
                }

                sb.AppendLine();
            });

            cw.Indented("Serenity.Decorators.registerEnumType(");
            sb.Append(enumType.Name);
            sb.Append(", '");
            sb.Append(fullName);
            sb.Append("'");
            if (enumKey != fullName)
            {
                sb.Append(", '");
                sb.Append(enumKey);
                sb.AppendLine("');");
            }
            else
                sb.AppendLine(");");
        }
    }
}