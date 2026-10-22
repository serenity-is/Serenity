namespace Serenity.TypeScript;

public class TextRange : ITextRange
{
    public int Pos { get; set; } = Utilities.UnsetTextRange;
    public int End { get; set; } = Utilities.UnsetTextRange;
}
