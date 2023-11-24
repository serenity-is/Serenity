namespace Serenity.CodeGenerator;

public interface IGeneratorConsole
{
    void Error(string message);
    void Exception(Exception exception);
#if !ISSOURCEGENERATOR
    T Prompt<T>(Spectre.Console.IPrompt<T> prompt);
#endif
    string ReadLine();
    void ShowHelp(string message);
    void Write(string message);
    void Write(string message, ConsoleColor color);
    void WriteLine();
    void WriteLine(string message);
    void WriteLine(string message, ConsoleColor color);
}