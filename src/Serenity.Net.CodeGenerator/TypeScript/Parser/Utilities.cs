using static Serenity.TypeScript.NodeVisitor;
using static Serenity.TypeScript.Scanner;

namespace Serenity.TypeScript;

public class Utilities
{
    public static int GetFullWidth(INode node)
    {
        return (node.End ?? 0) - (node.Pos ?? 0);
    }


    public static INode ContainsParseError(INode node)
    {
        AggregateChildData(node);

        return (node.Flags & NodeFlags.ThisNodeOrAnySubNodesHasError) != 0 ? node : null;
    }


    public static void AggregateChildData(INode node)
    {
        if ((node.Flags & NodeFlags.HasAggregatedChildData) != 0)
        {
            var thisNodeOrAnySubNodesHasError = (node.Flags & NodeFlags.ThisNodeHasError) != 0 ||
                                                ForEachChild(node, ContainsParseError) != null;
            if (thisNodeOrAnySubNodesHasError)
                node.Flags |= NodeFlags.ThisNodeOrAnySubNodesHasError;


            node.Flags |= NodeFlags.HasAggregatedChildData;
        }
    }


    public static bool NodeIsMissing(INode node)
    {
        if (node == null)
            return true;


        return node.Pos == node.End && node.Pos >= 0 && node.Kind != SyntaxKind.EndOfFileToken;
    }


    public static string GetTextOfNodeFromSourceText(string sourceText, INode node)
    {
        if (NodeIsMissing(node))
            return "";

        var start = SkipTrivia(sourceText, node.Pos ?? 0);

        if (node.End == null)
            return sourceText[start..];

        return sourceText[start..node.End.Value];
    }

    public static List<CommentRange> GetLeadingCommentRangesOfNodeFromText(INode node, string text)
    {
        return GetLeadingCommentRanges(text, node.Pos ?? 0);
    }

    public static List<CommentRange> GetJsDocCommentRanges(INode node, string text)
    {
        var commentRanges = node.Kind == SyntaxKind.Parameter ||
                            node.Kind == SyntaxKind.TypeParameter ||
                            node.Kind == SyntaxKind.FunctionExpression ||
                            node.Kind == SyntaxKind.ArrowFunction
            ? GetTrailingCommentRanges(text, node.Pos ?? 0).Concat(GetLeadingCommentRanges(text, node.Pos ?? 0))
            : GetLeadingCommentRangesOfNodeFromText(node, text);
        commentRanges ??= new List<CommentRange>();
        return commentRanges.Where(comment =>
                text[(comment.Pos ?? 0) + 1] == '*' &&
                text[(comment.Pos ?? 0) + 2] == '*' &&
                text[(comment.Pos ?? 0) + 3] != '/')
            .ToList();
    }



    public static bool IsModifierKind(SyntaxKind token)
    {
        return token switch
        {
            SyntaxKind.AbstractKeyword or SyntaxKind.AsyncKeyword or SyntaxKind.ConstKeyword or SyntaxKind.DeclareKeyword or SyntaxKind.DefaultKeyword or SyntaxKind.ExportKeyword or SyntaxKind.PublicKeyword or SyntaxKind.PrivateKeyword or SyntaxKind.ProtectedKeyword or SyntaxKind.ReadonlyKeyword or SyntaxKind.StaticKeyword => true,
            _ => false,
        };
    }


    public static bool IsParameterDeclaration(IVariableLikeDeclaration node)
    {
        var root = GetRootDeclaration(node);

        return root.Kind == SyntaxKind.Parameter;
    }


    public static INode GetRootDeclaration(INode node)
    {
        while (node.Kind == SyntaxKind.BindingElement)
            node = node.Parent.Parent;

        return node;
    }


    public static bool HasModifiers(Node node)
    {
        return GetModifierFlags(node) != ModifierFlags.None;
    }

    public static bool HasModifier(INode node, ModifierFlags flags)
    {
        return (GetModifierFlags(node) & flags) != 0;
    }

