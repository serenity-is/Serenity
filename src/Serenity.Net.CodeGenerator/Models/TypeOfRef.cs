namespace Serenity.CodeGenerator;

public class TypeOfRef
{
    public string TypeName { get; }

    public TypeOfRef(string typeName)
    {
        TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
    }

    public string ToString(CodeWriter cw)
    {
        return "typeof(" + cw.ShortTypeRef(TypeName) + ")";
    }
}