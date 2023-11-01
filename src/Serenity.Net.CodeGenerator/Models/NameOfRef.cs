namespace Serenity.CodeGenerator;

public class NameOfRef
{
    public string PropertyName { get; }
    public string TypeName { get; }

    public NameOfRef(string typeName, string propertyName)
    {
        TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
    }

    public string ToString(CodeWriter cw)
    {
        return "nameof(" + cw.ShortTypeRef(TypeName) + "." + PropertyName + ")";
    }
}