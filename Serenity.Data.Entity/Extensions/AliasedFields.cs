using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Data
{
    public static class AliasedFields
    {
        private static readonly ConcurrentDictionary<Tuple<Type, string>, RowFieldsBase> cache =
            new ConcurrentDictionary<Tuple<Type, string>, RowFieldsBase>();

        public static TFields As<TFields>(this TFields fields, string alias)
            where TFields : RowFieldsBase, new()
        {
            Check.NotNullOrWhiteSpace(alias, "alias");

            if ((alias == "t0" || alias == "T0") &&
                (fields.alias == "t0" || fields.alias == "T0"))
            {
                return fields;
            }

            if (fields.AliasName != "t0" && fields.AliasName != "T0")
                throw new ArgumentException("You can't alias fields that are already aliased once!");

            var key = new Tuple<Type, string>(typeof(TFields), alias);

            RowFieldsBase cached;
            if (cache.TryGetValue(key, out cached))
                return (TFields)cached;

            var result = new TFields();
            result.Initialize();

            var aliasPrefix = alias + "_";

            var joinByKey = new HashSet<string>(result.joins.Keys, StringComparer.OrdinalIgnoreCase);

            Func<string, string> mapAlias = x =>
            {
                if (x == "t0" || x == "T0")
                    return alias;

                if (!joinByKey.Contains(x))
                    return x;

                return aliasPrefix + x;
            };

            Func<string, string> mapExpression = x =>
            {
                if (x == null)
                    return null;

                return JoinAliasLocator.ReplaceAliases(x, mapAlias);
            };

            foreach (var field in result)
            {
                field.expression = mapExpression(field.expression);

                if (field.referencedAliases != null && field.ReferencedAliases.Count > 0)
                {
                    var old = field.ReferencedAliases.ToArray();
                    field.ReferencedAliases.Clear();
                    foreach (var x in old)
                        field.ReferencedAliases.Add(mapAlias(x));
                }

                field.join = null;
                field.joinAlias = field.joinAlias == null ? null : mapAlias(field.joinAlias);
            }

            var oldJoins = result.joins.ToArray();
            result.joins.Clear();
            foreach (var join in oldJoins)
            {
                new ReplacedJoin(result.Joins, 
                    mapExpression(join.Value.Table), 
                    mapAlias(join.Value.Name),
                    new Criteria(mapExpression(join.Value.OnCriteria.ToString())), 
                    join.Value.GetKeyword());
            }

            result.alias = alias;
            result.aliasDot = alias + ".";

            cache[key] = result;
            return result;
        }

        private class ReplacedJoin : Join
        {
            private string keyword;

            public ReplacedJoin(IDictionary<string, Join> joins, 
                string toTable, string alias, ICriteria onCriteria, string keyword)
                : base(joins, toTable, alias, onCriteria)
            {
                this.keyword = keyword;
            }

            public override string GetKeyword()
            {
                return keyword;
            }
        }
    }
}