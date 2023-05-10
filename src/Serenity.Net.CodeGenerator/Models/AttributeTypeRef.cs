namespace Serenity.CodeGenerator;

public class AttributeTypeRef
{
    public string TypeName { get; set; }
    public string Arguments { get; set; }

    public AttributeTypeRef(string typeName, string arguments = null)
    {
        TypeName = typeName;
        Arguments = arguments;
    }
}