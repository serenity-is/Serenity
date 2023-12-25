namespace Inflector;

public static partial class Inflector
{
    #region Default Rules

    static Inflector()
    {
        AddPlural("$", "s");
        AddPlural("s$", "s");
        AddPlural("(ax|test)is$", "$1es");
        AddPlural("(octop|vir|alumn|fung)us$", "$1i");
        AddPlural("(alias|status)$", "$1es");
        AddPlural("(bu)s$", "$1ses");
        AddPlural("(buffal|tomat|volcan)o$", "$1oes");
        AddPlural("([ti])um$", "$1a");
        AddPlural("sis$", "ses");
        AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
        AddPlural("(hive)$", "$1s");
        AddPlural("([^aeiouy]|qu)y$", "$1ies");
        AddPlural("(x|ch|ss|sh)$", "$1es");
        AddPlural("(matr|vert|ind)ix|ex$", "$1ices");
        AddPlural("([m|l])ouse$", "$1ice");
        AddPlural("^(ox)$", "$1en");
        AddPlural("(quiz)$", "$1zes");

        AddSingular("s$", "");
        AddSingular("(n)ews$", "$1ews");
        AddSingular("([ti])a$", "$1um");
        AddSingular("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
        AddSingular("(^analy)ses$", "$1sis");
        AddSingular("([^f])ves$", "$1fe");
        AddSingular("(hive)s$", "$1");
        AddSingular("(tive)s$", "$1");
        AddSingular("([lr])ves$", "$1f");
        AddSingular("([^aeiouy]|qu)ies$", "$1y");
        AddSingular("(s)eries$", "$1eries");
        AddSingular("(m)ovies$", "$1ovie");
        AddSingular("(x|ch|ss|sh)es$", "$1");
        AddSingular("([m|l])ice$", "$1ouse");
        AddSingular("(bus)es$", "$1");
        AddSingular("(o)es$", "$1");
        AddSingular("(shoe)s$", "$1");
        AddSingular("(cris|ax|test)es$", "$1is");
        AddSingular("(octop|vir|alumn|fung)i$", "$1us");
        AddSingular("(alias|status)es$", "$1");
        AddSingular("^(ox)en", "$1");
        AddSingular("(vert|ind)ices$", "$1ex");
        AddSingular("(matr)ices$", "$1ix");
        AddSingular("(quiz)zes$", "$1");

        AddIrregular("person", "people");
        AddIrregular("man", "men");
        AddIrregular("child", "children");
        AddIrregular("sex", "sexes");
        AddIrregular("move", "moves");
        AddIrregular("goose", "geese");
        AddIrregular("alumna", "alumnae");

        AddUncountable("equipment");
        AddUncountable("information");
        AddUncountable("rice");
        AddUncountable("money");
        AddUncountable("species");
        AddUncountable("series");
        AddUncountable("fish");
        AddUncountable("sheep");
        AddUncountable("deer");
        AddUncountable("aircraft");
    }

    #endregion

    private class Rule(string pattern, string replacement)
    {
        private readonly Regex _regex = new(pattern, RegexOptions.IgnoreCase);
        private readonly string _replacement = replacement;

        public string Apply(string word)
        {
            if (!_regex.IsMatch(word))
            {
                return null;
            }

            return _regex.Replace(word, _replacement);
        }
    }

    private static void AddIrregular(string singular, string plural)
    {
        AddPlural("(" + singular[0] + ")" + singular[1..] + "$", "$1" + plural[1..]);
        AddSingular("(" + plural[0] + ")" + plural[1..] + "$", "$1" + singular[1..]);
    }

    private static void AddUncountable(string word)
    {
        _uncountables.Add(word.ToLowerInvariant());
    }

    private static void AddPlural(string rule, string replacement)
    {
        _plurals.Add(new Rule(rule, replacement));
    }

    private static void AddSingular(string rule, string replacement)
    {
        _singulars.Add(new Rule(rule, replacement));
    }

    private static readonly List<Rule> _plurals = [];
    private static readonly List<Rule> _singulars = [];
    private static readonly List<string> _uncountables = [];

    public static string Pluralize(this string word)
    {
        return ApplyRules(_plurals, word);
    }

    public static string Singularize(this string word)
    {
        return ApplyRules(_singulars, word);
    }

    private static string ApplyRules(List<Rule> rules, string word)
    {
        string result = word;

        if (!_uncountables.Contains(word.ToLowerInvariant()))
        {
            for (int i = rules.Count - 1; i >= 0; i--)
            {
                if ((result = rules[i].Apply(word)) != null)
                {
                    break;
                }
            }
        }

        return result;
    }

    [GeneratedRegex(@"\b([a-z])")]
    private static partial Regex TitleizeRegexGen();

    public static string Titleize(this string word)
    {
        return TitleizeRegexGen().Replace(Humanize(Underscore(word)), delegate (Match match)
        {
            return match.Captures[0].Value.ToUpperInvariant();
        });
    }

    [GeneratedRegex(@"_")]
    private static partial Regex HumanizeRegexGen();

    public static string Humanize(this string lowercaseAndUnderscoredWord)
    {
        return Capitalize(HumanizeRegexGen().Replace(lowercaseAndUnderscoredWord, " "));
    }

    [GeneratedRegex("(?:^|_)(.)")]
    private static partial Regex PascalizeRegexGen();

    public static string Pascalize(this string lowercaseAndUnderscoredWord)
    {
        return PascalizeRegexGen().Replace(lowercaseAndUnderscoredWord, delegate (Match match)
                             {
                                 return match.Groups[1].Value.ToUpperInvariant();
                             });
    }

    public static string Camelize(this string lowercaseAndUnderscoredWord)
    {
        return Uncapitalize(Pascalize(lowercaseAndUnderscoredWord));
    }

    [GeneratedRegex(@"([A-Z]+)([A-Z][a-z])")]
    private static partial Regex UnderscoreRegexGen1();

    [GeneratedRegex(@"([a-z\d])([A-Z])")]
    private static partial Regex UnderscoreRegexGen2();

    [GeneratedRegex(@"[-\s]")]
    private static partial Regex UnderscoreRegexGen3();

    public static string Underscore(this string pascalCasedWord)
    {
        return UnderscoreRegexGen3().Replace(
            UnderscoreRegexGen2().Replace(
                UnderscoreRegexGen1().Replace(pascalCasedWord, "$1_$2"), "$1_$2"), "_").ToLowerInvariant();
    }

    public static string Capitalize(this string word)
    {
        return word[..1].ToUpperInvariant() + word[1..].ToLowerInvariant();
    }

    public static string Uncapitalize(this string word)
    {
        return word[..1].ToLowerInvariant() + word[1..];
    }

    public static string Ordinalize(this string numberString)
    {
        return Ordanize(int.Parse(numberString, CultureInfo.InvariantCulture), numberString);
    }

    public static string Ordinalize(this int number)
    {
        return Ordanize(number, number.ToString(CultureInfo.CurrentCulture));
    }

    private static string Ordanize(int number, string numberString)
    {
        int nMod100 = number % 100;

        if (nMod100 >= 11 && nMod100 <= 13)
        {
            return numberString + "th";
        }

        return (number % 10) switch
        {
            1 => numberString + "st",
            2 => numberString + "nd",
            3 => numberString + "rd",
            _ => numberString + "th",
        };
    }


    public static string Dasherize(this string underscoredWord)
    {
        return underscoredWord.Replace('_', '-');
    }

}
