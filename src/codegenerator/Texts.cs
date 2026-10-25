namespace Serenity.CodeGenerator;

public static class Texts
{
    public static string Help => $"""""
        Serenity Code Generator {Assembly.GetEntryAssembly()?.GetName().Version}
        Usage: sergen [switches] [command]

        Commands:
            c[lienttypes]     Imports editor and formatter types from TypeScript to C#.
            doctor            Analyzes the project and tooling environment.
            g[enerate]        Launches the table code generator.
            m[vc]             Generates IntelliSense helpers for esm entry points and view locations.
            mvct              Executes client types and mvc commands simulatenously.
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

    public static string GenerateHelp => $"""
        Serenity Code Generator {Assembly.GetEntryAssembly()?.GetName().Version}
        Usage: sergen g [switches]

        Launches the table code generator.

        Switches:
            --cnk, --connection-key <key>  Connection key to use. Required when the project
                                          has several connections and prompts are disabled.
            --tbl, --table <table>         Table to generate code for, e.g. dbo.Movie
                                          (SQLite needs no schema). Required when prompts
                                          are disabled.
            --mod, --module <module>       Module (namespace) for the generated code.
            --cls, --identifier <class>    Class name for the entity.
            --pms, --permission <key>      Permission key, e.g. Administration:General.
            --wtg, --what <R|S|U|C|*>      What to generate: Row, Services, User Interface,
                                          Custom, or * for all of them.

            --no-interactive               Never prompt. Values with a default fall back to
                                          it, --table (and --connection-key with several
                                          connections) become required, and existing files
                                          that would change make the command fail with
                                          exit code 13.
            --overwrite[:mode]             What to do with existing files whose content
                                          changed. Without a value it means "yes":
                                            yes    replace, keeping a *.bak copy
                                            no     fail with exit code 13
                                            skip   leave the file as it is
                                            force  replace, without a backup
                                          Omit it to be asked, or to fail when
                                          --no-interactive is used.

        """;

    public const string CantFindProject = "Can't find a project file at: {0}";

    public const string NoProjectFiles = "Multiple project files found in current directory!\n" +
        "Please run Sergen in a folder that contains only one Asp.Net Core project.";

    public const string MultipleProjectFiles = "Can't find a project file in current directory!\n" +
        "Please run Sergen in a folder that contains the Asp.Net Core project.";
}