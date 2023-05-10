using Serenity.TypeScript.TsTypes;
#if ISSOURCEGENERATOR
#else
using CharSpan = System.ReadOnlySpan<char>;
#endif
using static Serenity.TypeScript.TsParser.Core;

namespace Serenity.TypeScript.TsParser;

public delegate void ErrorCallback(DiagnosticMessage message, int? length);

public class Scanner
{
    private string _text;
    // Current position (end position of text of current token)
    private int _pos;

    // end of text
    private int _end;

    // Start position of whitespace before current token
    private int _startPos;

    // Start position of text of current token
    private int _tokenPos;

    private SyntaxKind _token;
    private string _tokenValue;
    private bool _precedingLineBreak;
    private bool _tokenIsUnterminated;
    public event ErrorCallback OnError;
    private LanguageVariant _languageVariant;

    public int StartPos => _startPos;
    public int TextPos
    {
        get => _pos;
        set
        {
            Debug.Assert(value >= 0);
            _pos = value;
            _startPos = value;
            _tokenPos = value;
            _token = SyntaxKind.Unknown;
            _precedingLineBreak = false;
            _tokenValue = null;
            _tokenIsUnterminated = false;
        }
    }

    public int TokenPos => _tokenPos;
    public string TokenText => _text[_tokenPos.._pos];
    public string TokenValue => _tokenValue;
    public bool HasPrecedingLineBreak => _precedingLineBreak;
    public bool IsUnterminated => _tokenIsUnterminated;

    public Scanner(LanguageVariant languageVariant, string text, int start = 0, int? length = null)
    {
        _languageVariant = languageVariant;
        _startPos = start;
        SetText(text, start, length);
    }

    private static readonly Dictionary<string, SyntaxKind> textToToken = new()
    {
        { "abstract", SyntaxKind.AbstractKeyword },
        { "any", SyntaxKind.AnyKeyword },
        { "as", SyntaxKind.AsKeyword },
        { "boolean", SyntaxKind.BooleanKeyword },
        { "break", SyntaxKind.BreakKeyword },
        { "case", SyntaxKind.CaseKeyword },
        { "catch", SyntaxKind.CatchKeyword },
        { "class", SyntaxKind.ClassKeyword },
        { "continue", SyntaxKind.ContinueKeyword },
        { "const", SyntaxKind.ConstKeyword },
        { "constructor", SyntaxKind.ConstructorKeyword },
        { "debugger", SyntaxKind.DebuggerKeyword },
        { "declare", SyntaxKind.DeclareKeyword },
        { "default", SyntaxKind.DefaultKeyword },
        { "delete", SyntaxKind.DeleteKeyword },
        { "do", SyntaxKind.DoKeyword },
        { "else", SyntaxKind.ElseKeyword },
        { "enum", SyntaxKind.EnumKeyword },
        { "export", SyntaxKind.ExportKeyword },
        { "extends", SyntaxKind.ExtendsKeyword },
        { "false", SyntaxKind.FalseKeyword },
        { "finally", SyntaxKind.FinallyKeyword },
        { "for", SyntaxKind.ForKeyword },
        { "from", SyntaxKind.FromKeyword },
        { "function", SyntaxKind.FunctionKeyword },
        { "get", SyntaxKind.GetKeyword },
        { "if", SyntaxKind.IfKeyword },
        { "implements", SyntaxKind.ImplementsKeyword },
        { "import", SyntaxKind.ImportKeyword },
        { "in", SyntaxKind.InKeyword },
        { "instanceof", SyntaxKind.InstanceOfKeyword },
        { "interface", SyntaxKind.InterfaceKeyword },
        { "is", SyntaxKind.IsKeyword },
        { "keyof", SyntaxKind.KeyOfKeyword },
        { "let", SyntaxKind.LetKeyword },
        { "module", SyntaxKind.ModuleKeyword },
        { "namespace", SyntaxKind.NamespaceKeyword },
        { "never", SyntaxKind.NeverKeyword },
        { "new", SyntaxKind.NewKeyword },
        { "null", SyntaxKind.NullKeyword },
        { "number", SyntaxKind.NumberKeyword },
        { "object", SyntaxKind.ObjectKeyword },
        { "package", SyntaxKind.PackageKeyword },
        { "private", SyntaxKind.PrivateKeyword },
        { "protected", SyntaxKind.ProtectedKeyword },
        { "public", SyntaxKind.PublicKeyword },
        { "readonly", SyntaxKind.ReadonlyKeyword },
        { "require", SyntaxKind.RequireKeyword },
        { "global", SyntaxKind.GlobalKeyword },
        { "return", SyntaxKind.ReturnKeyword },
        { "set", SyntaxKind.SetKeyword },
        { "static", SyntaxKind.StaticKeyword },
        { "string", SyntaxKind.StringKeyword },
        { "super", SyntaxKind.SuperKeyword },
        { "switch", SyntaxKind.SwitchKeyword },
        { "symbol", SyntaxKind.SymbolKeyword },
        { "this", SyntaxKind.ThisKeyword },
        { "throw", SyntaxKind.ThrowKeyword },
        { "true", SyntaxKind.TrueKeyword },
        { "try", SyntaxKind.TryKeyword },
        { "type", SyntaxKind.TypeKeyword },
        { "typeof", SyntaxKind.TypeOfKeyword },
        { "undefined", SyntaxKind.UndefinedKeyword },
        { "var", SyntaxKind.VarKeyword },
        { "void", SyntaxKind.VoidKeyword },
        { "while", SyntaxKind.WhileKeyword },
        { "with", SyntaxKind.WithKeyword },
        { "yield", SyntaxKind.YieldKeyword },
        { "async", SyntaxKind.AsyncKeyword },
        { "await", SyntaxKind.AwaitKeyword },
        { "of", SyntaxKind.OfKeyword },
        { "{", SyntaxKind.OpenBraceToken },
        { "}", SyntaxKind.CloseBraceToken },
        { "(", SyntaxKind.OpenParenToken },
        { ")", SyntaxKind.CloseParenToken },
        { "[", SyntaxKind.OpenBracketToken },
        { "]", SyntaxKind.CloseBracketToken },
        { ".", SyntaxKind.DotToken },
        { "...", SyntaxKind.DotDotDotToken },
        { ";", SyntaxKind.SemicolonToken },
        { ",", SyntaxKind.CommaToken },
        { "<", SyntaxKind.LessThanToken },
        { ">", SyntaxKind.GreaterThanToken },
        { "<=", SyntaxKind.LessThanEqualsToken },
        { ">=", SyntaxKind.GreaterThanEqualsToken },
        { "==", SyntaxKind.EqualsEqualsToken },
        { "!=", SyntaxKind.ExclamationEqualsToken },
        { "===", SyntaxKind.EqualsEqualsEqualsToken },
        { "!==", SyntaxKind.ExclamationEqualsEqualsToken },
        { "=>", SyntaxKind.EqualsGreaterThanToken },
        { "+", SyntaxKind.PlusToken },
        { "-", SyntaxKind.MinusToken },
        { "**", SyntaxKind.AsteriskAsteriskToken },
        { "*", SyntaxKind.AsteriskToken },
        { "/", SyntaxKind.SlashToken },
        { "%", SyntaxKind.PercentToken },
        { "++", SyntaxKind.PlusPlusToken },
        { "--", SyntaxKind.MinusMinusToken },
        { "<<", SyntaxKind.LessThanLessThanToken },
        { "</", SyntaxKind.LessThanSlashToken },
        { ">>", SyntaxKind.GreaterThanGreaterThanToken },
        { ">>>", SyntaxKind.GreaterThanGreaterThanGreaterThanToken },
        { "&", SyntaxKind.AmpersandToken },
        { "|", SyntaxKind.BarToken },
        { "^", SyntaxKind.CaretToken },
        { "!", SyntaxKind.ExclamationToken },
        { "~", SyntaxKind.TildeToken },
        { "&&", SyntaxKind.AmpersandAmpersandToken },
        { "||", SyntaxKind.BarBarToken },
        { "?", SyntaxKind.QuestionToken },
        { ":", SyntaxKind.ColonToken },
        { "=", SyntaxKind.EqualsToken },
        { "+=", SyntaxKind.PlusEqualsToken },
        { "-=", SyntaxKind.MinusEqualsToken },
        { "*=", SyntaxKind.AsteriskEqualsToken },
        { "**=", SyntaxKind.AsteriskAsteriskEqualsToken },
        { "/=", SyntaxKind.SlashEqualsToken },
        { "%=", SyntaxKind.PercentEqualsToken },
        { "<<=", SyntaxKind.LessThanLessThanEqualsToken },
        { ">>=", SyntaxKind.GreaterThanGreaterThanEqualsToken },
        { ">>>=", SyntaxKind.GreaterThanGreaterThanGreaterThanEqualsToken },
        { "&=", SyntaxKind.AmpersandEqualsToken },
        { "|=", SyntaxKind.BarEqualsToken },
        { "^=", SyntaxKind.CaretEqualsToken },
        { "@", SyntaxKind.AtToken }
    };

