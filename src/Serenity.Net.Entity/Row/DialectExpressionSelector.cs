using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Data
{
    public class DialectExpressionSelector
    {
        private readonly string dialectServerType;
        private readonly string dialectTypeName;

        public DialectExpressionSelector(ISqlDialect dialect)
        {
            if (dialect == null)
                throw new ArgumentNullException(nameof(dialect));
            dialectServerType = dialect.ServerType;
            dialectTypeName = dialect.GetType().Name;
        }

        private bool IsMatch(string s)
        {
            return dialectServerType.StartsWith(s, StringComparison.OrdinalIgnoreCase) ||
                dialectTypeName.StartsWith(s, StringComparison.OrdinalIgnoreCase);
        }

        public TAttribute GetBestMatch<TAttribute>(IEnumerable<TAttribute> expressions, 
            Func<TAttribute, string> getDialect)
        {
            if (!expressions.Any(x => !string.IsNullOrEmpty(getDialect(x))))
                return expressions.FirstOrDefault();

            var st = dialectServerType;
            var tn = dialectTypeName;

            Dictionary<TAttribute, int> weight = null;

            var bestMatch = expressions.Where(x =>
            {
                var d = getDialect(x);

                if (string.IsNullOrEmpty(getDialect(x)))
                    return true;

                if (d.IndexOf(',') < 0)
                    return IsMatch(d);

                var best = d.Split(comma, StringSplitOptions.RemoveEmptyEntries)
                    .Select(z => z.Trim())
                    .Where(z => IsMatch(z))
                    .OrderByDescending(z => z.Length)
                    .FirstOrDefault();

                if (best != null)
                {
                    if (weight == null)
                        weight = new Dictionary<TAttribute, int>();

                    weight[x] = best.Length;
                    return true;
                }

                return false;
            })
            .OrderByDescending(x =>
            {
                var d = getDialect(x);
                if (string.IsNullOrEmpty(d))
                    return 0;

                if (weight != null && weight.TryGetValue(x, out int w))
                    return w;

                return d.Length;
            })
            .FirstOrDefault();

            return bestMatch;
        }

        private static readonly char[] comma = new char[] { ',' };
    }
}