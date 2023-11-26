namespace Serenity.CodeGenerator;

public static class Texts
{
    public static string Help => $"""""
        Serenity Code Generator {Assembly.GetEntryAssembly().GetName().Version}
        Usage: sergen [switches] [command]

        Commands:
            c[lienttypes]    Imports editor and formatter types from TypeScript to C#.
            g[enerate]        Launches the table code generator.
            m[vc]             Generates IntelliSense helpers for view locations.
            mvct              Executes client types and mvc commands simulatenously.
            r[estore]         [Obsolete] Restores content (e.g., scripts, CSS) from .nupkg.
            s[ervertypings]   Imports row, form, and service types from C# to TypeScript.
            t[ransform]       Executes clienttypes, mvc, and servertypings commands simultaneously.

        Switches:
            -p <ProjectFile>  Specifies the project file. Useful when multiple projects exist 
                            in the current directory.
            -prop:<n>=<v>     Provides hints to sergen for project-level properties, 
                            where <n> is the property name and <v> is its value.
                            Use a semicolon to separate multiple properties or specify each 
                            property separately. This is helpful for improving performance
                            as Sergen won't have to parse the project, and also for addressing 
                            cases where sergen might not determine a property correctly.

                            Examples:
                                -prop:Configuration=Release
                                -prop:"OutDir=..\bin\Debug\;AssemblyName=MyAssembly"
            
        """"";

    public const string CantFindProject = "Can't find a project file at: {0}";

    public const string NoProjectFiles = "Multiple project files found in current directory!\n" +
        "Please run Sergen in a folder that contains only one Asp.Net Core project.";

    public const string MultipleProjectFiles = "Can't find a project file in current directory!\n" +
        "Please run Sergen in a folder that contains the Asp.Net Core project.";
}