    public static readonly int[] UnicodeEs5IdentifierStart = { 170, 170, 181, 181, 186, 186, 192, 214, 216, 246, 248, 705, 710, 721, 736, 740, 748, 748, 750, 750, 880, 884, 886, 887, 890, 893, 902, 902, 904, 906, 908, 908, 910, 929, 931, 1013, 1015, 1153, 1162, 1319, 1329, 1366, 1369, 1369, 1377, 1415, 1488, 1514, 1520, 1522, 1568, 1610, 1646, 1647, 1649, 1747, 1749, 1749, 1765, 1766, 1774, 1775, 1786, 1788, 1791, 1791, 1808, 1808, 1810, 1839, 1869, 1957, 1969, 1969, 1994, 2026, 2036, 2037, 2042, 2042, 2048, 2069, 2074, 2074, 2084, 2084, 2088, 2088, 2112, 2136, 2208, 2208, 2210, 2220, 2308, 2361, 2365, 2365, 2384, 2384, 2392, 2401, 2417, 2423, 2425, 2431, 2437, 2444, 2447, 2448, 2451, 2472, 2474, 2480, 2482, 2482, 2486, 2489, 2493, 2493, 2510, 2510, 2524, 2525, 2527, 2529, 2544, 2545, 2565, 2570, 2575, 2576, 2579, 2600, 2602, 2608, 2610, 2611, 2613, 2614, 2616, 2617, 2649, 2652, 2654, 2654, 2674, 2676, 2693, 2701, 2703, 2705, 2707, 2728, 2730, 2736, 2738, 2739, 2741, 2745, 2749, 2749, 2768, 2768, 2784, 2785, 2821, 2828, 2831, 2832, 2835, 2856, 2858, 2864, 2866, 2867, 2869, 2873, 2877, 2877, 2908, 2909, 2911, 2913, 2929, 2929, 2947, 2947, 2949, 2954, 2958, 2960, 2962, 2965, 2969, 2970, 2972, 2972, 2974, 2975, 2979, 2980, 2984, 2986, 2990, 3001, 3024, 3024, 3077, 3084, 3086, 3088, 3090, 3112, 3114, 3123, 3125, 3129, 3133, 3133, 3160, 3161, 3168, 3169, 3205, 3212, 3214, 3216, 3218, 3240, 3242, 3251, 3253, 3257, 3261, 3261, 3294, 3294, 3296, 3297, 3313, 3314, 3333, 3340, 3342, 3344, 3346, 3386, 3389, 3389, 3406, 3406, 3424, 3425, 3450, 3455, 3461, 3478, 3482, 3505, 3507, 3515, 3517, 3517, 3520, 3526, 3585, 3632, 3634, 3635, 3648, 3654, 3713, 3714, 3716, 3716, 3719, 3720, 3722, 3722, 3725, 3725, 3732, 3735, 3737, 3743, 3745, 3747, 3749, 3749, 3751, 3751, 3754, 3755, 3757, 3760, 3762, 3763, 3773, 3773, 3776, 3780, 3782, 3782, 3804, 3807, 3840, 3840, 3904, 3911, 3913, 3948, 3976, 3980, 4096, 4138, 4159, 4159, 4176, 4181, 4186, 4189, 4193, 4193, 4197, 4198, 4206, 4208, 4213, 4225, 4238, 4238, 4256, 4293, 4295, 4295, 4301, 4301, 4304, 4346, 4348, 4680, 4682, 4685, 4688, 4694, 4696, 4696, 4698, 4701, 4704, 4744, 4746, 4749, 4752, 4784, 4786, 4789, 4792, 4798, 4800, 4800, 4802, 4805, 4808, 4822, 4824, 4880, 4882, 4885, 4888, 4954, 4992, 5007, 5024, 5108, 5121, 5740, 5743, 5759, 5761, 5786, 5792, 5866, 5870, 5872, 5888, 5900, 5902, 5905, 5920, 5937, 5952, 5969, 5984, 5996, 5998, 6000, 6016, 6067, 6103, 6103, 6108, 6108, 6176, 6263, 6272, 6312, 6314, 6314, 6320, 6389, 6400, 6428, 6480, 6509, 6512, 6516, 6528, 6571, 6593, 6599, 6656, 6678, 6688, 6740, 6823, 6823, 6917, 6963, 6981, 6987, 7043, 7072, 7086, 7087, 7098, 7141, 7168, 7203, 7245, 7247, 7258, 7293, 7401, 7404, 7406, 7409, 7413, 7414, 7424, 7615, 7680, 7957, 7960, 7965, 7968, 8005, 8008, 8013, 8016, 8023, 8025, 8025, 8027, 8027, 8029, 8029, 8031, 8061, 8064, 8116, 8118, 8124, 8126, 8126, 8130, 8132, 8134, 8140, 8144, 8147, 8150, 8155, 8160, 8172, 8178, 8180, 8182, 8188, 8305, 8305, 8319, 8319, 8336, 8348, 8450, 8450, 8455, 8455, 8458, 8467, 8469, 8469, 8473, 8477, 8484, 8484, 8486, 8486, 8488, 8488, 8490, 8493, 8495, 8505, 8508, 8511, 8517, 8521, 8526, 8526, 8544, 8584, 11264, 11310, 11312, 11358, 11360, 11492, 11499, 11502, 11506, 11507, 11520, 11557, 11559, 11559, 11565, 11565, 11568, 11623, 11631, 11631, 11648, 11670, 11680, 11686, 11688, 11694, 11696, 11702, 11704, 11710, 11712, 11718, 11720, 11726, 11728, 11734, 11736, 11742, 11823, 11823, 12293, 12295, 12321, 12329, 12337, 12341, 12344, 12348, 12353, 12438, 12445, 12447, 12449, 12538, 12540, 12543, 12549, 12589, 12593, 12686, 12704, 12730, 12784, 12799, 13312, 19893, 19968, 40908, 40960, 42124, 42192, 42237, 42240, 42508, 42512, 42527, 42538, 42539, 42560, 42606, 42623, 42647, 42656, 42735, 42775, 42783, 42786, 42888, 42891, 42894, 42896, 42899, 42912, 42922, 43000, 43009, 43011, 43013, 43015, 43018, 43020, 43042, 43072, 43123, 43138, 43187, 43250, 43255, 43259, 43259, 43274, 43301, 43312, 43334, 43360, 43388, 43396, 43442, 43471, 43471, 43520, 43560, 43584, 43586, 43588, 43595, 43616, 43638, 43642, 43642, 43648, 43695, 43697, 43697, 43701, 43702, 43705, 43709, 43712, 43712, 43714, 43714, 43739, 43741, 43744, 43754, 43762, 43764, 43777, 43782, 43785, 43790, 43793, 43798, 43808, 43814, 43816, 43822, 43968, 44002, 44032, 55203, 55216, 55238, 55243, 55291, 63744, 64109, 64112, 64217, 64256, 64262, 64275, 64279, 64285, 64285, 64287, 64296, 64298, 64310, 64312, 64316, 64318, 64318, 64320, 64321, 64323, 64324, 64326, 64433, 64467, 64829, 64848, 64911, 64914, 64967, 65008, 65019, 65136, 65140, 65142, 65276, 65313, 65338, 65345, 65370, 65382, 65470, 65474, 65479, 65482, 65487, 65490, 65495, 65498, 65500, };
    public static readonly int[] UnicodeEs5IdentifierPart = { 170, 170, 181, 181, 186, 186, 192, 214, 216, 246, 248, 705, 710, 721, 736, 740, 748, 748, 750, 750, 768, 884, 886, 887, 890, 893, 902, 902, 904, 906, 908, 908, 910, 929, 931, 1013, 1015, 1153, 1155, 1159, 1162, 1319, 1329, 1366, 1369, 1369, 1377, 1415, 1425, 1469, 1471, 1471, 1473, 1474, 1476, 1477, 1479, 1479, 1488, 1514, 1520, 1522, 1552, 1562, 1568, 1641, 1646, 1747, 1749, 1756, 1759, 1768, 1770, 1788, 1791, 1791, 1808, 1866, 1869, 1969, 1984, 2037, 2042, 2042, 2048, 2093, 2112, 2139, 2208, 2208, 2210, 2220, 2276, 2302, 2304, 2403, 2406, 2415, 2417, 2423, 2425, 2431, 2433, 2435, 2437, 2444, 2447, 2448, 2451, 2472, 2474, 2480, 2482, 2482, 2486, 2489, 2492, 2500, 2503, 2504, 2507, 2510, 2519, 2519, 2524, 2525, 2527, 2531, 2534, 2545, 2561, 2563, 2565, 2570, 2575, 2576, 2579, 2600, 2602, 2608, 2610, 2611, 2613, 2614, 2616, 2617, 2620, 2620, 2622, 2626, 2631, 2632, 2635, 2637, 2641, 2641, 2649, 2652, 2654, 2654, 2662, 2677, 2689, 2691, 2693, 2701, 2703, 2705, 2707, 2728, 2730, 2736, 2738, 2739, 2741, 2745, 2748, 2757, 2759, 2761, 2763, 2765, 2768, 2768, 2784, 2787, 2790, 2799, 2817, 2819, 2821, 2828, 2831, 2832, 2835, 2856, 2858, 2864, 2866, 2867, 2869, 2873, 2876, 2884, 2887, 2888, 2891, 2893, 2902, 2903, 2908, 2909, 2911, 2915, 2918, 2927, 2929, 2929, 2946, 2947, 2949, 2954, 2958, 2960, 2962, 2965, 2969, 2970, 2972, 2972, 2974, 2975, 2979, 2980, 2984, 2986, 2990, 3001, 3006, 3010, 3014, 3016, 3018, 3021, 3024, 3024, 3031, 3031, 3046, 3055, 3073, 3075, 3077, 3084, 3086, 3088, 3090, 3112, 3114, 3123, 3125, 3129, 3133, 3140, 3142, 3144, 3146, 3149, 3157, 3158, 3160, 3161, 3168, 3171, 3174, 3183, 3202, 3203, 3205, 3212, 3214, 3216, 3218, 3240, 3242, 3251, 3253, 3257, 3260, 3268, 3270, 3272, 3274, 3277, 3285, 3286, 3294, 3294, 3296, 3299, 3302, 3311, 3313, 3314, 3330, 3331, 3333, 3340, 3342, 3344, 3346, 3386, 3389, 3396, 3398, 3400, 3402, 3406, 3415, 3415, 3424, 3427, 3430, 3439, 3450, 3455, 3458, 3459, 3461, 3478, 3482, 3505, 3507, 3515, 3517, 3517, 3520, 3526, 3530, 3530, 3535, 3540, 3542, 3542, 3544, 3551, 3570, 3571, 3585, 3642, 3648, 3662, 3664, 3673, 3713, 3714, 3716, 3716, 3719, 3720, 3722, 3722, 3725, 3725, 3732, 3735, 3737, 3743, 3745, 3747, 3749, 3749, 3751, 3751, 3754, 3755, 3757, 3769, 3771, 3773, 3776, 3780, 3782, 3782, 3784, 3789, 3792, 3801, 3804, 3807, 3840, 3840, 3864, 3865, 3872, 3881, 3893, 3893, 3895, 3895, 3897, 3897, 3902, 3911, 3913, 3948, 3953, 3972, 3974, 3991, 3993, 4028, 4038, 4038, 4096, 4169, 4176, 4253, 4256, 4293, 4295, 4295, 4301, 4301, 4304, 4346, 4348, 4680, 4682, 4685, 4688, 4694, 4696, 4696, 4698, 4701, 4704, 4744, 4746, 4749, 4752, 4784, 4786, 4789, 4792, 4798, 4800, 4800, 4802, 4805, 4808, 4822, 4824, 4880, 4882, 4885, 4888, 4954, 4957, 4959, 4992, 5007, 5024, 5108, 5121, 5740, 5743, 5759, 5761, 5786, 5792, 5866, 5870, 5872, 5888, 5900, 5902, 5908, 5920, 5940, 5952, 5971, 5984, 5996, 5998, 6000, 6002, 6003, 6016, 6099, 6103, 6103, 6108, 6109, 6112, 6121, 6155, 6157, 6160, 6169, 6176, 6263, 6272, 6314, 6320, 6389, 6400, 6428, 6432, 6443, 6448, 6459, 6470, 6509, 6512, 6516, 6528, 6571, 6576, 6601, 6608, 6617, 6656, 6683, 6688, 6750, 6752, 6780, 6783, 6793, 6800, 6809, 6823, 6823, 6912, 6987, 6992, 7001, 7019, 7027, 7040, 7155, 7168, 7223, 7232, 7241, 7245, 7293, 7376, 7378, 7380, 7414, 7424, 7654, 7676, 7957, 7960, 7965, 7968, 8005, 8008, 8013, 8016, 8023, 8025, 8025, 8027, 8027, 8029, 8029, 8031, 8061, 8064, 8116, 8118, 8124, 8126, 8126, 8130, 8132, 8134, 8140, 8144, 8147, 8150, 8155, 8160, 8172, 8178, 8180, 8182, 8188, 8204, 8205, 8255, 8256, 8276, 8276, 8305, 8305, 8319, 8319, 8336, 8348, 8400, 8412, 8417, 8417, 8421, 8432, 8450, 8450, 8455, 8455, 8458, 8467, 8469, 8469, 8473, 8477, 8484, 8484, 8486, 8486, 8488, 8488, 8490, 8493, 8495, 8505, 8508, 8511, 8517, 8521, 8526, 8526, 8544, 8584, 11264, 11310, 11312, 11358, 11360, 11492, 11499, 11507, 11520, 11557, 11559, 11559, 11565, 11565, 11568, 11623, 11631, 11631, 11647, 11670, 11680, 11686, 11688, 11694, 11696, 11702, 11704, 11710, 11712, 11718, 11720, 11726, 11728, 11734, 11736, 11742, 11744, 11775, 11823, 11823, 12293, 12295, 12321, 12335, 12337, 12341, 12344, 12348, 12353, 12438, 12441, 12442, 12445, 12447, 12449, 12538, 12540, 12543, 12549, 12589, 12593, 12686, 12704, 12730, 12784, 12799, 13312, 19893, 19968, 40908, 40960, 42124, 42192, 42237, 42240, 42508, 42512, 42539, 42560, 42607, 42612, 42621, 42623, 42647, 42655, 42737, 42775, 42783, 42786, 42888, 42891, 42894, 42896, 42899, 42912, 42922, 43000, 43047, 43072, 43123, 43136, 43204, 43216, 43225, 43232, 43255, 43259, 43259, 43264, 43309, 43312, 43347, 43360, 43388, 43392, 43456, 43471, 43481, 43520, 43574, 43584, 43597, 43600, 43609, 43616, 43638, 43642, 43643, 43648, 43714, 43739, 43741, 43744, 43759, 43762, 43766, 43777, 43782, 43785, 43790, 43793, 43798, 43808, 43814, 43816, 43822, 43968, 44010, 44012, 44013, 44016, 44025, 44032, 55203, 55216, 55238, 55243, 55291, 63744, 64109, 64112, 64217, 64256, 64262, 64275, 64279, 64285, 64296, 64298, 64310, 64312, 64316, 64318, 64318, 64320, 64321, 64323, 64324, 64326, 64433, 64467, 64829, 64848, 64911, 64914, 64967, 65008, 65019, 65024, 65039, 65056, 65062, 65075, 65076, 65101, 65103, 65136, 65140, 65142, 65276, 65296, 65305, 65313, 65338, 65343, 65343, 65345, 65370, 65382, 65470, 65474, 65479, 65482, 65487, 65490, 65495, 65498, 65500, };
    private static readonly int _mergeConflictMarkerLength = "<<<<<<<".Length;
    private static readonly Regex _shebangTriviaRegex = new("/^#!.*/");

