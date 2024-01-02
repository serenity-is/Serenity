namespace Serenity.TypeScript;

public static class NodeExtensions
{
    public static SourceFile GetSourceFile(this INode node)
    {
        while (node != null && node is not SourceFile)
            node = node.Parent;
        return node as SourceFile;
    }

    public static bool HasModifier(this INode node, SyntaxKind kind)
    {
        return node is IHasModifierLike hasModifiersLike &&
            hasModifiersLike.Modifiers != null &&
            hasModifiersLike.Modifiers.Any(x => x.Kind == kind);
    }

    public static IEnumerable<Decorator> GetDecorators(this INode node)
    {
        if (node is not IHasModifierLike hasModifiersLike ||
            hasModifiersLike.Modifiers == null)
            return [];

        return hasModifiersLike.Modifiers.OfType<Decorator>();
    }

    public static string GetText(this INode node, string sourceText = null)
    {
        if ((sourceText ??= node.GetSourceFile()?.Text) == null)
            return null;

        if (node.Pos == null)
            return null;

        var pos = Scanner.SkipTrivia(sourceText, node.Pos) ?? node.Pos ?? 0;
        if (node.End < pos)
            return null;

        return sourceText[pos..node.End.Value];
    }

    public static string GetTextWithTrivia(this INode node, string sourceText = null)
    {
        if ((sourceText ??= node.GetSourceFile()?.Text) == null)
            return null;

        if (node.Pos == null || node.End < node.Pos)
            return null;

        return sourceText[(int)node.Pos..(int)node.End];
    }

    public static INode ForEachChild(this INode node, Func<INode, INode> visitor, bool recursively = false)
    {
        if (node == null)
            return null;

        INode result;

        if (node is IHasModifierLike hasModifierLike && hasModifierLike.Modifiers != null)
        {
            foreach (var modifier in hasModifierLike.Modifiers)
            {
                if (modifier == null)
                    continue;

                if ((result = visitor(modifier)) != null)
                    return result;

                if (recursively &&
                    (modifier is IHasModifierLike || modifier is IGetRestChildren) &&
                    (result = ForEachChild(modifier, visitor, recursively: true)) != null)
                    return result;
            }
        }

        if (node is not IGetRestChildren getRestChildren)
            return null;

        var children = getRestChildren.GetRestChildren();
        if (children == null)
            return null;

        foreach (var child in children)
        {
            if (child == null)
                continue;

            if ((result = visitor(child)) != null)
                return result;

            if (recursively && 
                (child is IHasModifierLike || child is IGetRestChildren) &&
                (result = ForEachChild(child, visitor, recursively: true)) != null)
                return result;
        }

        return null;
    }
}