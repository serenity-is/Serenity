namespace Serenity.CodeGenerator;

public interface IClassMetadata
{
    string Namespace { get; }
    string ClassName { get; }
    string FullName => string.IsNullOrEmpty(Namespace) ? ClassName : Namespace + "." + ClassName;
}