    public static bool TokenIsIdentifierOrKeyword(SyntaxKind token)
    {
        return token >= SyntaxKind.Identifier;
    }

    public static bool LookupInUnicodeMap(int code, int[] map)
    {
        if (code < map[0])
        {
            return false;
        }
        var lo = 0;
        int hi = map.Length;
        int mid;
        while (lo + 1 < hi)
        {
            mid = lo + (hi - lo) / 2;
            // mid has to be even to catch a range's beginning
            mid -= mid % 2;
            if (map[mid] <= code && code <= map[mid + 1])
            {
                return true;
            }
            if (code < map[mid])
            {
                hi = mid;
            }
            else
            {
                lo = mid + 2;
            }
        }
        return false;
    }


    public static bool IsUnicodeIdentifierStart(int code)
    {
        return LookupInUnicodeMap(code, UnicodeEs5IdentifierStart);
    }


    public static bool IsUnicodeIdentifierPart(int code)
    {
        return LookupInUnicodeMap(code, UnicodeEs5IdentifierPart);
    }

    public static string TokenToString(SyntaxKind t)
    {
        return textToToken.FirstOrDefault(v => v.Value == t).Key;
    }

    public SyntaxKind StringToToken(string s)
    {
        return textToToken[s];
    }

    public static List<int> ComputeLineStarts(string text)
    {
        List<int> result = new();
        var pos = 0;
        var lineStart = 0;
        while (pos < text.Length)
        {
            var ch = text[pos];
            pos++;
            switch (ch)
            {
                case '\r':
                    if (text[pos] == '\n')
                    {
                        pos++;
                    }
                    goto caseLabel2;
                case '\n':
                    caseLabel2: result.Add(lineStart);
                    lineStart = pos;
                    break;
                default:
                    if (ch > (int)CharacterCodes.MaxAsciiCharacter && IsLineBreak(ch))
                    {
                        result.Add(lineStart);
                        lineStart = pos;
                    }
                    break;
            }
        }
        result.Add(lineStart);
        return result;
    }