    public static ModifierFlags GetModifierFlags(INode node)
    {
        if ((node.ModifierFlagsCache & ModifierFlags.HasComputedFlags) != 0)
            return node.ModifierFlagsCache & ~ModifierFlags.HasComputedFlags;
        var flags = ModifierFlags.None;
        if (node.Modifiers != null)
            foreach (var modifier in node.Modifiers)
                flags |= ModifierToFlag(modifier.Kind);
        if ((node.Flags & NodeFlags.NestedNamespace) != 0 || node.Kind == SyntaxKind.Identifier &&
            ((Identifier)node).IsInJsDocNamespace)
            flags |= ModifierFlags.Export;


        node.ModifierFlagsCache = flags | ModifierFlags.HasComputedFlags;

        return flags;
    }

    public static ModifierFlags ModifiersToFlags(IEnumerable<Modifier> modifiers)
    {
        var flags = ModifierFlags.None;
        if (modifiers != null)
        {
            foreach (var modifier in modifiers)
            {
                flags |= ModifierToFlag(modifier.Kind);
            }
        }
        return flags;
    }

    public static ModifierFlags ModifierToFlag(SyntaxKind token)
    {
        return token switch
        {
            SyntaxKind.StaticKeyword => ModifierFlags.Static,
            SyntaxKind.PublicKeyword => ModifierFlags.Public,
            SyntaxKind.ProtectedKeyword => ModifierFlags.Protected,
            SyntaxKind.PrivateKeyword => ModifierFlags.Private,
            SyntaxKind.AbstractKeyword => ModifierFlags.Abstract,
            SyntaxKind.ExportKeyword => ModifierFlags.Export,
            SyntaxKind.DeclareKeyword => ModifierFlags.Ambient,
            SyntaxKind.ConstKeyword => ModifierFlags.Const,
            SyntaxKind.DefaultKeyword => ModifierFlags.Default,
            SyntaxKind.AsyncKeyword => ModifierFlags.Async,
            SyntaxKind.ReadonlyKeyword => ModifierFlags.Readonly,
            _ => ModifierFlags.None,
        };
    }


    public static bool IsLogicalOperator(SyntaxKind token)
    {
        return token == SyntaxKind.BarBarToken
               || token == SyntaxKind.AmpersandAmpersandToken
               || token == SyntaxKind.ExclamationToken;
    }


    public static bool IsAssignmentOperator(SyntaxKind token)
    {
        return token >= SyntaxKind.FirstAssignment && token <= SyntaxKind.LastAssignment;
    }


    public static bool IsLeftHandSideExpressionKind(SyntaxKind kind)
    {
        return kind == SyntaxKind.PropertyAccessExpression
               || kind == SyntaxKind.ElementAccessExpression
               || kind == SyntaxKind.NewExpression
               || kind == SyntaxKind.CallExpression
               || kind == SyntaxKind.JsxElement
               || kind == SyntaxKind.JsxSelfClosingElement
               || kind == SyntaxKind.TaggedTemplateExpression
               || kind == SyntaxKind.ArrayLiteralExpression
               || kind == SyntaxKind.ParenthesizedExpression
               || kind == SyntaxKind.ObjectLiteralExpression
               || kind == SyntaxKind.ClassExpression
               || kind == SyntaxKind.FunctionExpression
               || kind == SyntaxKind.Identifier
               || kind == SyntaxKind.RegularExpressionLiteral
               || kind == SyntaxKind.NumericLiteral
               || kind == SyntaxKind.StringLiteral
               || kind == SyntaxKind.NoSubstitutionTemplateLiteral
               || kind == SyntaxKind.TemplateExpression
               || kind == SyntaxKind.FalseKeyword
               || kind == SyntaxKind.NullKeyword
               || kind == SyntaxKind.ThisKeyword
               || kind == SyntaxKind.TrueKeyword
               || kind == SyntaxKind.SuperKeyword
               || kind == SyntaxKind.NonNullExpression
               || kind == SyntaxKind.MetaProperty;
    }


