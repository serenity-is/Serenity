namespace Serenity.CodeGeneration;

public class ExternalType
{
    public string SourceFile { get; set; }
    public string Module { get; set; }
    public string Namespace { get; set; }
    public string Name { get; set; }
    public string BaseType { get; set; }
    public List<string> Interfaces { get; set; }
    public List<ExternalAttribute> Attributes { get; set; }
    public List<ExternalMember> Fields { get; set; }
    public List<ExternalMethod> Methods { get; set; }
    public List<ExternalGenericParameter> GenericParameters { get; set; }
    public bool? IsAbstract { get; set; }
    public bool? IsDeclaration { get; set; }
    public bool? IsInterface { get; set; }

    public string FullName
    {
        get
        {
            return (string.IsNullOrEmpty(Module) ? "" : (Module + ":")) +
                (string.IsNullOrEmpty(Namespace) ? Name : Namespace + "." + Name);
        }
    }

}