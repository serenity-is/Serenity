using static Serenity.TypeScript.Scanner;

namespace Serenity.TypeScript;

public class Utilities
{
    internal static int GetFullWidth(INode node)
    {
        return (node.End ?? 0) - (node.Pos ?? 0);
    }

    internal static LanguageVariant GetLanguageVariant(ScriptKind scriptKind)
    {
        // .tsx and .jsx files are treated as jsx language variant.
        return scriptKind == ScriptKind.TSX || scriptKind == ScriptKind.JSX || scriptKind == ScriptKind.JS ? LanguageVariant.JSX : LanguageVariant.Standard;
    }

    internal static INode ContainsParseError(INode node)
    {
        AggregateChildData(node);

        return (node.Flags & NodeFlags.ThisNodeOrAnySubNodesHasError) != 0 ? node : null;
    }

    internal static void AggregateChildData(INode node)
    {
        if ((node.Flags & NodeFlags.HasAggregatedChildData) != 0)
        {
            var thisNodeOrAnySubNodesHasError = (node.Flags & NodeFlags.ThisNodeHasError) != 0 ||
                node.ForEachChild(ContainsParseError) != null;
            if (thisNodeOrAnySubNodesHasError)
                node.Flags |= NodeFlags.ThisNodeOrAnySubNodesHasError;


            node.Flags |= NodeFlags.HasAggregatedChildData;
        }
    }

    internal static bool NodeIsMissing(INode node)
    {
        if (node == null)
            return true;


        return node.Pos == node.End && node.Pos >= 0 && node.Kind != SyntaxKind.EndOfFileToken;
    }

    internal static bool NodeIsPresent(INode node)
    {
        return !NodeIsMissing(node);
    }

    internal static bool IsInOrOfKeyword(SyntaxKind t)
    {
        return t == SyntaxKind.InKeyword || t == SyntaxKind.OfKeyword;
    }

    public static string GetTextOfNodeFromSourceText(string sourceText, INode node)
    {
        if (NodeIsMissing(node))
            return "";

        var start = SkipTrivia(sourceText, node.Pos ?? 0) ?? 0;

        if (node.End == null)
            return sourceText[start..];

        return sourceText[start..node.End.Value];
    }

    internal static bool IsModifierKind(SyntaxKind token)
    {
        return token switch
        {
            SyntaxKind.AbstractKeyword or SyntaxKind.AccessorKeyword or SyntaxKind.AsyncKeyword 
                or SyntaxKind.ConstKeyword or SyntaxKind.DeclareKeyword or SyntaxKind.DefaultKeyword 
                or SyntaxKind.ExportKeyword or SyntaxKind.InKeyword or SyntaxKind.PublicKeyword 
                or SyntaxKind.PrivateKeyword or SyntaxKind.ProtectedKeyword or SyntaxKind.ReadonlyKeyword 
                or SyntaxKind.StaticKeyword or SyntaxKind.OutKeyword or SyntaxKind.OverrideKeyword => true,
            _ => false,
        };
    }

    internal static bool IsParameterPropertyModifier(SyntaxKind kind)
    {
        return (ModifierToFlag(kind) & ModifierFlags.ParameterPropertyModifier) != 0;
    }

    internal static bool IsClassMemberModifier(SyntaxKind idToken)
    {
        return IsParameterPropertyModifier(idToken) ||
            idToken == SyntaxKind.StaticKeyword ||
            idToken == SyntaxKind.OverrideKeyword ||
            idToken == SyntaxKind.AccessorKeyword;
    }

    internal static ModifierFlags ModifiersToFlags(IEnumerable<IModifierLike> modifiers)
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

    internal static ModifierFlags ModifierToFlag(SyntaxKind token)
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

    internal static bool IsAssignmentOperator(SyntaxKind token)
    {
        return token >= SyntaxKindMarker.FirstAssignment && token <= SyntaxKindMarker.LastAssignment;
    }