    public int GetPositionOfLineAndCharacter(SourceFile sourceFile, int line, int character)
    {
        return ComputePositionOfLineAndCharacter(GetLineStarts(sourceFile), line, character);
    }

    public static int ComputePositionOfLineAndCharacter(int[] lineStarts, int line, int character)
    {
        Debug.Assert(line >= 0 && line < lineStarts.Length);
        return lineStarts[line] + character;
    }

    public static int[] GetLineStarts(ISourceFileLike sourceFile)
    {
        return sourceFile.LineMap ??= ComputeLineStarts(sourceFile.Text).ToArray();
    }

    public static LineAndCharacter ComputeLineAndCharacterOfPosition(int[] lineStarts, int position)
    {
        var lineNumber = BinarySearch(lineStarts, position);
        if (lineNumber < 0)
        {
            // If the actual position was not found,
            // the binary search returns the 2's-complement of the next line start
            // e.g. if the line starts at [5, 10, 23, 80] and the position requested was 20
            // then the search will return -2.
            //
            // We want the index of the previous line start, so we subtract 1.
            // Review 2's-complement if this is confusing.
            lineNumber = ~lineNumber - 1;
            Debug.Assert(lineNumber != -1, "position cannot precede the beginning of the file");
        }
        return new LineAndCharacter
        {
            Line = lineNumber,
            Character = position - lineStarts[lineNumber]
        };
    }

    public static LineAndCharacter GetLineAndCharacterOfPosition(SourceFile sourceFile, int position)
    {
        return ComputeLineAndCharacterOfPosition(GetLineStarts(sourceFile), position);
    }

    public static bool IsWhiteSpace(int ch)
    {
        return IsWhiteSpaceSingleLine(ch) || IsLineBreak(ch);
    }

    public static bool IsWhiteSpaceSingleLine(int ch)
    {
        // Note: nextLine is in the Zs space, and should be considered to be a whitespace.
        // It is explicitly not a line-break as it isn't in the exact set specified by EcmaScript.
        return ch == ' ' ||
            ch == '\t' ||
            ch == '\v' ||
            ch == '\f' ||
            ch == (int)CharacterCodes.NonBreakingSpace ||
            ch == (int)CharacterCodes.NextLine ||
            ch == (int)CharacterCodes.Ogham ||
            ch >= (int)CharacterCodes.EnQuad && ch <= (int)CharacterCodes.ZeroWidthSpace ||
            ch == (int)CharacterCodes.NarrowNoBreakSpace ||
            ch == (int)CharacterCodes.MathematicalSpace ||
            ch == (int)CharacterCodes.IdeographicSpace ||
            ch == (int)CharacterCodes.ByteOrderMark;
    }

    public static bool IsLineBreak(int ch)
    {
        // ES5 7.3:
        // The ECMAScript line terminator characters are listed in Table 3.
        //     Table 3: Line Terminator Characters
        //     Code Unit Value     Name                    Formal Name
        //     \u000A              Line Feed               <LF>
        //     \u000D              Carriage Return         <CR>
        //     \u2028              Line separator          <LS>
        //     \u2029              Paragraph separator     <PS>
        // Only the characters in Table 3 are treated as line terminators. Other new line or line
        // breaking characters are treated as white space but not as line terminators.

        return ch == '\n' ||
            ch == '\r' ||
            ch == (int)CharacterCodes.LineSeparator ||
            ch == (int)CharacterCodes.ParagraphSeparator;
    }

    public static bool IsDigit(int ch)
    {
        return ch >= '0' && ch <= '9';
    }

    public static bool IsOctalDigit(int ch)
    {
        return ch >= '0' && ch <= '7';
    }

    public static int SkipTriviaM(string text, int pos, bool stopAfterLineBreak = false, bool stopAtComments = false)
    {
        if (PositionIsSynthesized(pos))
        {
            return pos;
        }
        while (true)
        {
            if (pos >= text.Length) return pos;
            var ch = text[pos];
            switch (ch)
            {
                case '\r':
                    if (pos + 1 >= text.Length) return pos;
                    if (text[pos + 1] == '\n')
                    {
                        pos++;
                    }
                    goto caseLabel2;
                case '\n':
                    caseLabel2: pos++;
                    if (stopAfterLineBreak)
                    {
                        return pos;
                    }
                    continue;
                case '\t':
                case '\v':
                case '\f':
                case ' ':
                    pos++;
                    continue;
                case '/':
                    if (stopAtComments)
                    {
                        break;
                    }
                    if (pos + 1 >= text.Length) return pos;
                    if (text[pos + 1] == '/')
                    {
                        pos += 2;
                        while (pos < text.Length)
                        {
                            if (IsLineBreak(text[pos]))
                            {
                                break;
                            }
                            pos++;
                        }
                        continue;
                    }
                    if (pos + 1 >= text.Length) return pos;
                    if (text[pos + 1] == '*')
                    {
                        pos += 2;
                        while (pos < text.Length)
                        {
                            if (pos + 1 >= text.Length) return pos;
                            if (text[pos] == '*' && text[pos + 1] == '/')
                            {
                                pos += 2;
                                break;
                            }
                            pos++;
                        }
                        continue;
                    }
                    break;
                case '<':
                case '=':
                case '>':
                    if (IsConflictMarkerTrivia(text, pos))
                    {
                        pos = ScanConflictMarkerTrivia(text, pos);
                        continue;
                    }
                    break;
                case '#':
                    if (pos == 0 && IsShebangTrivia(text, pos))
                    {
                        pos = ScanShebangTrivia(text, pos);
                        continue;
                    }
                    break;
                default:
                    if (ch > (int)CharacterCodes.MaxAsciiCharacter && (IsWhiteSpace(ch)))
                    {
                        pos++;
                        continue;
                    }
                    break;
            }
            return pos;
        }
    }

    public static bool IsConflictMarkerTrivia(string text, int pos)
    {
        Debug.Assert(pos >= 0);
        if (pos == 0 || IsLineBreak(text[pos - 1]))
        {
            var ch = text[pos];
            if ((pos + _mergeConflictMarkerLength) < text.Length)
            {
                for (var i = 0; i < _mergeConflictMarkerLength; i++)
                {
                    if (text[pos + i] != ch)
                    {
                        return false;
                    }
                };
                return ch == '=' ||
                                    text[pos + _mergeConflictMarkerLength] == ' ';
            }
        }
        return false;
    }

    public static int ScanConflictMarkerTrivia(string text, int pos, Action<DiagnosticMessage, int> error = null)
    {
        error?.Invoke(Diagnostics.Merge_conflict_marker_encountered, _mergeConflictMarkerLength);
        var ch = text[pos];
        var len = text.Length;
        if (ch == '<' || ch == '>')
        {
            while (pos < len && !IsLineBreak(text[pos]))
            {
                pos++;
            }
        }
        else
        {
            ////Debug.assert(ch ==  '=');
            while (pos < len)
            {
                var ch2 = text[pos];
                if (ch2 == '>' && IsConflictMarkerTrivia(text, pos))
                {
                    break;
                }
                pos++;
            }
        }
        return pos;
    }
    public static bool IsShebangTrivia(string text, int pos)
    {
        // Shebangs check must only be done at the start of the file
        Debug.Assert(pos == 0);
        return _shebangTriviaRegex.Test(text);
    }

    public static int ScanShebangTrivia(string text, int pos)
    {
        var shebang = _shebangTriviaRegex.Exec(text)[0];
        pos = pos + shebang.Length;
        return pos;
    }

