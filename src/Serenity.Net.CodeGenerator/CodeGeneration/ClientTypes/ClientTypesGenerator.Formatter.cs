using System;
using System.Linq;

namespace Serenity.CodeGeneration
{
    public partial class ClientTypesGenerator : ImportGeneratorBase
    {
        private void GenerateFormatter(ExternalType type, string name)
        {
            cw.Indented("public partial class ");
            sb.Append(name);
            sb.AppendLine(" : CustomFormatterAttribute");

            cw.InBrace(delegate
            {
                cw.Indented("public const string Key = \"");
                sb.Append(type.FullName);
                sb.AppendLine("\";");
                sb.AppendLine();

                cw.Indented("public ");
                sb.Append(name);
                sb.AppendLine("()");
                cw.IndentedLine("    : base(Key)");
                cw.IndentedLine("{");
                cw.IndentedLine("}");

                GenerateOptionMembers(type, skip: null, isWidget: false);
            });
        }

        private static bool IsFormatterType(ExternalType type)
        {
            if (type.IsAbstract)
                return false;

            if (type.GenericParameters.Count > 0)
                return false;

            if (type.AssemblyName != null &&
                type.AssemblyName.StartsWith("Serenity.", StringComparison.Ordinal))
                return false;

            return type.Interfaces.Any(x =>
                x == "Serenity.ISlickFormatter" ||
                x == "Slick.Formatter");
        }
    }
}
