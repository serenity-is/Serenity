using System;
using System.Collections;

namespace Serenity.CodeGeneration
{
    public partial class ServerImportsGenerator : ServerImportGeneratorBase
    {
        private void GenerateEnum(Type enumType)
        {
            var codeNamespace = GetNamespace(enumType);
            var enumKey = EnumMapper.GetEnumTypeKey(enumType);

            cw.Indented("[EnumKey(\"");
            sb.Append(enumKey);
            sb.AppendLine("\"), PreserveMemberCase]");

            cw.Indented("public enum ");
            sb.AppendLine(enumType.Name);
            cw.InBrace(delegate
            {
                var names = Enum.GetNames(enumType);
                var values = Enum.GetValues(enumType);

                int i = 0;
                int inserted = 0;
                foreach (var name in names)
                {
                    // If element of enum has IgnoreAttribute, skip it
                    if (EnumMapper.GetIgnoreAttribute(enumType, ((IList)values)[i]))
                    {
                        i++;
                        continue;
                    }

                    if (inserted > 0)
                        sb.AppendLine(",");

                    cw.Indented(name);
                    sb.Append(" = ");
                    sb.Append(Convert.ToInt32(((IList)values)[i]));
                    i++;
                    inserted++;
                }


                sb.AppendLine();
            });
        }
    }
}