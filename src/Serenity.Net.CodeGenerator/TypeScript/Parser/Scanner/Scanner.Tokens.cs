using Serenity.TypeScript.TsTypes;

namespace Serenity.TypeScript.TsParser;

partial class Scanner
{
    private static readonly Dictionary<string, SyntaxKind> textToKeyword = new()
    {
        ["abstract"] = SyntaxKind.AbstractKeyword,
        ["accessor"] = SyntaxKind.AccessorKeyword,
        ["any"] = SyntaxKind.AnyKeyword,
        ["as"] = SyntaxKind.AsKeyword,
        ["asserts"] = SyntaxKind.AssertsKeyword,
        ["assert"] = SyntaxKind.AssertKeyword,
        ["bigint"] = SyntaxKind.BigIntKeyword,
        ["boolean"] = SyntaxKind.BooleanKeyword,
        ["break"] = SyntaxKind.BreakKeyword,
        ["case"] = SyntaxKind.CaseKeyword,
        ["catch"] = SyntaxKind.CatchKeyword,
        ["class"] = SyntaxKind.ClassKeyword,
        ["continue"] = SyntaxKind.ContinueKeyword,
        ["const"] = SyntaxKind.ConstKeyword,
        ["constructor"] = SyntaxKind.ConstructorKeyword,
        ["debugger"] = SyntaxKind.DebuggerKeyword,
        ["declare"] = SyntaxKind.DeclareKeyword,
        ["default"] = SyntaxKind.DefaultKeyword,
        ["delete"] = SyntaxKind.DeleteKeyword,
        ["do"] = SyntaxKind.DoKeyword,
        ["else"] = SyntaxKind.ElseKeyword,
        ["enum"] = SyntaxKind.EnumKeyword,
        ["export"] = SyntaxKind.ExportKeyword,
        ["extends"] = SyntaxKind.ExtendsKeyword,
        ["false"] = SyntaxKind.FalseKeyword,
        ["finally"] = SyntaxKind.FinallyKeyword,
        ["for"] = SyntaxKind.ForKeyword,
        ["from"] = SyntaxKind.FromKeyword,
        ["function"] = SyntaxKind.FunctionKeyword,
        ["get"] = SyntaxKind.GetKeyword,
        ["if"] = SyntaxKind.IfKeyword,
        ["implements"] = SyntaxKind.ImplementsKeyword,
        ["import"] = SyntaxKind.ImportKeyword,
        ["in"] = SyntaxKind.InKeyword,
        ["infer"] = SyntaxKind.InferKeyword,
        ["instanceof"] = SyntaxKind.InstanceOfKeyword,
        ["interface"] = SyntaxKind.InterfaceKeyword,
        ["intrinsic"] = SyntaxKind.IntrinsicKeyword,
        ["is"] = SyntaxKind.IsKeyword,
        ["keyof"] = SyntaxKind.KeyOfKeyword,
        ["let"] = SyntaxKind.LetKeyword,
        ["module"] = SyntaxKind.ModuleKeyword,
        ["namespace"] = SyntaxKind.NamespaceKeyword,
        ["never"] = SyntaxKind.NeverKeyword,
        ["new"] = SyntaxKind.NewKeyword,
        ["null"] = SyntaxKind.NullKeyword,
        ["number"] = SyntaxKind.NumberKeyword,
        ["object"] = SyntaxKind.ObjectKeyword,
        ["package"] = SyntaxKind.PackageKeyword,
        ["private"] = SyntaxKind.PrivateKeyword,
        ["protected"] = SyntaxKind.ProtectedKeyword,
        ["public"] = SyntaxKind.PublicKeyword,
        ["override"] = SyntaxKind.OverrideKeyword,
        ["out"] = SyntaxKind.OutKeyword,
        ["readonly"] = SyntaxKind.ReadonlyKeyword,
        ["require"] = SyntaxKind.RequireKeyword,
        ["global"] = SyntaxKind.GlobalKeyword,
        ["return"] = SyntaxKind.ReturnKeyword,
        ["satisfies"] = SyntaxKind.SatisfiesKeyword,
        ["set"] = SyntaxKind.SetKeyword,
        ["static"] = SyntaxKind.StaticKeyword,
        ["string"] = SyntaxKind.StringKeyword,
        ["super"] = SyntaxKind.SuperKeyword,
        ["switch"] = SyntaxKind.SwitchKeyword,
        ["symbol"] = SyntaxKind.SymbolKeyword,
        ["this"] = SyntaxKind.ThisKeyword,
        ["throw"] = SyntaxKind.ThrowKeyword,
        ["true"] = SyntaxKind.TrueKeyword,
        ["try"] = SyntaxKind.TryKeyword,
        ["type"] = SyntaxKind.TypeKeyword,
        ["typeof"] = SyntaxKind.TypeOfKeyword,
        ["undefined"] = SyntaxKind.UndefinedKeyword,
        ["unique"] = SyntaxKind.UniqueKeyword,
        ["unknown"] = SyntaxKind.UnknownKeyword,
        ["using"] = SyntaxKind.UsingKeyword,
        ["var"] = SyntaxKind.VarKeyword,
        ["void"] = SyntaxKind.VoidKeyword,
        ["while"] = SyntaxKind.WhileKeyword,
        ["with"] = SyntaxKind.WithKeyword,
        ["yield"] = SyntaxKind.YieldKeyword,
        ["async"] = SyntaxKind.AsyncKeyword,
        ["await"] = SyntaxKind.AwaitKeyword,
        ["of"] = SyntaxKind.OfKeyword
    };