    public static U IterateCommentRanges<T, U>(bool reduce, string text, int pos, bool trailing, Func<(int pos, int end, SyntaxKind kind, bool hasTrailingNewLine, T state, U memo), U> cb, T state, U initial = default(U))
    {
        int pendingPos = 0;
        int pendingEnd = 0;
        var pendingKind = SyntaxKind.Unknown;
        bool pendingHasTrailingNewLine = false;
        var hasPendingCommentRange = false;
        var collecting = trailing || pos == 0;
        var accumulator = initial;
        while (pos >= 0 && pos < text.Length)
        {
            var ch = text[pos];
            switch (ch)
            {
                case '\r':
                    if (text[pos + 1] == '\n')
                    {
                        pos++;
                    }
                    goto caseLabel2;
                case '\n':
                    caseLabel2: pos++;
                    if (trailing)
                    {
                        goto breakScan;
                    }
                    collecting = true;
                    if (hasPendingCommentRange)
                    {
                        pendingHasTrailingNewLine = true;
                    }
                    continue;
                case '\t':
                case '\v':
                case '\f':
                case ' ':
                    pos++;
                    continue;
                case '/':
                    var nextChar = text[pos + 1];
                    var hasTrailingNewLine = false;
                    if (nextChar == '/' || nextChar == '*')
                    {
                        var kind = nextChar == '/' ? SyntaxKind.SingleLineCommentTrivia : SyntaxKind.MultiLineCommentTrivia;
                        var startPos = pos;
                        pos += 2;
                        if (nextChar == '/')
                        {
                            while (pos < text.Length)
                            {
                                if (IsLineBreak(text[pos]))
                                {
                                    hasTrailingNewLine = true;
                                    break;
                                }
                                pos++;
                            }
                        }
                        else
                        {
                            while (pos < text.Length)
                            {
                                if (text[pos] == '*' && text[pos + 1] == '/')
                                {
                                    pos += 2;
                                    break;
                                }
                                pos++;
                            }
                        }
                        if (collecting)
                        {
                            if (hasPendingCommentRange)
                            {
                                accumulator = cb((pendingPos, pendingEnd, pendingKind, pendingHasTrailingNewLine, state, accumulator));
                                if (!reduce && accumulator != null)
                                {
                                    // If we are not reducing and we have a truthy result, return it.
                                    return accumulator;
                                }
                                hasPendingCommentRange = false;
                            }
                            pendingPos = startPos;
                            pendingEnd = pos;
                            pendingKind = kind;
                            pendingHasTrailingNewLine = hasTrailingNewLine;
                            hasPendingCommentRange = true;
                        }
                        continue;
                    }
                    goto breakScan;
                default:
                    if (ch > (int)CharacterCodes.MaxAsciiCharacter && (IsWhiteSpace(ch)))
                    {
                        if (hasPendingCommentRange && IsLineBreak(ch))
                        {
                            pendingHasTrailingNewLine = true;
                        }
                        pos++;
                        continue;
                    }
                    goto breakScan;
            }
        }
        breakScan:
        if (hasPendingCommentRange)
        {
            accumulator = cb((pendingPos, pendingEnd, pendingKind, pendingHasTrailingNewLine, state, accumulator));
        }
        return accumulator;
    }

    public static U ReduceEachLeadingCommentRange<T, U>(string text, int pos, Func<(int pos, int end, SyntaxKind kind, bool hasTrailingNewLine, T state, U memo), U> cb, T state, U initial)
    {
        return IterateCommentRanges(/*reduce*/ true, text, pos, /*trailing*/ false, cb, state, initial);
    }

    public static U ReduceEachTrailingCommentRange<T, U>(string text, int pos, Func<(int pos, int end, SyntaxKind kind, bool hasTrailingNewLine, T state, U memo), U> cb, T state, U initial)
    {
        return IterateCommentRanges(/*reduce*/ true, text, pos, /*trailing*/ true, cb, state, initial);
    }

    public static List<CommentRange> AppendCommentRange((int pos, int end, SyntaxKind kind, bool hasTrailingNewLine, object _state, List<CommentRange> comments) cb)
    {
        cb.comments ??= new List<CommentRange>();
        cb.comments.Add(new CommentRange { Kind = cb.kind, Pos = cb.pos, End = cb.end, HasTrailingNewLine = cb.hasTrailingNewLine });
        return cb.comments;
    }

    public static List<CommentRange> GetLeadingCommentRanges(string text, int pos)
    {
        return ReduceEachLeadingCommentRange<object, List<CommentRange>>(text, pos, AppendCommentRange, null, null) ?? new List<CommentRange>();
    }


    public static List<CommentRange> GetTrailingCommentRanges(string text, int pos)
    {
        return ReduceEachTrailingCommentRange<object, List<CommentRange>>(text, pos, AppendCommentRange, null, null) ?? new List<CommentRange>();
    }


    public bool IsIdentifierStart(int ch)
    {
        return ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z' ||
                    ch == '$' || ch == '_' ||
                    ch > (int)CharacterCodes.MaxAsciiCharacter && IsUnicodeIdentifierStart(ch);
    }


    public static bool IsIdentifierPart(int ch)
    {
        return ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z' ||
                    ch >= '0' && ch <= '9' || ch == '$' || ch == '_' ||
                    ch > (int)CharacterCodes.MaxAsciiCharacter && IsUnicodeIdentifierPart(ch);
    }

    public void Error(DiagnosticMessage message, int length = 0)
    {
        OnError?.Invoke(message, length);
    }

    public string ScanNumber()
    {
        var start = _pos;
        while (IsDigit(_text[_pos]))
        {
            _pos++;
        }
        if (_text[_pos] == '.')
        {
            _pos++;
            while (IsDigit(_text[_pos]))
            {
                _pos++;
            }
        }
        var end = _pos;
        if (_text[_pos] == 'E' || _text[_pos] == 'e')
        {
            _pos++;
            if (_text[_pos] == '+' || _text[_pos] == '-')
            {
                _pos++;
            }
            if (IsDigit(_text[_pos]))
            {
                _pos++;
                while (IsDigit(_text[_pos]))
                {
                    _pos++;
                }
                end = _pos;
            }
            else
            {
                Error(Diagnostics.Digit_expected);
            }
        }
        return _text[start..end];
    }

    public int ScanOctalDigits()
    {
        var start = _pos;
        while (IsOctalDigit(_text[_pos]))
        {
            _pos++;
        }
#if ISSOURCEGENERATOR
        return int.Parse(_text[start.._pos]);
#else
        return int.Parse(_text.AsSpan(start, _pos - start));
#endif
    }

    public int ScanExactNumberOfHexDigits(int count)
    {
        return ScanHexDigits(/*minCount*/ count, /*scanAsManyAsPossible*/ false);
    }

    public int ScanMinimumNumberOfHexDigits(int count)
    {
        return ScanHexDigits(/*minCount*/ count, /*scanAsManyAsPossible*/ true);
    }

    public int ScanHexDigits(int minCount, bool scanAsManyAsPossible)
    {
        var digits = 0;
        var value = 0;
        while (digits < minCount || scanAsManyAsPossible)
        {
            var ch = _text[_pos];
            if (ch >= '0' && ch <= '9')
            {
                value = value * 16 + ch - '0';
            }
            else
        if (ch >= 'A' && ch <= 'f')
            {
                value = value * 16 + ch - 'A' + 10;
            }
            else
        if (ch >= 'a' && ch <= 'f')
            {
                value = value * 16 + ch - 'a' + 10;
            }
            else
            {
                break;
            }
            _pos++;
            digits++;
        }
        if (digits < minCount)
        {
            value = -1;
        }
        return value;
    }

    public string ScanString(bool allowEscapes = true)
    {
        var quote = _text[_pos];
        _pos++;
        var result = new StringBuilder();
        var start = _pos;
        while (true)
        {
            if (_pos >= _end)
            {
                result.Append(_text, start, _pos - start);
                _tokenIsUnterminated = true;
                Error(Diagnostics.Unterminated_string_literal);
                break;
            }
            var ch = _text[_pos];
            if (ch == quote)
            {
                result.Append(_text, start, _pos - start);
                _pos++;
                break;
            }
            if (ch == '\\' && allowEscapes)
            {
                result.Append(_text, start, _pos - start);
                ScanEscapeSequence(result);
                start = _pos;
                continue;
            }
            if (IsLineBreak(ch))
            {
                result.Append(_text, start, _pos - start);
                _tokenIsUnterminated = true;
                Error(Diagnostics.Unterminated_string_literal);
                break;
            }
            _pos++;
        }
        return result.ToString();
    }

