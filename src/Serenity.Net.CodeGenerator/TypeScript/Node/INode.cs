namespace Serenity.TypeScript;

internal interface INode : ITextRange
{
    SyntaxKind Kind { get; set; }
    internal NodeFlags Flags { get; set; }

    INode Parent { get; set; }

    string GetText(string sourceText = null);
    string GetTextWithTrivia(string sourceText = null);

    string ToString(bool withPos);
}