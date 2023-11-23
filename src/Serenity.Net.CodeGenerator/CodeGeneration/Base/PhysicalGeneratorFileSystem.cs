namespace Serenity.CodeGeneration;

public class PhysicalGeneratorFileSystem : PhysicalFileSystem
{
    public DateTime GetLastWriteTime(string path)
    {
        return System.IO.File.GetLastWriteTime(path);
    }


}