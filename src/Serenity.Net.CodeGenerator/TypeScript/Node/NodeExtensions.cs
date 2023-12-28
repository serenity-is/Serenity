namespace Serenity.TypeScript;

internal static class NodeExtensions
{
    public static SourceFile GetSourceFile(this INode node)
    {
        while (node != null && node is not SourceFile)
            node = node.Parent;
        return node as SourceFile;
    }
}