    private static readonly Dictionary<string, SyntaxKind> textToToken = new(textToKeyword)
    {
        ["{"] = SyntaxKind.OpenBraceToken,
        ["}"] = SyntaxKind.CloseBraceToken,
        ["("] = SyntaxKind.OpenParenToken,
        [")"] = SyntaxKind.CloseParenToken,
        ["["] = SyntaxKind.OpenBracketToken,
        ["]"] = SyntaxKind.CloseBracketToken,
        ["."] = SyntaxKind.DotToken,
        ["..."] = SyntaxKind.DotDotDotToken,
        [";"] = SyntaxKind.SemicolonToken,
        [","] = SyntaxKind.CommaToken,
        ["<"] = SyntaxKind.LessThanToken,
        [">"] = SyntaxKind.GreaterThanToken,
        ["<="] = SyntaxKind.LessThanEqualsToken,
        [">="] = SyntaxKind.GreaterThanEqualsToken,
        ["=="] = SyntaxKind.EqualsEqualsToken,
        ["!="] = SyntaxKind.ExclamationEqualsToken,
        ["==="] = SyntaxKind.EqualsEqualsEqualsToken,
        ["!=="] = SyntaxKind.ExclamationEqualsEqualsToken,
        ["=>"] = SyntaxKind.EqualsGreaterThanToken,
        ["+"] = SyntaxKind.PlusToken,
        ["-"] = SyntaxKind.MinusToken,
        ["**"] = SyntaxKind.AsteriskAsteriskToken,
        ["*"] = SyntaxKind.AsteriskToken,
        ["/"] = SyntaxKind.SlashToken,
        ["%"] = SyntaxKind.PercentToken,
        ["++"] = SyntaxKind.PlusPlusToken,
        ["--"] = SyntaxKind.MinusMinusToken,
        ["<<"] = SyntaxKind.LessThanLessThanToken,
        ["</"] = SyntaxKind.LessThanSlashToken,
        [">>"] = SyntaxKind.GreaterThanGreaterThanToken,
        [">>>"] = SyntaxKind.GreaterThanGreaterThanGreaterThanToken,
        ["&"] = SyntaxKind.AmpersandToken,
        ["|"] = SyntaxKind.BarToken,
        ["^"] = SyntaxKind.CaretToken,
        ["!"] = SyntaxKind.ExclamationToken,
        ["~"] = SyntaxKind.TildeToken,
        ["&&"] = SyntaxKind.AmpersandAmpersandToken,
        ["||"] = SyntaxKind.BarBarToken,
        ["?"] = SyntaxKind.QuestionToken,
        ["??"] = SyntaxKind.QuestionQuestionToken,
        ["?."] = SyntaxKind.QuestionDotToken,
        [":"] = SyntaxKind.ColonToken,
        ["="] = SyntaxKind.EqualsToken,
        ["+="] = SyntaxKind.PlusEqualsToken,
        ["-="] = SyntaxKind.MinusEqualsToken,
        ["*="] = SyntaxKind.AsteriskEqualsToken,
        ["**="] = SyntaxKind.AsteriskAsteriskEqualsToken,
        ["/="] = SyntaxKind.SlashEqualsToken,
        ["%="] = SyntaxKind.PercentEqualsToken,
        ["<<="] = SyntaxKind.LessThanLessThanEqualsToken,
        [">>="] = SyntaxKind.GreaterThanGreaterThanEqualsToken,
        [">>>="] = SyntaxKind.GreaterThanGreaterThanGreaterThanEqualsToken,
        ["&="] = SyntaxKind.AmpersandEqualsToken,
        ["|="] = SyntaxKind.BarEqualsToken,
        ["^="] = SyntaxKind.CaretEqualsToken,
        ["||="] = SyntaxKind.BarBarEqualsToken,
        ["&&="] = SyntaxKind.AmpersandAmpersandEqualsToken,
        ["??="] = SyntaxKind.QuestionQuestionEqualsToken,
        ["@"] = SyntaxKind.AtToken,
        ["#"] = SyntaxKind.HashToken,
        ["`"] = SyntaxKind.BacktickToken
    };

    private static string[] MakeReverseMap(Dictionary<string, SyntaxKind> source)
    {
        int max = 0;
        foreach (var x in Enum.GetValues<SyntaxKind>())
            if ((int)x > max)
                max = (int)x;
        var result = new string[max + 1];
        foreach (var pair in source)
            result[(int)pair.Value] = pair.Key;
        return result;
    }

    private static readonly string[] tokenStrings = MakeReverseMap(textToToken);

    internal static bool TokenIsIdentifierOrKeyword(SyntaxKind token)
    {
        return token >= SyntaxKind.Identifier;
    }

    private static bool TokenIsIdentifierOrKeywordOrGreaterThan(SyntaxKind token)
    {
        return token == SyntaxKind.GreaterThanToken || TokenIsIdentifierOrKeyword(token);
    }

    internal static string TokenToString(SyntaxKind t)
    {
        return t >= 0 && (int)t <= tokenStrings.Length ? tokenStrings[(int)t] : null;
    }

    private static SyntaxKind? StringToToken(string s)
    {
        if (textToToken.TryGetValue(s, out var token))
            return token;

        return null;
    }

}