    public static bool IsLeftHandSideExpression(IExpression node)
    {
        return IsLeftHandSideExpressionKind(SkipPartiallyEmittedExpressions(node).Kind);
    }

    public static ScriptKind EnsureScriptKind(string fileName, ScriptKind scriptKind)
    {
        // Using scriptKind as a condition handles both:
        // - 'scriptKind' is unspecified and thus it is `null`
        // - 'scriptKind' is set and it is `Unknown` (0)
        // If the 'scriptKind' is 'null' or 'Unknown' then we attempt
        // to get the ScriptKind from the file name. If it cannot be resolved
        // from the file name then the default 'TS' script kind is returned.
        var sk = scriptKind != ScriptKind.Unknown ? scriptKind : GetScriptKindFromFileName(fileName);
        return sk != ScriptKind.Unknown ? sk : ScriptKind.TS;
    }
    public static ScriptKind GetScriptKindFromFileName(string fileName)
    {
        //var ext = fileName.substr(fileName.LastIndexOf("."));
        var ext = System.IO.Path.GetExtension(fileName);
        return (ext?.ToLower()) switch
        {
            ".js" => ScriptKind.JS,
            ".jsx" => ScriptKind.JSX,
            ".ts" => ScriptKind.TS,
            ".tsx" => ScriptKind.TSX,
            _ => ScriptKind.Unknown,
        };
    }

    public static string NormalizePath(string path)
    {
        path = path.Replace('\\', '/');
        var rootLength = GetRootLength(path);
        var root = path[..rootLength];
        var normalized = GetNormalizedParts(path, rootLength);
        if (normalized.Count != 0)
        {
#if ISSOURCEGENERATOR
            var joinedParts = root + string.Join("/", normalized);
#else
            var joinedParts = root + string.Join('/', normalized);
#endif
            return path[^1] == '/' ? joinedParts + '/' : joinedParts;
        }
        else
        {
            return root;
        }
    }

    public static int GetRootLength(string path)
    {
        if (path[0] == '/')
        {
            if (path.Length < 2 || path[1] != '/')
                return 1;
            var p1 = path.IndexOf('/', 2);
            if (p1 < 0)
                return 2;
            var p2 = path.IndexOf('/', p1 + 1);
            if (p2 < 0)
                return p1 + 1;
            return p2 + 1;
        }

        if (path.Length > 1 && path[1] == ':')
        {
            if (path.Length > 2 && path[2] == '/')
                return 3;
            return 2;
        }

        if (path.LastIndexOf("file:///", 0, StringComparison.Ordinal) == 0)
            return "file:///".Length;
        var idx = path.IndexOf("://", StringComparison.Ordinal);
        if (idx != -1)
            return idx + "://".Length;
        return 0;
    }

    private static List<string> GetNormalizedParts(string normalizedSlashedPath, int rootLength)
    {
        var parts = normalizedSlashedPath[rootLength..].Split('/');
        List<string> normalized = [];
        foreach (var part in parts)
        {
            if (part == ".")
                continue;
            if (part == ".." && normalized.Count > 0 && normalized.LastOrDefault() != "..")
                normalized.RemoveAt(normalized.Count - 1);
            else if (!string.IsNullOrEmpty(part))
                normalized.Add(part);
        }
        return normalized;
    }

    public static bool FileExtensionIs(string path, string extension)
    {
        return path.EndsWith(extension, StringComparison.Ordinal);
    }

    public static Diagnostic CreateFileDiagnostic(SourceFile file, int start, int length, DiagnosticMessage message, object argument)
    {
        return new Diagnostic
        {
            File = file,
            Start = start,
            Length = length,
            Message = message,
            Argument = argument
        };
    }

    public static INode SkipPartiallyEmittedExpressions(INode node)
    {
        while (node.Kind == SyntaxKind.PartiallyEmittedExpression)
            node = ((PartiallyEmittedExpression)node).Expression;


        return node;
    }
}