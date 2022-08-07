namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : TypingsGeneratorBase
    {
        private void GenerateColumns(TypeDefinition type, CustomAttribute columnsAttribute, bool module)
        {
            var codeNamespace = GetNamespace(type);

            var identifier = type.Name;

            cw.Indented("export class ");
            sb.Append(identifier);

            cw.InBrace(delegate
            {
                cw.Indented("static columnsKey = '");
                var key = columnsAttribute.ConstructorArguments() != null &&
                    columnsAttribute.ConstructorArguments().Count > 0 ? columnsAttribute.ConstructorArguments()[0].Value as string : null;
                key ??= type.FullNameOf();

                sb.Append(key);
                sb.AppendLine("';");
            });

            RegisterGeneratedType((string.IsNullOrEmpty(codeNamespace) ? "" : codeNamespace + ".") + identifier, module);
        }
    }
}