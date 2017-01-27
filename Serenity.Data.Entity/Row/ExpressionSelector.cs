using Serenity.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Data
{
    public class DialectExpressionSelector
    {
        private string connectionKey;
        private string dialectServerType;
        private string dialectTypeName;

        public DialectExpressionSelector(string connectionKey)
        {
            this.connectionKey = connectionKey;
        }

        public DialectExpressionSelector(ISqlDialect dialect)
        {
            dialect = dialect ?? SqlSettings.DefaultDialect;
            dialectServerType = dialect.ServerType;
            dialectTypeName = dialect.GetType().Name;
        }

        private bool IsMatch(string s)
        {
            return dialectServerType.StartsWith(s, StringComparison.OrdinalIgnoreCase) ||
                dialectTypeName.StartsWith(s, StringComparison.OrdinalIgnoreCase);
        }

        public ExpressionAttribute GetBestMatch(IEnumerable<ExpressionAttribute> expressions)
        {
            if (!expressions.Any(x => !string.IsNullOrEmpty(x.Dialect)))
                return expressions.FirstOrDefault();

            if (dialectTypeName == null)
            {
                ISqlDialect dialect = null;

                if (!string.IsNullOrEmpty(connectionKey))
                {
                    var csi = SqlConnections.TryGetConnectionString(connectionKey);
                    if (csi != null)
                        dialect = csi.Dialect;
                }

                dialect = dialect ?? SqlSettings.DefaultDialect;
                dialectServerType = dialect.ServerType;
                dialectTypeName = dialect.GetType().Name;
            }

            var st = dialectServerType;
            var tn = dialectTypeName;

            Dictionary<ExpressionAttribute, int> weight = null;

            var bestMatch = expressions.Where(x =>
            {
                if (string.IsNullOrEmpty(x.Dialect))
                    return true;

                if (x.Dialect.IndexOf(',') < 0)
                    return IsMatch(x.Dialect);

                var best = x.Dialect.Split(comma, StringSplitOptions.RemoveEmptyEntries)
                    .Select(z => z.Trim())
                    .Where(z => IsMatch(z))
                    .OrderByDescending(z => z.Length)
                    .FirstOrDefault();

                if (best != null)
                {
                    if (weight == null)
                        weight = new Dictionary<ExpressionAttribute, int>();

                    weight[x] = best.Length;
                    return true;
                }

                return false;
            })
            .OrderByDescending(x =>
            {
                if (string.IsNullOrEmpty(x.Dialect))
                    return 0;

                int w;
                if (weight != null && weight.TryGetValue(x, out w))
                    return w;

                return x.Dialect.Length;
            })
            .FirstOrDefault();

            return bestMatch;
        }


        private static readonly char[] comma = new char[] { ',' };
    }
}