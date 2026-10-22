namespace Serenity.TypeScript;

public static class NodeExtensions
{
    // Parent pointers are no longer stored; callers must pass source text explicitly.
    // public static SourceFile? GetSourceFile(this INode? node)
    // {
    //     while (node != null && node is not SourceFile)
    //         node = node.Parent;
    //     return node as SourceFile;
    // }

    public static bool HasModifier(this INode node, SyntaxKind kind)
    {
        if (node is not IHasModifierLike hasModifiersLike || hasModifiersLike.Modifiers == null)
            return false;

        var modifiers = hasModifiersLike.Modifiers;
        for (var i = 0; i < modifiers.Count; i++)
        {
            if (modifiers[i]?.Kind == kind)
                return true;
        }
        return false;
    }

    public static List<Decorator>? GetDecorators(this INode node)
    {
        if (node is not IHasModifierLike hasModifiersLike ||
            hasModifiersLike.Modifiers == null)
            return null;

        List<Decorator>? result = null;
        var modifiers = hasModifiersLike.Modifiers;
        for (var i = 0; i < modifiers.Count; i++)
        {
            if (modifiers[i] is Decorator decorator)
            {
                (result ??= []).Add(decorator);
            }
        }
        return result;
    }

    public static string? GetText(this INode node, string? sourceText = null)
    {
        // Parent pointers are no longer stored; the caller must supply the source text.
        if (sourceText == null)
            return null;

        if (node.Pos < 0 || node.End < 0)
            return null;

        var pos = Scanner.SkipTrivia(sourceText!, node.Pos) ?? node.Pos;
        if (node.End < pos)
            return null;

        return sourceText![pos..node.End];
    }

    public static string? GetTextWithTrivia(this INode node, string? sourceText = null)
    {
        // Parent pointers are no longer stored; the caller must supply the source text.
        if (sourceText == null)
            return null;

        if (node.Pos < 0 || node.End < 0 || node.End < node.Pos)
            return null;

        return sourceText![node.Pos..node.End];
    }

    public static INode? ForEachChild(this INode node, Func<INode, INode?> visitor, bool recursively = false)
    {
        if (node == null)
            return null;

        INode? result;

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