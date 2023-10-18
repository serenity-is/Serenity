namespace Serenity.CodeGenerator;

public class TypeNameRef
{
    public string TypeName { get; }

    public TypeNameRef(string typeName)
    {
        TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
    }

    public string ToString(CodeWriter cw)
    {
        return "typeof(" + cw.ShortTypeRef(TypeName) + ")";
    }
}