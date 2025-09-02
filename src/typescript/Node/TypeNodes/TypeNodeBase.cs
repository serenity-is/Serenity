namespace Serenity.TypeScript;

public class TypeNodeBase(SyntaxKind kind) 
    : Node(kind), ITypeNode
{
}
