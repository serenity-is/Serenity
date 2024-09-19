namespace Serenity.TypeScript;

partial class Scanner
{
    private static readonly Dictionary<int, RegularExpressionFlags> CharCodeToRegExpFlag = new()
    {
        [CharacterCodes.d] = RegularExpressionFlags.HasIndices,
        [CharacterCodes.g] = RegularExpressionFlags.Global,
        [CharacterCodes.i] = RegularExpressionFlags.IgnoreCase,
        [CharacterCodes.m] = RegularExpressionFlags.Multiline,
        [CharacterCodes.s] = RegularExpressionFlags.DotAll,
        [CharacterCodes.u] = RegularExpressionFlags.Unicode,
        [CharacterCodes.v] = RegularExpressionFlags.UnicodeSets,
        [CharacterCodes.y] = RegularExpressionFlags.Sticky,
    };

    RegularExpressionFlags? CharacterCodeToRegularExpressionFlag(int ch)
    {
        return CharCodeToRegExpFlag.TryGetValue(ch, out RegularExpressionFlags value) ? value : null;
    }

    private static readonly Dictionary<RegularExpressionFlags, LanguageFeatureMinimumTarget> RegExpFlagToFirstAvailableLanguageVersion = new()
    {
        [RegularExpressionFlags.HasIndices] = LanguageFeatureMinimumTarget.RegularExpressionFlagsHasIndices,
        [RegularExpressionFlags.DotAll] = LanguageFeatureMinimumTarget.RegularExpressionFlagsDotAll,
        [RegularExpressionFlags.Unicode] = LanguageFeatureMinimumTarget.RegularExpressionFlagsUnicode,
        [RegularExpressionFlags.UnicodeSets] = LanguageFeatureMinimumTarget.RegularExpressionFlagsUnicodeSets,
        [RegularExpressionFlags.Sticky] = LanguageFeatureMinimumTarget.RegularExpressionFlagsSticky
    };

    public class RegexGroupNameRef : TextRange
    {
        public string Name { get; set; }
    }

    public class RegexDecimalEscape : TextRange
    {
        public decimal Value { get; set; }
    }
}