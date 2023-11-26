namespace Serenity.CodeGenerator;

public interface IGeneratedFileWriter
{
    public void WriteAllText(string targetFile, string contents);
}