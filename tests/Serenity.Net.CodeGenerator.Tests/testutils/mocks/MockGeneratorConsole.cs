using Serenity.CodeGenerator;

namespace Serenity.TestUtils;

public class MockGeneratorConsole : IGeneratorConsole
{
    public enum CallType
    {
        Error,
        Exception,
        Help,
        Prompt,
        Write,
        WriteLine
    };

    public readonly List<(CallType type, string message, object data)> WriteCalls = [];

    public void Write(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        WriteCalls.Add((CallType.Write, message, null));
    }

    public void Write(string message, ConsoleColor color)
    {
        ArgumentNullException.ThrowIfNull(message);
        WriteCalls.Add((CallType.Write, message, color));
    }

    public void ShowHelp(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        WriteCalls.Add((CallType.Help, message, null));
    }

    public void WriteLine()
    {
        WriteCalls.Add((CallType.WriteLine, null, null));
    }

    public void WriteLine(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        WriteCalls.Add((CallType.WriteLine, message, null));
    }

    public void WriteLine(string message, ConsoleColor color)
    {
        ArgumentNullException.ThrowIfNull(message);
        WriteCalls.Add((CallType.WriteLine, message, color));
    }

    public void Error(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        WriteCalls.Add((CallType.Error, message, null));
    }

    public void Exception(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        WriteCalls.Add((CallType.Exception, null, exception));
    }

    public Queue<object?> PromptResults { get; } = [];

    public virtual T Prompt<T>(Spectre.Console.IPrompt<T> prompt)
    {
        ArgumentNullException.ThrowIfNull(prompt);
        WriteCalls.Add((CallType.Prompt, null, prompt));
        return PromptResults.Count > 0 ? (T)PromptResults.Dequeue()! : default!;
    }

    public Queue<string> ReadLineAnswers { get; } = [];

    public virtual string ReadLine()
    {
        return ReadLineAnswers.Count > 0 ? ReadLineAnswers.Dequeue() : "Y";
    }
}