    public SyntaxKind ScanTemplateAndSetTokenValue()
    {
        var startedWithBacktick = _text[_pos] == '`';
        _pos++;
        var start = _pos;
        var contents = new StringBuilder();
        SyntaxKind resultingToken;
        while (true)
        {
            if (_pos >= _end)
            {
                contents.Append(_text, start, _pos - start);
                _tokenIsUnterminated = true;
                Error(Diagnostics.Unterminated_template_literal);
                resultingToken = startedWithBacktick ? SyntaxKind.NoSubstitutionTemplateLiteral : SyntaxKind.TemplateTail;
                break;
            }
            var currChar = _text[_pos];
            if (currChar == '`')
            {
                contents.Append(_text, start, _pos - start);
                _pos++;
                resultingToken = startedWithBacktick ? SyntaxKind.NoSubstitutionTemplateLiteral : SyntaxKind.TemplateTail;
                break;
            }
            if (currChar == '$' && _pos + 1 < _end && _text[_pos + 1] == '{')
            {
                contents.Append(_text, start, _pos - start);
                _pos += 2;
                resultingToken = startedWithBacktick ? SyntaxKind.TemplateHead : SyntaxKind.TemplateMiddle;
                break;
            }
            if (currChar == '\\')
            {
                contents.Append(_text, start, _pos - start);
                ScanEscapeSequence(contents);
                start = _pos;
                continue;
            }
            if (currChar == '\r')
            {
                contents.Append(_text, start, _pos - start);
                _pos++;
                if (_pos < _end && _text[_pos] == '\n')
                {
                    _pos++;
                }
                contents.Append('\n');
                start = _pos;
                continue;
            }
            _pos++;
        }
        //Debug.assert(resultingToken != null);
        _tokenValue = contents.ToString();
        return resultingToken;
    }

    public void ScanEscapeSequence(StringBuilder sb)
    {
        _pos++;
        if (_pos >= _end)
        {
            Error(Diagnostics.Unexpected_end_of_text);
        }
        var ch = _text[_pos];
        _pos++;
        switch (ch)
        {
            case '0':
                sb.Append('\0');
                return;
            case 'b':
                sb.Append('\b');
                return;
            case 't':
                sb.Append('\t');
                return;
            case 'n':
                sb.Append('\n');
                return;
            case 'v':
                sb.Append('\v');
                return;
            case 'f':
                sb.Append('\f');
                return;
            case 'r':
                sb.Append('\r');
                return;
            case '\'':
                sb.Append('\'');
                return;
            case '"':
                sb.Append('"');
                return;
            case 'u':
                if (_pos < _end && _text[_pos] == '{')
                {
                    _pos++;
                    ScanExtendedUnicodeEscape(sb);
                    return;
                }
                // '\uDDDD'
                ScanHexadecimalEscape(sb, /*numDigits*/ 4);
                return;
            case 'x':
                // '\xDD'
                ScanHexadecimalEscape(sb, /*numDigits*/ 2);
                return;
            case '\r':
                if (_pos < _end && _text[_pos] == '\n')
                {
                    _pos++;
                }
                return;
            case '\n':
            case (char)CharacterCodes.LineSeparator:
            case (char)CharacterCodes.ParagraphSeparator:
                return;
            default:
                sb.Append(ch);
                return;
        }
    }

    public void ScanHexadecimalEscape(StringBuilder sb, int numDigits)
    {
        var escapedValue = ScanExactNumberOfHexDigits(numDigits);
        if (escapedValue >= 0)
        {
            sb.Append((char)escapedValue);
        }
        else
        {
            Error(Diagnostics.Hexadecimal_digit_expected);
        }
    }

    public void ScanExtendedUnicodeEscape(StringBuilder sb)
    {
        var codePoint = ScanMinimumNumberOfHexDigits(1);
        if (codePoint < 0)
        {
            Error(Diagnostics.Hexadecimal_digit_expected);
            return;
        }
        
        if (codePoint > 0x10FFFF)
        {
            Error(Diagnostics.An_extended_Unicode_escape_value_must_be_between_0x0_and_0x10FFFF_inclusive);
            return;
        }

        if (_pos >= _end)
        {
            Error(Diagnostics.Unexpected_end_of_text);
            return;
        }

        if (_text[_pos] == '}')
        {
            // Only swallow the following character up if it's a '}'.
            _pos++;
        }
        else
        {
            Error(Diagnostics.Unterminated_Unicode_escape_sequence);
            return;
        }

        if (codePoint <= 65535)
        {
            sb.Append((char)codePoint);
            return;
        }

        var codeUnit1 = (int)Math.Floor(((double)codePoint - 65536) / 1024) + 0xD800;
        var codeUnit2 = ((codePoint - 65536) % 1024) + 0xDC00;
        sb.Append((char)codeUnit1);
        sb.Append((char)codeUnit2);
    }

    public int PeekUnicodeEscape()
    {
        if (_pos + 5 < _end && _text[_pos + 1] == 'u')
        {
            var start = _pos;
            _pos += 2;
            var value = ScanExactNumberOfHexDigits(4);
            _pos = start;
            return value;
        }
        return -1;
    }

    public string ScanIdentifierParts()
    {
        var result = new StringBuilder();
        var start = _pos;
        while (_pos < _end)
        {
            var ch = _text[_pos];
            if (IsIdentifierPart(ch))
            {
                _pos++;
            }
            else
            if (ch == '\\')
            {
                ch = (char)PeekUnicodeEscape();
                if (!(ch >= 0 && IsIdentifierPart(ch)))
                {
                    break;
                }
                result.Append(_text, start, _pos - start);
                result.Append(ch);
                // Valid Unicode escape is always six characters
                _pos += 6;
                start = _pos;
            }
            else
            {
                break;
            }
        }
        result.Append(_text, start, _pos - start);
        return result.ToString();
    }

    public SyntaxKind GetIdentifierToken()
    {
        var len = _tokenValue.Length;
        if (len >= 2 && len <= 11)
        {
            var ch = _tokenValue[0];
            if (ch >= 'a' && ch <= 'z')
            {
                if (textToToken.ContainsKey(_tokenValue))
                {
                    _token = textToToken[_tokenValue];
                    return _token;
                }
            }
        }
        _token = SyntaxKind.Identifier;
        return _token;
    }

    public int ScanBinaryOrOctalDigits(int @base)
    {
        Debug.Assert(@base == 2 || @base == 8, "Expected either @base 2 or @base 8");
        var value = 0;
        var numberOfDigits = 0;
        while (true)
        {
            var ch = _text[_pos];
            var valueOfCh = ch - '0';
            if (!IsDigit(ch) || valueOfCh >= @base)
            {
                break;
            }
            value = value * @base + valueOfCh;
            _pos++;
            numberOfDigits++;
        }
        if (numberOfDigits == 0)
        {
            return -1;
        }
        return value;
    }