    internal static bool IsLeftHandSideExpressionKind(SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.PropertyAccessExpression or SyntaxKind.ElementAccessExpression or SyntaxKind.NewExpression 
                or SyntaxKind.CallExpression or SyntaxKind.JsxElement or SyntaxKind.JsxSelfClosingElement 
                or SyntaxKind.JsxFragment or SyntaxKind.TaggedTemplateExpression 
                or SyntaxKind.ArrayLiteralExpression or SyntaxKind.ParenthesizedExpression 
                or SyntaxKind.ObjectLiteralExpression or SyntaxKind.ClassExpression 
                or SyntaxKind.FunctionExpression or SyntaxKind.Identifier or SyntaxKind.PrivateIdentifier 
                or SyntaxKind.RegularExpressionLiteral or SyntaxKind.NumericLiteral or SyntaxKind.BigIntLiteral 
                or SyntaxKind.StringLiteral or SyntaxKind.NoSubstitutionTemplateLiteral 
                or SyntaxKind.TemplateExpression or SyntaxKind.FalseKeyword or SyntaxKind.NullKeyword 
                or SyntaxKind.ThisKeyword or SyntaxKind.TrueKeyword or SyntaxKind.SuperKeyword 
                or SyntaxKind.NonNullExpression or SyntaxKind.ExpressionWithTypeArguments 
                or SyntaxKind.MetaProperty or SyntaxKind.ImportKeyword 
                or SyntaxKind.MissingDeclaration => true,
            _ => false,
        };
    }

    internal static bool IsLeftHandSideExpression(IExpression node)
    {
        return IsLeftHandSideExpressionKind(SkipPartiallyEmittedExpressions(node).Kind);
    }

    internal static ScriptKind EnsureScriptKind(string fileName, ScriptKind scriptKind)
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

    internal static ScriptKind GetScriptKindFromFileName(string fileName)
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
#if NETSTANDARD2_0
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

    internal static int GetRootLength(string path)
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

    internal static bool FileExtensionIs(string path, string extension)
    {
        return path.EndsWith(extension, StringComparison.Ordinal);
    }

    internal static Diagnostic CreateDetachedDiagnostic(string fileName, string sourceText, int start, int length, DiagnosticMessage message, object argument = null)
    {

        if ((start + length) > sourceText.Length)
        {
            length = sourceText.Length - start;
        }

        return new Diagnostic
        {
            FileName = fileName,
            Start = start,
            Length = length,
            Message = message,
            Argument = argument
        };
    }

    internal static Diagnostic CreateFileDiagnostic(SourceFile file, int start, int length, DiagnosticMessage message, object argument)
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

    internal static ITemplateLiteralLikeNode CreateTemplateLiteralLikeNode(SyntaxKind kind, string text, string rawText, TokenFlags? templateFlags)
    {
        if (kind == SyntaxKind.NoSubstitutionTemplateLiteral)
            return new NoSubstitutionTemplateLiteral(text, rawText, templateFlags ?? TokenFlags.None);

        return new TemplateLiteralLikeNode(kind, text, rawText, templateFlags);
    }

    internal static ILiteralLikeNode CreateLiteralLikeNode(SyntaxKind kind, string text)
    {
        return kind switch
        {
            SyntaxKind.NumericLiteral => new NumericLiteral(text, numericLiteralFlags: 0),
            SyntaxKind.BigIntLiteral => new BigIntLiteral(text),
            SyntaxKind.StringLiteral => new StringLiteral(text, isSingleQuote: null),
            SyntaxKind.JsxText => new JsxText(text, containsOnlyTriviaWhiteSpaces: false),
            SyntaxKind.JsxTextAllWhiteSpaces => new JsxText(text, containsOnlyTriviaWhiteSpaces: true),
            SyntaxKind.RegularExpressionLiteral => new RegularExpressionLiteral(text),
            SyntaxKind.NoSubstitutionTemplateLiteral => CreateTemplateLiteralLikeNode(kind, text, rawText: null, templateFlags: 0),
            _ => null,
        };
    }

    internal static INode SkipPartiallyEmittedExpressions(INode node)
    {
        while (node is PartiallyEmittedExpression partiallyEmitted)
            node = partiallyEmitted.Expression;

        return node;
    }

    private static bool FileExtensionIsOneOf(string path, string[] extensions)
    {
        foreach (var extension in extensions)
        {
            if (FileExtensionIs(path, extension))
            {
                return true;
            }
        }
        return false;
    }

    static readonly string[] SupportedDeclarationExtensions = [".d.ts" /* Dts */, ".d.cts" /* Dcts */, ".d.mts" /* Dmts */];

    internal static bool IsDeclarationFileName(string fileName)
    {
        return FileExtensionIsOneOf(fileName, SupportedDeclarationExtensions) ||
            (FileExtensionIs(fileName, ".ts") && System.IO.Path.GetFileName(fileName)
#if NETSTANDARD2_0
            .IndexOf(".d.") >= 0);
#else
            .Contains(".d.", StringComparison.Ordinal));
