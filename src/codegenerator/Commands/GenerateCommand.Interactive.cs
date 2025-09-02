using Spectre.Console;

namespace Serenity.CodeGenerator;

public partial class GenerateCommand
{
    private List<string> SelectWhatToGenerate()
    {
        var whatToGenerate = new List<string>
        {
            "Row",
            "Services",
            "User Interface",
            "Custom"
        };

        var prompt = new MultiSelectionPrompt<string>()
            .Title("[steelblue1]Choose What to Generate[/]")
            .InstructionsText(
                "[grey](Press [blue]<space>[/] to select/unselect, " +
                "[grey]up and down to navigate[/], " +
                "[green]<enter>[/] to accept)[/]")
            .AddChoiceGroup("All", whatToGenerate);

        prompt.Select("All");
        foreach (var x in whatToGenerate)
            prompt.Select(x);

        return Console.Prompt(prompt);
    }

    private string SelectPermissionKey(string table, string defaultPermissionKey)
    {
        return Console.Prompt(
            new TextPrompt<string>($"Enter a Permission Key for table [springgreen3_1]{table}[/]")
                .DefaultValue(defaultPermissionKey)
                .Validate(module =>
                {
                    if (string.IsNullOrEmpty(module))
                        return ValidationResult.Error("[red]Can not be empty[/]");

                    if (module.IndexOf(' ') > -1)
                        return ValidationResult.Error("[red]Can not contains space[/]");

                    return ValidationResult.Success();
                }));
    }

    private string SelectIdentifier(string table, string defaultIdentifier)
    {
        Console.WriteLine();
        return Console.Prompt(
            new TextPrompt<string>($"Enter a class Identifier for table [springgreen3_1]{table}[/]")
                .Validate(module =>
                {
                    if (string.IsNullOrEmpty(module))
                        return ValidationResult.Error("[red]Can not be empty[/]");

                    if (module.IndexOf(' ') > -1)
                        return ValidationResult.Error("[red]Can not contains space[/]");

                    return ValidationResult.Success();
                })
                .DefaultValue(defaultIdentifier));
    }

    private string SelectModule(string table, string defaultModule)
    {
        Console.WriteLine();
        return Console.Prompt(
            new TextPrompt<string>($"Enter a Module name for table [springgreen3_1]{table}[/]")
                .Validate(module =>
                {
                    if (string.IsNullOrEmpty(module))
                        return ValidationResult.Error("[red]Can not be empty[/]");

                    if (module.IndexOf(' ') > -1)
                        return ValidationResult.Error("[red]Can not contains space[/]");

                    return ValidationResult.Success();
                })
                .DefaultValue(defaultModule));
    }

    private List<string> SelectTables(IEnumerable<string> tables)
    {
        return Console.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("[steelblue1]Select tables for code generation (single/multiple)[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more tables)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to select/unselect, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoices(tables));
    }

    private string SelectConnection(ConnectionStringOptions options)
    {
        var connectionKeys = options.Keys.OrderBy(x => x).ToArray();

        RegisterSqlProviders();
        Console.WriteLine();
        var selections = new SelectionPrompt<string>()
                .Title("[steelblue1]Available Connections[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more connections)[/]")
                .AddChoices(connectionKeys);

        return Console.Prompt(selections);
    }
}
