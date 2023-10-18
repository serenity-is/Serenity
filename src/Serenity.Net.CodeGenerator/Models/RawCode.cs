namespace Serenity.CodeGenerator;

public class RawCode
{
    public string Code { get; }

    public RawCode(string code)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
    }

    public override string ToString()
    {
        return Code;
    }
}