#endif
    }

    internal static string IdText(Identifier identifier)
    {
        return identifier.Text;
        // return unescapeLeadingUnderscores(identifierOrPrivateName.escapedText);
    }

    internal static bool CanHaveModifiers(INode node)
    {
        var kind = node.Kind;
        return kind == SyntaxKind.TypeParameter
            || kind == SyntaxKind.Parameter
            || kind == SyntaxKind.PropertySignature
            || kind == SyntaxKind.PropertyDeclaration
            || kind == SyntaxKind.MethodSignature
            || kind == SyntaxKind.MethodDeclaration
            || kind == SyntaxKind.Constructor
            || kind == SyntaxKind.GetAccessor
            || kind == SyntaxKind.SetAccessor
            || kind == SyntaxKind.IndexSignature
            || kind == SyntaxKind.ConstructorType
            || kind == SyntaxKind.FunctionExpression
            || kind == SyntaxKind.ArrowFunction
            || kind == SyntaxKind.ClassExpression
            || kind == SyntaxKind.VariableStatement
            || kind == SyntaxKind.FunctionDeclaration
            || kind == SyntaxKind.ClassDeclaration
            || kind == SyntaxKind.InterfaceDeclaration
            || kind == SyntaxKind.TypeAliasDeclaration
            || kind == SyntaxKind.EnumDeclaration
            || kind == SyntaxKind.ModuleDeclaration
            || kind == SyntaxKind.ImportEqualsDeclaration
            || kind == SyntaxKind.ImportDeclaration
            || kind == SyntaxKind.ExportAssignment
            || kind == SyntaxKind.ExportDeclaration;
    }

    internal static bool HasModifierOfKind(INode node, SyntaxKind kind)
    {
        return node is IHasModifierLike hasModifiers && hasModifiers.Modifiers != null &&
            hasModifiers.Modifiers.Any(m => m.Kind == kind);
    }

    static bool IsAnExternalModuleIndicatorNode(INode node)
    {
        return (CanHaveModifiers(node) && HasModifierOfKind(node, SyntaxKind.ExportKeyword))
            || (node is ImportEqualsDeclaration importEquals && importEquals.ModuleReference is ExternalModuleReference)
            || node is ImportDeclaration
            || node is ExportAssignment
            || node is ExportDeclaration;
    }

    static INode GetImportMetaIfNecessary(SourceFile sourceFile)
    {
        return (sourceFile.Flags & NodeFlags.PossiblyContainsImportMeta) != 0 ?
            WalkTreeForImportMeta(sourceFile) :
            null;
    }

    static INode WalkTreeForImportMeta(INode node)
    {
        return IsImportMeta(node) ? node : node.ForEachChild(WalkTreeForImportMeta);
    }

    static bool IsImportMeta(INode node)
    {
        return node is MetaProperty { KeywordToken: SyntaxKind.ImportKeyword } mp && mp.Name?.EscapedText == "meta";
    }

    internal static INode IsFileProbablyExternalModule(SourceFile sourceFile)
    {
        // Try to use the first top-level import/export when available, then
        // fall back to looking for an 'import.meta' somewhere in the tree if necessary.
        return sourceFile.Statements.FirstOrDefault(IsAnExternalModuleIndicatorNode) ??
            GetImportMetaIfNecessary(sourceFile);
    }

    internal static void SetExternalModuleIndicator(SourceFile sourceFile)
    {
        sourceFile.ExternalModuleIndicator = IsFileProbablyExternalModule(sourceFile);
    }

    internal static ITextRange SetTextRangePosEnd(ITextRange range, int pos, int end)
    {
        range.Pos = pos;
        range.End = end;
        return range;
    }

    internal static ITextRange SetTextRangePosWidth(ITextRange range, int pos, int width)
    {
        return SetTextRangePosEnd(range, pos, pos + width);
    }

    internal static ITextRange SetTextRange(ITextRange range, ITextRange location)
    {
        return location != null ? SetTextRangePosEnd(range, location.Pos ?? 0, location.End ?? location.Pos ?? 0) : range;
    }

    internal static ITextRange SetTextRangePos(ITextRange range, int pos)
    {
        range.Pos = pos;
        return range;
    }

    internal static OperatorPrecedence GetBinaryOperatorPrecedence(SyntaxKind kind)
    {
        return kind switch
        {
            SyntaxKind.QuestionQuestionToken => OperatorPrecedence.Coalesce,
            SyntaxKind.BarBarToken => OperatorPrecedence.LogicalOR,
            SyntaxKind.AmpersandAmpersandToken => OperatorPrecedence.LogicalAND,
            SyntaxKind.BarToken => OperatorPrecedence.BitwiseOR,
            SyntaxKind.CaretToken => OperatorPrecedence.BitwiseXOR,
            SyntaxKind.AmpersandToken => OperatorPrecedence.BitwiseAND,
            SyntaxKind.EqualsEqualsToken or SyntaxKind.ExclamationEqualsToken or SyntaxKind.EqualsEqualsEqualsToken
                or SyntaxKind.ExclamationEqualsEqualsToken => OperatorPrecedence.Equality,
            SyntaxKind.LessThanToken or SyntaxKind.GreaterThanToken or SyntaxKind.LessThanEqualsToken
                or SyntaxKind.GreaterThanEqualsToken or SyntaxKind.InstanceOfKeyword or SyntaxKind.InKeyword
                or SyntaxKind.AsKeyword or SyntaxKind.SatisfiesKeyword => OperatorPrecedence.Relational,
            SyntaxKind.LessThanLessThanToken or SyntaxKind.GreaterThanGreaterThanToken
                or SyntaxKind.GreaterThanGreaterThanGreaterThanToken => OperatorPrecedence.Shift,
            SyntaxKind.PlusToken or SyntaxKind.MinusToken => OperatorPrecedence.Additive,
            SyntaxKind.AsteriskToken or SyntaxKind.SlashToken
                or SyntaxKind.PercentToken => OperatorPrecedence.Multiplicative,
            SyntaxKind.AsteriskAsteriskToken => OperatorPrecedence.Exponentiation,
            // -1 is lower than all other precedences.  Returning it will cause binary expression
            // parsing to stop.
            _ => (OperatorPrecedence)(-1),
        };
    }

    internal static bool TagNamesAreEquivalent(IJsxTagNameExpression lhs, IJsxTagNameExpression rhs)
    {
        if (lhs == null || rhs == null || lhs.Kind != rhs.Kind)
            return false;

        if (lhs is Identifier identifier && rhs is Identifier rhsIdentifier)
        {
            return identifier.EscapedText == rhsIdentifier.EscapedText;
        }

        if (lhs.Kind == SyntaxKind.ThisKeyword)
        {
            return true;
        }

        if (lhs is JsxNamespacedName lhsNamespaced && rhs is JsxNamespacedName rhsNamespaced)
        {
            return lhsNamespaced.Namespace?.EscapedText == rhsNamespaced.Namespace?.EscapedText &&
                lhsNamespaced.Name?.EscapedText == rhsNamespaced?.Name?.EscapedText;
        }

        // If we are at this statement then we must have PropertyAccessExpression and because tag name in Jsx element can only
        // take forms of JsxTagNameExpression which includes an identifier, "this" expression, or another propertyAccessExpression
        // it is safe to case the expression property as such. See parseJsxElementName for how we parse tag name in Jsx element
        return lhs is PropertyAccessExpression lhsAccessExpr && rhs is PropertyAccessExpression rhsAccessExpr &&
            lhsAccessExpr.Name?.EscapedText == rhsAccessExpr.Name?.EscapedText &&
            lhsAccessExpr.Expression is IJsxTagNameExpression lhsJsxTag && 
            rhsAccessExpr.Expression is IJsxTagNameExpression rhsJsxTag &&
            TagNamesAreEquivalent(lhsJsxTag, rhsJsxTag);
    }
}
