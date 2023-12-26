namespace Serenity.TypeScript;

public interface INode : ITextRange
{
    SyntaxKind Kind { get; set; }
    NodeFlags Flags { get; set; }
    ModifierFlags ModifierFlagsCache { get; set; }

    NodeArray<Decorator> Decorators { get; set; }

    NodeArray<Modifier> Modifiers { get; set; }

    INode Parent { get; set; }
    List<Node> Children { get; set; }
    List<JsDoc> JsDoc { get; set; }

    string GetText();
    string GetTextWithComments(string source = null);

    string ToString(bool withPos);

}