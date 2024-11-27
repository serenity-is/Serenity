namespace Serenity.CodeGeneration;

public static partial class CSharpSyntaxRules
{
    private static readonly HashSet<string> Keywords = new(StringComparer.Ordinal)
    {
        "abstract",
        "as",
        "base",
        "bool",
        "breal",
        "byte",
        "case",
        "catch",
        "char",
        "checked",
        "class",
        "const",
        "continue",
        "decimal",
        "default",
        "delegate",
        "do",
        "double",
        "else",
        "enum",
        "event",
        "explicit",
        "extern",
        "false",
        "finally",
        "fixed",
        "float",
        "for",
        "foreach",
        "goto",
        "if",
        "implicit",
        "in",
        "int",
        "interface",
        "internal",
        "is",
        "lock",
        "long",
        "namespace",
        "new",
        "null",
        "object",
        "operator",
        "out",
        "override",
        "params",
        "private",
        "protected",
        "public",
        "readonly",
        "ref",
        "return",
        "sbyte",
        "sealed",
        "short",
        "sizeof",
        "stackalloc",
        "static",
        "string",
        "struct",
        "switch",
        "this",
        "throw",
        "true",
        "try",
        "typeof",
        "uint",
        "ulong",
        "unchecked",
        "unsafe",
        "ushort",
        "using",
        "virtual",
        "void",
        "volatile",
        "while"
    };


#if ISSOURCEGENERATOR
    const string formattingCharacter = @"\p{Cf}";
    const string connectingCharacter = @"\p{Pc}";
    const string decimalDigitCharacter = @"\p{Nd}";
    const string combiningCharacter = @"\p{Mn}|\p{Mc}";
    const string letterCharacter = @"\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}";
    const string identifierPartCharacter = letterCharacter + "|" + decimalDigitCharacter + "|" + 
        connectingCharacter + "|" + combiningCharacter + "|" + formattingCharacter;
    const string identifierPartCharacters = "(" + identifierPartCharacter + ")+";
    const string identifierStartCharacter = "(" + letterCharacter + "|_)";
    const string identifierOrKeyword = identifierStartCharacter + "(" + identifierPartCharacters + ")*";
    static readonly Regex ValidIdentifierRegex = new("^" + identifierOrKeyword + "$", RegexOptions.Compiled);
#else   
    static readonly Regex ValidIdentifierRegex = ValidIdentifierRegexGen();

    [GeneratedRegex(@"^(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|_)((\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\p{Nd}|\p{Pc}|\p{Mn}|\p{Mc}|\p{Cf})+)*$", RegexOptions.Compiled)]
    private static partial Regex ValidIdentifierRegexGen();
#endif

    public static bool IsKeyword(string identifier)
    {
        return Keywords.Contains(identifier);
    }

    public static bool IsValidIdentifier(string identifier, bool ignoreKeywords)
    {
        if (string.IsNullOrEmpty(identifier))
            return false;

        if (!ignoreKeywords && IsKeyword(identifier))
            return false;

        return ValidIdentifierRegex.IsMatch(identifier);
    }
}