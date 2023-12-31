namespace Serenity.TypeScript;

internal class TypeNodeBase(SyntaxKind kind) 
    : Node(kind), ITypeNode
{
}
