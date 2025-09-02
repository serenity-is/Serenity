namespace Serenity.CodeGeneration;

public class ExternalMethod : ExternalMember
{
    public List<ExternalArgument> Arguments { get; set; }
    public bool? IsConstructor { get; set; }
}