    public SyntaxKind Scan()
    {
        _startPos = _pos;
        _precedingLineBreak = false;
        _tokenIsUnterminated = false;
        while (true)
        {
            _tokenPos = _pos;
            if (_pos >= _end)
            {
                _token = SyntaxKind.EndOfFileToken;
                return _token;
            }
            var ch = _text[_pos];
            if (ch == '#' && _pos == 0 && IsShebangTrivia(_text, _pos))
            {
                _pos = ScanShebangTrivia(_text, _pos);
                continue;
            }
            switch (ch)
            {
                case '\n':
                case '\r':
                    _precedingLineBreak = true;
                    _pos++;
                    continue;
                case '\t':
                case '\v':
                case '\f':
                case ' ':
                    _pos++;
                    continue;
                case '!':
                    if (_text[_pos + 1] == '=')
                    {
                        if (_text[_pos + 2] == '=')
                        {
                            _pos += 3;
                            _token = SyntaxKind.ExclamationEqualsEqualsToken;
                            return _token;
                        }
                        _pos += 2;
                        _token = SyntaxKind.ExclamationEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.ExclamationToken;
                    return _token;
                case '"':
                case '\'':
                    _tokenValue = ScanString();
                    _token = SyntaxKind.StringLiteral;
                    return _token;
                case '`':
                    _token = ScanTemplateAndSetTokenValue();
                    return _token;
                case '%':
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.PercentEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.PercentToken;
                    return _token;
                case '&':
                    if (_text[_pos + 1] == '&')
                    {
                        _pos += 2;
                        _token = SyntaxKind.AmpersandAmpersandToken;
                        return _token;
                    }
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.AmpersandEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.AmpersandToken;
                    return _token;
                case '(':
                    _pos++;
                    _token = SyntaxKind.OpenParenToken;
                    return _token;
                case ')':
                    _pos++;
                    _token = SyntaxKind.CloseParenToken;
                    return _token;
                case '*':
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.AsteriskEqualsToken;
                        return _token;
                    }
                    if (_text[_pos + 1] == '*')
                    {
                        if (_text[_pos + 2] == '=')
                        {
                            _pos += 3;
                            _token = SyntaxKind.AsteriskAsteriskEqualsToken;
                            return _token;
                        }
                        _pos += 2;
                        _token = SyntaxKind.AsteriskAsteriskToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.AsteriskToken;
                    return _token;
                case '+':
                    if (_text[_pos + 1] == '+')
                    {
                        _pos += 2;
                        _token = SyntaxKind.PlusPlusToken;
                        return _token;
                    }
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.PlusEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.PlusToken;
                    return _token;
                case ',':
                    _pos++;
                    _token = SyntaxKind.CommaToken;
                    return _token;
                case '-':
                    if (_text[_pos + 1] == '-')
                    {
                        _pos += 2;
                        _token = SyntaxKind.MinusMinusToken;
                        return _token;
                    }
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.MinusEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.MinusToken;
                    return _token;
                case '.':
                    if (IsDigit(_text[_pos + 1]))
                    {
                        _tokenValue = ScanNumber();
                        _token = SyntaxKind.NumericLiteral;
                        return _token;
                    }
                    if (_text[_pos + 1] == '.' && _text[_pos + 2] == '.')
                    {
                        _pos += 3;
                        _token = SyntaxKind.DotDotDotToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.DotToken;
                    return _token;
                case '/':
                    if (_text[_pos + 1] == '/')
                    {
                        _pos += 2;
                        while (_pos < _end)
                        {
                            if (IsLineBreak(_text[_pos]))
                            {
                                break;
                            }
                            _pos++;
                        }
                        continue;
                    }

                    if (_text[_pos + 1] == '*')
                    {
                        _pos += 2;
                        var commentClosed = false;
                        while (_pos < _end)
                        {
                            var ch2 = _text[_pos];
                            if (ch2 == '*' && _text[_pos + 1] == '/')
                            {
                                _pos += 2;
                                commentClosed = true;
                                break;
                            }
                            if (IsLineBreak(ch2))
                            {
                                _precedingLineBreak = true;
                            }
                            _pos++;
                        }
                        if (!commentClosed)
                        {
                            Error(Diagnostics.Asterisk_Slash_expected);
                        }
                        continue;
                    }
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.SlashEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.SlashToken;
                    return _token;
                case '0':
                    if (_pos + 2 < _end && (_text[_pos + 1] == 'x' || _text[_pos + 1] == 'x'))
                    {
                        _pos += 2;
                        var value = ScanMinimumNumberOfHexDigits(1);
                        if (value < 0)
                        {
                            Error(Diagnostics.Hexadecimal_digit_expected);
                            value = 0;
                        }
                        _tokenValue = value.ToString();
                        _token = SyntaxKind.NumericLiteral;
                        return _token;
                    }
                    else
            if (_pos + 2 < _end && (_text[_pos + 1] == 'b' || _text[_pos + 1] == 'b'))
                    {
                        _pos += 2;
                        var value = ScanBinaryOrOctalDigits(/* base */ 2);
                        if (value < 0)
                        {
                            Error(Diagnostics.Binary_digit_expected);
                            value = 0;
                        }
                        _tokenValue = value.ToString();
                        _token = SyntaxKind.NumericLiteral;
                        return _token;
                    }
                    else
            if (_pos + 2 < _end && (_text[_pos + 1] == 'O' || _text[_pos + 1] == 'o'))
                    {
                        _pos += 2;
                        var value = ScanBinaryOrOctalDigits(/* base */ 8);
                        if (value < 0)
                        {
                            Error(Diagnostics.Octal_digit_expected);
                            value = 0;
                        }
                        _tokenValue = value.ToString();
                        _token = SyntaxKind.NumericLiteral;
                        return _token;
                    }
                    if (_pos + 1 < _end && IsOctalDigit(_text[_pos + 1]))
                    {
                        _tokenValue = ScanOctalDigits().ToString();
                        _token = SyntaxKind.NumericLiteral;
                        return _token;
                    }
                    goto caseLabel30;
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    caseLabel30: _tokenValue = ScanNumber();
                    _token = SyntaxKind.NumericLiteral;
                    return _token;
                case ':':
                    _pos++;
                    _token = SyntaxKind.ColonToken;
                    return _token;
                case ';':
                    _pos++;
                    _token = SyntaxKind.SemicolonToken;
                    return _token;
                case '<':
                    if (IsConflictMarkerTrivia(_text, _pos))
                    {
                        _pos = ScanConflictMarkerTrivia(_text, _pos, Error);
                        continue;
                    }
                    if (_text[_pos + 1] == '<')
                    {
                        if (_text[_pos + 2] == '=')
                        {
                            _pos += 3;
                            _token = SyntaxKind.LessThanLessThanEqualsToken;
                            return _token;
                        }
                        _pos += 2;
                        _token = SyntaxKind.LessThanLessThanToken;
                        return _token;
                    }
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.LessThanEqualsToken;
                        return _token;
                    }
                    if (_languageVariant == LanguageVariant.Jsx &&
                                                    _text[_pos + 1] == '/' &&
                                                    _text[_pos + 2] != '*')
                    {
                        _pos += 2;
                        _token = SyntaxKind.LessThanSlashToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.LessThanToken;
                    return _token;
                case '=':
                    if (IsConflictMarkerTrivia(_text, _pos))
                    {
                        _pos = ScanConflictMarkerTrivia(_text, _pos, Error);
                        continue;
                    }
                    if (_text[_pos + 1] == '=')
                    {
                        if (_text[_pos + 2] == '=')
                        {
                            _pos += 3;
                            _token = SyntaxKind.EqualsEqualsEqualsToken;
                            return _token;
                        }
                        _pos += 2;
                        _token = SyntaxKind.EqualsEqualsToken;
                        return _token;
                    }
                    if (_text[_pos + 1] == '>')
                    {
                        _pos += 2;
                        _token = SyntaxKind.EqualsGreaterThanToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.EqualsToken;
                    return _token;
                case '>':
                    if (IsConflictMarkerTrivia(_text, _pos))
                    {
                        _pos = ScanConflictMarkerTrivia(_text, _pos, Error);
                        continue;
                    }
                    _pos++;
                    _token = SyntaxKind.GreaterThanToken;
                    return _token;
                case '?':
                    _pos++;
                    _token = SyntaxKind.QuestionToken;
                    return _token;
                case '[':
                    _pos++;
                    _token = SyntaxKind.OpenBracketToken;
                    return _token;
                case ']':
                    _pos++;
                    _token = SyntaxKind.CloseBracketToken;
                    return _token;
                case '^':
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.CaretEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.CaretToken;
                    return _token;
                case '{':
                    _pos++;
                    _token = SyntaxKind.OpenBraceToken;
                    return _token;
                case '|':
                    if (_text[_pos + 1] == '|')
                    {
                        _pos += 2;
                        _token = SyntaxKind.BarBarToken;
                        return _token;
                    }
                    if (_text[_pos + 1] == '=')
                    {
                        _pos += 2;
                        _token = SyntaxKind.BarEqualsToken;
                        return _token;
                    }
                    _pos++;
                    _token = SyntaxKind.BarToken;
                    return _token;
                case '}':
                    _pos++;
                    _token = SyntaxKind.CloseBraceToken;
                    return _token;
                case '~':
                    _pos++;
                    _token = SyntaxKind.TildeToken;
                    return _token;
                case '@':
                    _pos++;
                    _token = SyntaxKind.AtToken;
                    return _token;
                case '\\':
                    var cookedChar = PeekUnicodeEscape();
                    if (cookedChar >= 0 && IsIdentifierStart(cookedChar))
                    {
                        _pos += 6;
                        _tokenValue = (char)cookedChar + ScanIdentifierParts();
                        _token = GetIdentifierToken();
                        return _token;
                    }
                    Error(Diagnostics.Invalid_character);
                    _pos++;
                    _token = SyntaxKind.Unknown;
                    return _token;
                default:
                    if (IsIdentifierStart(ch))
                    {
                        _pos++;
                        while (_pos < _end && IsIdentifierPart(ch = _text[_pos])) _pos++;
                        _tokenValue = _text[_tokenPos.._pos];
                        if (ch == '\\')
                        {
                            _tokenValue += ScanIdentifierParts();
                        }
                        return _token = GetIdentifierToken();
                    }
                    else if (IsWhiteSpaceSingleLine(ch))
                    {
                        _pos++;
                        continue;
                    }
                    else if (IsLineBreak(ch))
                    {
                        _precedingLineBreak = true;
                        _pos++;
                        continue;
                    }
                    Error(Diagnostics.Invalid_character);
                    _pos++;
                    _token = SyntaxKind.Unknown;
                    return _token;
            }
        }
    }

    public SyntaxKind ReScanGreaterToken()
    {
        if (_token == SyntaxKind.GreaterThanToken)
        {
            if (_text[_pos] == '>')
            {
                if (_text[_pos + 1] == '>')
                {
                    if (_text[_pos + 2] == '=')
                    {
                        _pos += 3;
                        _token = SyntaxKind.GreaterThanGreaterThanGreaterThanEqualsToken;
                        return _token;
                    }
                    _pos += 2;
                    _token = SyntaxKind.GreaterThanGreaterThanGreaterThanToken;
                    return _token;
                }
                if (_text[_pos + 1] == '=')
                {
                    _pos += 2;
                    _token = SyntaxKind.GreaterThanGreaterThanEqualsToken;
                    return _token;
                }
                _pos++;
                _token = SyntaxKind.GreaterThanGreaterThanToken;
                return _token;
            }
            if (_text[_pos] == '=')
            {
                _pos++;
                _token = SyntaxKind.GreaterThanEqualsToken;
                return _token;
            }
        }
        return _token;
    }

    public SyntaxKind ReScanSlashToken()
    {
        if (_token == SyntaxKind.SlashToken || _token == SyntaxKind.SlashEqualsToken)
        {
            var p = _tokenPos + 1;
            var inEscape = false;
            var inCharacterClass = false;
            while (true)
            {
                if (p >= _end)
                {
                    _tokenIsUnterminated = true;
                    Error(Diagnostics.Unterminated_regular_expression_literal);
                    break;
                }
                var ch = _text[p];
                if (IsLineBreak(ch))
                {
                    _tokenIsUnterminated = true;
                    Error(Diagnostics.Unterminated_regular_expression_literal);
                    break;
                }
                if (inEscape)
                {
                    // Parsing an escape character;
                    // reset the flag and just advance to the next char.
                    inEscape = false;
                }
                else
                if (ch == '/' && !inCharacterClass)
                {
                    // A slash within a character class is permissible,
                    // but in general it signals the end of the regexp literal.
                    p++;
                    break;
                }
                else
                if (ch == '[')
                {
                    inCharacterClass = true;
                }
                else
                if (ch == '\\')
                {
                    inEscape = true;
                }
                else
                if (ch == ']')
                {
                    inCharacterClass = false;
                }
                p++;
            }
            while (p < _end && IsIdentifierPart(_text[p]))
            {
                p++;
            }
            _pos = p;
            _tokenValue = _text[_tokenPos.._pos];
            _token = SyntaxKind.RegularExpressionLiteral;
        }
        return _token;
    }

    public SyntaxKind ReScanTemplateToken()
    {
        Debug.Assert(_token == SyntaxKind.CloseBraceToken, "'reScanTemplateToken' should only be called on a '}'");
        _pos = _tokenPos;
        _token = ScanTemplateAndSetTokenValue();
        return _token;
    }

    public SyntaxKind ReScanJsxToken()
    {
        _pos = _tokenPos = _startPos;
        _token = ScanJsxToken();
        return _token;
    }

    public SyntaxKind ScanJsxToken()
    {
        _startPos = _tokenPos = _pos;
        if (_pos >= _end)
        {
            _token = SyntaxKind.EndOfFileToken;
            return _token;
        }
        var @char = _text[_pos];
        if (@char == '<')
        {
            if (_text[_pos + 1] == '/')
            {
                _pos += 2;
                _token = SyntaxKind.LessThanSlashToken;
                return _token;
            }
            _pos++;
            _token = SyntaxKind.LessThanToken;
            return _token;
        }
        if (@char == '{')
        {
            _pos++;
            _token = SyntaxKind.OpenBraceToken;
            return _token;
        }
        while (_pos < _end)
        {
            _pos++;
            @char = _text[_pos];
            if (@char == '{')
            {
                break;
            }
            if (@char == '<')
            {
                if (IsConflictMarkerTrivia(_text, _pos))
                {
                    _pos = ScanConflictMarkerTrivia(_text, _pos, Error);
                    _token = SyntaxKind.ConflictMarkerTrivia;
                    return _token;
                }
                break;
            }
        }
        _token = SyntaxKind.JsxText;
        return _token;
    }

    public SyntaxKind ScanJsxIdentifier()
    {
        if (TokenIsIdentifierOrKeyword(_token))
        {
            var firstCharPosition = _pos;
            while (_pos < _end)
            {
                var ch = _text[_pos];
                if (ch == '-' || ((firstCharPosition == _pos) ? IsIdentifierStart(ch) : IsIdentifierPart(ch)))
                {
                    _pos++;
                }
                else
                {
                    break;
                }
            }
            _tokenValue += _text[firstCharPosition.._pos];
        }
        return _token;
    }

    public SyntaxKind ScanJsxAttributeValue()
    {
        _startPos = _pos;
        switch (_text[_pos])
        {
            case '"':
            case '\'':
                _tokenValue = ScanString(/*allowEscapes*/ false);
                _token = SyntaxKind.StringLiteral;
                return _token;
            default:
                // If this scans anything other than `{`, it's a parse error.
                return Scan();
        }
    }

    public SyntaxKind ScanJsDocToken()
    {
        if (_pos >= _end)
        {
            _token = SyntaxKind.EndOfFileToken;
            return _token;
        }
        _startPos = _pos;
        _tokenPos = _pos;
        var ch = _text[_pos];
        switch (ch)
        {
            case '\t':
            case '\v':
            case '\f':
            case ' ':
                while (_pos < _end && IsWhiteSpaceSingleLine(_text[_pos]))
                {
                    _pos++;
                }
                _token = SyntaxKind.WhitespaceTrivia;
                return _token;
            case '@':
                _pos++;
                _token = SyntaxKind.AtToken;
                return _token;
            case '\n':
            case '\r':
                _pos++;
                _token = SyntaxKind.NewLineTrivia;
                return _token;
            case '*':
                _pos++;
                _token = SyntaxKind.AsteriskToken;
                return _token;
            case '{':
                _pos++;
                _token = SyntaxKind.OpenBraceToken;
                return _token;
            case '}':
                _pos++;
                _token = SyntaxKind.CloseBraceToken;
                return _token;
            case '[':
                _pos++;
                _token = SyntaxKind.OpenBracketToken;
                return _token;
            case ']':
                _pos++;
                _token = SyntaxKind.CloseBracketToken;
                return _token;
            case '=':
                _pos++;
                _token = SyntaxKind.EqualsToken;
                return _token;
            case ',':
                _pos++;
                _token = SyntaxKind.CommaToken;
                return _token;
            case '.':
                _pos++;
                _token = SyntaxKind.DotToken;
                return _token;
        }
        if (IsIdentifierStart(ch))
        {
            _pos++;
            while (IsIdentifierPart(_text[_pos]) && _pos < _end)
            {
                _pos++;
            }
            _token = SyntaxKind.Identifier;
            return _token;
        }
        else
        {
            _pos += 1;
            _token = SyntaxKind.Unknown;
            return _token;
        }
    }

    public T SpeculationHelper<T>(Func<T> callback, bool isLookahead)
    {
        var savePos = _pos;
        var saveStartPos = _startPos;
        var saveTokenPos = _tokenPos;
        var saveToken = _token;
        var saveTokenValue = _tokenValue;
        var savePrecedingLineBreak = _precedingLineBreak;
        var result = callback();
        if (result == null || ((result is bool) && Convert.ToBoolean(result) == false) || isLookahead)
        {
            _pos = savePos;
            _startPos = saveStartPos;
            _tokenPos = saveTokenPos;
            _token = saveToken;
            _tokenValue = saveTokenValue;
            _precedingLineBreak = savePrecedingLineBreak;
        }
        return result;
    }

    public T ScanRange<T>(int? start, int length, Func<T> callback)
    {
        var saveEnd = _end;
        var savePos = _pos;
        var saveStartPos = _startPos;
        var saveTokenPos = _tokenPos;
        var saveToken = _token;
        var savePrecedingLineBreak = _precedingLineBreak;
        var saveTokenValue = _tokenValue;
        var saveTokenIsUnterminated = _tokenIsUnterminated;
        SetText(_text, start, length);
        var result = callback();
        _end = saveEnd;
        _pos = savePos;
        _startPos = saveStartPos;
        _tokenPos = saveTokenPos;
        _token = saveToken;
        _precedingLineBreak = savePrecedingLineBreak;
        _tokenValue = saveTokenValue;
        _tokenIsUnterminated = saveTokenIsUnterminated;
        return result;
    }

    public T LookAhead<T>(Func<T> callback)
    {
        return SpeculationHelper(callback, /*isLookahead*/ true);
    }

    public T TryScan<T>(Func<T> callback)
    {
        return SpeculationHelper(callback, /*isLookahead*/ false);
    }

    public void SetText(string newText, int? start = null, int? length = null)
    {
        _text = newText ?? "";
        _end = length == null ? _text.Length : (int)start + (int)length;
        TextPos = start ?? 0;
    }

    public void SetOnError(ErrorCallback errorCallback)
    {
        OnError = errorCallback;
    }

    public void SetLanguageVariant(LanguageVariant variant)
    {
        _languageVariant = variant;
    }
}
