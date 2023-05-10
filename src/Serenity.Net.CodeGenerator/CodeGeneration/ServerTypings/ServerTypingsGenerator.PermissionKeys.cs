namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    protected void GeneratePermissionKeys(TypeDefinition type, bool module)
    {
        GeneratePermissionKeysFor(type);
    }

    protected void GeneratePermissionKeysFor(TypeDefinition type)
    {
        cw.Indented("export namespace ");
        sb.Append(type.Name);
        cw.InBrace(delegate
        {
            foreach (var fi in type.FieldsOf().Where(x =>
                x.IsPublic() && x.IsStatic && x.HasConstant() && x.Constant() is string &&
                x.DeclaringType().FullNameOf() == type.FullNameOf() &&
                x.FieldType().FullNameOf() == "System.String"))
            {
                cw.Indented("export const ");
                sb.Append(fi.Name);
                sb.Append(" = ");
                sb.Append((fi.Constant() as string).ToDoubleQuoted());
                sb.AppendLine(";");
            }

            if (!type.HasNestedTypes())
                return;

            foreach (var nested in type.NestedTypes())
            {
                if (TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "ScriptSkipAttribute") != null)
                    continue;

                sb.AppendLine();
                GeneratePermissionKeysFor(nested);
            }
        });
    }
}