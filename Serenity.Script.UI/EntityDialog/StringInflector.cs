using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Serenity
{
    public static class StringInflector
    {
        private static List<object[]> plural;
        private static List<object[]> singular;
        private static List<string> countable;
        private static JsDictionary<string, string> pluralizeCache;
        private static JsDictionary<string, string> singularizeCache;
        private static bool initialized;

        private static void Initialize()
        {
            if (initialized)
                return;

            var plural = new List<object[]>();
            plural.Add(new object[] { new Regex("move", "i"), "moves" });
            plural.Add(new object[] { new Regex("sex", "i"), "sexes" });
            plural.Add(new object[] { new Regex("child", "i"), "children" });
            plural.Add(new object[] { new Regex("man", "i"), "men" });
            plural.Add(new object[] { new Regex("foot", "i"), "feet" });
            plural.Add(new object[] { new Regex("person", "i"), "people" });
            plural.Add(new object[] { new Regex("taxon", "i"), "taxa" });
            plural.Add(new object[] { new Regex("(quiz)", "i"), "$1zes" });
            plural.Add(new object[] { new Regex("^(ox)$", "i"), "$1en" });
            plural.Add(new object[] { new Regex("(m|l)ouse$", "i"), "$1ice" });
            plural.Add(new object[] { new Regex("(matr|vert|ind|suff)ix|ex$", "i"), "$1ices" });
            plural.Add(new object[] { new Regex("(x|ch|ss|sh)$", "i"), "$1es" });
            plural.Add(new object[] { new Regex("([^aeiouy]|qu)y$", "i"), "$1ies" });
            plural.Add(new object[] { new Regex("(?:([^f])fe|([lr])f)$", "i"), "$1$2ves" });
            plural.Add(new object[] { new Regex("sis$", "i"), "ses" });
            plural.Add(new object[] { new Regex("([ti]|addend)um$", "i"), "$1a" });
            plural.Add(new object[] { new Regex("(alumn|formul)a$", "i"), "$1ae" });
            plural.Add(new object[] { new Regex("(buffal|tomat|her)o$", "i"), "$1oes" });
            plural.Add(new object[] { new Regex("(bu)s$", "i"), "$1ses" });
            plural.Add(new object[] { new Regex("(alias|status)$", "i"), "$1es" });
            plural.Add(new object[] { new Regex("(octop|vir)us$", "i"), "$1i" });
            plural.Add(new object[] { new Regex("(gen)us$", "i"), "$1era" });
            plural.Add(new object[] { new Regex("(ax|test)is$", "i"), "$1es" });
            plural.Add(new object[] { new Regex("s$", "i"), "s" });
            plural.Add(new object[] { new Regex("$", "i"), "s" });

            var singular = new List<object[]>();
            singular.Add(new object[] { new Regex("cookies$", "i"), "cookie" });
            singular.Add(new object[] { new Regex("moves$", "i"), "move" });
            singular.Add(new object[] { new Regex("sexes$", "i"), "sex" });
            singular.Add(new object[] { new Regex("children$", "i"), "child" });
            singular.Add(new object[] { new Regex("men$", "i"), "man" });
            singular.Add(new object[] { new Regex("feet$", "i"), "foot" });
            singular.Add(new object[] { new Regex("people$", "i"), "person" });
            singular.Add(new object[] { new Regex("taxa$", "i"), "taxon" });
            singular.Add(new object[] { new Regex("databases$", "i"), "database" });
            singular.Add(new object[] { new Regex("(quiz)zes$", "i"), "$1" });
            singular.Add(new object[] { new Regex("(matr|suff)ices$", "i"), "$1ix" });
            singular.Add(new object[] { new Regex("(vert|ind)ices$", "i"), "$1ex" });
            singular.Add(new object[] { new Regex("^(ox)en", "i"), "$1" });
            singular.Add(new object[] { new Regex("(alias|status)es$", "i"), "$1" });
            singular.Add(new object[] { new Regex("(tomato|hero|buffalo)es$", "i"), "$1" });
            singular.Add(new object[] { new Regex("([octop|vir])i$", "i"), "$1us" });
            singular.Add(new object[] { new Regex("(gen)era$", "i"), "$1us" });
            singular.Add(new object[] { new Regex("(cris|ax|test)es$", "i"), "$1is" });
            singular.Add(new object[] { new Regex("(shoe)s$", "i"), "$1" });
            singular.Add(new object[] { new Regex("(o)es$", "i"), "$1" });
            singular.Add(new object[] { new Regex("(bus)es$", "i"), "$1" });
            singular.Add(new object[] { new Regex("([m|l])ice$", "i"), "$1ouse" });
            singular.Add(new object[] { new Regex("(x|ch|ss|sh)es$", "i"), "$1" });
            singular.Add(new object[] { new Regex("(m)ovies$", "i"), "$1ovie" });
            singular.Add(new object[] { new Regex("(s)eries$", "i"), "$1eries" });
            singular.Add(new object[] { new Regex("([^aeiouy]|qu)ies$", "i"), "$1y" });
            singular.Add(new object[] { new Regex("([lr])ves$", "i"), "$1f" });
            singular.Add(new object[] { new Regex("(tive)s$", "i"), "$1" });
            singular.Add(new object[] { new Regex("(hive)s$", "i"), "$1" });
            singular.Add(new object[] { new Regex("([^f])ves$", "i"), "$1fe" });
            singular.Add(new object[] { new Regex("(^analy)ses$", "i"), "$1sis" });
            singular.Add(new object[] { new Regex("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "i"), "$1\\2sis" });
            singular.Add(new object[] { new Regex("([ti]|addend)a$", "i"), "$1um" });
            singular.Add(new object[] { new Regex("(alumn|formul)ae$", "i"), "$1a" });
            singular.Add(new object[] { new Regex("(n)ews$", "i"), "$1ews" });
            singular.Add(new object[] { new Regex("(.*)s$", "i"), "$1" });

            var countable = new List<string>();
            countable.Add("aircraft");
            countable.Add("cannon");
            countable.Add("deer");
            countable.Add("equipment");
            countable.Add("fish");
            countable.Add("information");
            countable.Add("money");
            countable.Add("moose");
            countable.Add("rice");
            countable.Add("series");
            countable.Add("sheep");
            countable.Add("species");
            countable.Add("swin");

            StringInflector.plural = plural;
            StringInflector.singular = singular;
            StringInflector.countable = countable;
            initialized = true;
        }

        public static string Pluralize(string word)
        {
            Initialize();

            if (pluralizeCache != null &&
                pluralizeCache.ContainsKey(word))
                return pluralizeCache[word];

            string result = word;

            foreach (object[] p in plural)
            {
                Regex r = (Regex)p[0];
                if (r.Test(word))
                {
                    result = word.Replace(r, (string)p[1]);
                    break;
                }
            }

            foreach (var c in countable)
            {
                if (word == c)
                {
                    result = c;
                    break;
                }
            }

            pluralizeCache = pluralizeCache ?? new JsDictionary<string, string>();
            pluralizeCache[word] = result;

            return result;
        }

        public static string Singularize(string word)
        {
            Initialize();

            if (singularizeCache != null &&
                singularizeCache.ContainsKey(word))
                return singularizeCache[word];

            string result = word;

            foreach (object[] p in singular)
            {
                Regex r = (Regex)p[0];
                if (r.Test(word))
                {
                    result = word.Replace(r, (string)p[1]);
                    break;
                }
            }

            foreach (string c in countable)
            {
                if (word == c)
                {
                    result = c;
                    break;
                }
            }

            singularizeCache = singularizeCache ?? new JsDictionary<string, string>();
            singularizeCache[word] = result;

            return result;
        }

        public static bool IsSingular(string word)
        {
            Initialize();
            return Singularize(Pluralize(word)).ToLower() == word.ToLower();
        }

        public static bool IsPlural(string word)
        {
            Initialize();
            return Pluralize(Singularize(word)).ToLower() == word.ToLower();
        }
    }
}