namespace Serenity.CodeGenerator;

public class NameOfRef(string typeName, string propertyName)
{
    public string PropertyName { get; } = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
    public string TypeName { get; } = typeName ?? throw new ArgumentNullException(nameof(typeName));

    public string ToString(CodeWriter cw)
    {
        return "nameof(" + cw.ShortTypeRef(TypeName) + "." + PropertyName + ")";
    }
}