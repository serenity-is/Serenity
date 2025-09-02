namespace Serenity.CodeGenerator;

public class GeneratorConsole : IGeneratorConsole
{
    public void Error(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        Console.Error.WriteLine(message);
    }

    public void Write(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        Console.Write(message);
    }

    public void Write(string message, ConsoleColor color)
    {
        ArgumentNullException.ThrowIfNull(message);
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }

    public void WriteLine()
    {
        Console.WriteLine();
    }

    public void WriteLine(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        Console.WriteLine(message);
    }

    public void WriteLine(string message, ConsoleColor color)
    {
        ArgumentNullException.ThrowIfNull(message);
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void ShowHelp(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        Console.WriteLine(message);
    }

    public void Exception(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        Console.Error.WriteLine(exception.Message);
    }

    public T Prompt<T>(Spectre.Console.IPrompt<T> prompt)
    {
        ArgumentNullException.ThrowIfNull(prompt);
        return prompt.Show(Spectre.Console.AnsiConsole.Console);
    }

    public string ReadLine()
    {
        return Console.ReadLine();
    }
}