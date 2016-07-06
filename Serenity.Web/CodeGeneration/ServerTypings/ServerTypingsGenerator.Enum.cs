using System;
using System.Collections;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : ServerImportGeneratorBase
    {
        private void GenerateEnum(Type enumType)
        {
            var codeNamespace = GetNamespace(enumType);
            var enumKey = EnumMapper.GetEnumTypeKey(enumType);

            cw.Indented("export enum ");
            var generatedName = MakeFriendlyName(enumType, codeNamespace);
            generatedTypes.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + generatedName);

            cw.InBrace(delegate
            {
                var names = Enum.GetNames(enumType);
                var values = Enum.GetValues(enumType);

                int i = 0;
                foreach (var name in names)
                {
                    if (i > 0)
                        sb.AppendLine(",");

                    cw.Indented(name);
                    sb.Append(" = ");
                    sb.Append(Convert.ToInt32(((IList)values)[i]));
                    i++;
                }

                sb.AppendLine();
            });

            cw.Indented("Serenity.Decorators.registerEnum(");
            sb.Append(enumType.Name);
            sb.Append(", '");
            sb.Append(enumKey);
            sb.AppendLine("');");
        }
    }
}