using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class Program
{
    public static void Main(string[] args)
    {
        var fileSystem = new PhysicalGeneratorFileSystem();
        var console = new GeneratorConsole();
        var cli = new Cli(fileSystem, console)
        {
            BuildSystemFactory = () => new MSBuild.MSBuildProjectSystem()
        };
        
        var exitCode = cli.Run(args);
        if (exitCode != ExitCodes.Success &&
            exitCode != ExitCodes.Help)
            Environment.Exit((int)exitCode);
    }
}