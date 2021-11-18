using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Data
{
    /// <summary>
    /// Selects field expressions based on dialect
    /// </summary>
    public class DialectExpressionSelector
    {
        private readonly string dialectServerType;
        private readonly string dialectTypeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialectExpressionSelector"/> class.
        /// </summary>
        /// <param name="dialect">The dialect.</param>
        /// <exception cref="ArgumentNullException">dialect</exception>
        public DialectExpressionSelector(ISqlDialect dialect)
        {
            if (dialect == null)
                throw new ArgumentNullException(nameof(dialect));
            dialectServerType = dialect.ServerType;
            dialectTypeName = dialect.GetType().Name;
        }

        /// <summary>
        /// Gets if the dialect is a match, e.g. dialect server type or dialect type name starts with given name
        /// </summary>
        /// <param name="dialect">The dialect specifier.</param>
        /// <returns></returns>
        public bool IsMatch(string dialect)
        {
            if (string.IsNullOrEmpty(dialect))
                return true;

            if (!dialect.Contains(',', StringComparison.Ordinal))
                return InternalIsMatch(dialect);

            return dialect.Split(comma, StringSplitOptions.RemoveEmptyEntries)
                .Select(z => z.Trim())
                .Any(InternalIsMatch);
        }

        private bool InternalIsMatch(string dialect)
        {
            return dialectServerType.StartsWith(dialect, StringComparison.OrdinalIgnoreCase) ||
                dialectTypeName.StartsWith(dialect, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the best match.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="expressions">The expressions.</param>
        /// <param name="getDialect">The get dialect.</param>
        /// <returns></returns>
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

                if (!d.Contains(',', StringComparison.Ordinal))
                    return InternalIsMatch(d);

                var best = d.Split(comma, StringSplitOptions.RemoveEmptyEntries)
                    .Select(z => z.Trim())
                    .Where(InternalIsMatch)
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