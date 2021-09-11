using Mono.Cecil;
using Serenity.Reflection;
using System;
using System.Globalization;
using System.Linq;

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
                var fields = enumType.Fields.Where(x => x.IsStatic && !x.IsSpecialName && x.Constant != null &&
                    (!x.HasCustomAttributes ||
                        CecilUtils.FindAttr(x.CustomAttributes, "Serenity.ComponentModel", "IgnoreAttribute") == null));

                fields = fields.OrderBy(x => Convert.ToInt64(x.Constant, CultureInfo.InvariantCulture));

                var inserted = 0;
                foreach (var field in fields)
                {
                    if (inserted > 0)
                        sb.AppendLine(",");

                    cw.Indented(field.Name);
                    sb.Append(" = ");
                    sb.Append(Convert.ToInt64(field.Constant, CultureInfo.InvariantCulture));
                    inserted++;
                }

                sb.AppendLine();
            });

            cw.Indented("Serenity.Decorators.registerEnumType(");
            sb.Append(enumType.Name);
            sb.Append(", '");
            sb.Append(fullName);
            sb.Append('\'');
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