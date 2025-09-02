namespace Serenity.CodeGenerator;

public class TypeOfRef(string typeName)
{
    public string TypeName { get; } = typeName ?? throw new ArgumentNullException(nameof(typeName));

    public string ToString(CodeWriter cw)
    {
        return "typeof(" + cw.ShortTypeRef(TypeName) + ")";
    }
}