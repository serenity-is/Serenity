namespace Serenity.CodeGenerator;

public class RawCode(string code)
{
    public string Code { get; } = code ?? throw new ArgumentNullException(nameof(code));

    public override string ToString()
    {
        return Code;
    }
}