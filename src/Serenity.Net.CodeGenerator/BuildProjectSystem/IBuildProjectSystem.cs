namespace Serenity.CodeGenerator;

public interface IBuildProjectSystem
{
    IBuildProject LoadProject(string path);
}