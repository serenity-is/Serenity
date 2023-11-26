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

        try
        {
            var exitCode = cli.Run(new ArgumentReader(args));
            if (exitCode != ExitCodes.Success &&
                exitCode != ExitCodes.Help)
                Environment.Exit((int)exitCode);
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException)
            {
                console.Error(ex.Message);
                Environment.Exit((int)ExitCodes.InvalidArguments);
            }

            console.Exception(ex);
            Environment.Exit((int)ExitCodes.Exception);
        }
    }
}