using Serenity.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Collections.Concurrent;

namespace Serenity.Data
{
    internal class RowPropertyDictionary
    {
        internal Type rowType;
        internal string connectionKey;
        internal string tableName;
        internal string alias;
        internal string aliasDot;
        internal Dictionary<string, PropertyInfo> propertyByName;
        internal Dictionary<string, Tuple<string, ForeignKeyAttribute, LeftJoinAttribute>> joinPropertyByAlias;
        internal Dictionary<string, ISqlJoin> rowJoinByAlias;
        internal Dictionary<string, OriginAttribute> origins;
        internal ILookup<string, KeyValuePair<string, OriginAttribute>> originByAlias;
        internal DialectExpressionSelector expressionSelector;
        internal IDictionary<string, string> prefixByAlias;

        internal static ConcurrentDictionary<Type, RowPropertyDictionary> cache =
            new ConcurrentDictionary<Type, RowPropertyDictionary>();

        public RowPropertyDictionary(Type rowType)
        {
            this.rowType = rowType;
            this.alias = "T0";
            this.aliasDot = "T0.";
            this.rowJoinByAlias = new Dictionary<string, ISqlJoin>(StringComparer.OrdinalIgnoreCase);
            var tbn = this.rowType.GetCustomAttribute<TableNameAttribute>();
            if (tbn != null)
                tableName = tbn.Name;
            else
                tableName = "!!UNKNOWN!!";

            var connectionKeyAttr = rowType.GetCustomAttribute<ConnectionKeyAttribute>();
            if (connectionKeyAttr != null)
                connectionKey = connectionKeyAttr.Value;
            else
                connectionKey = "Default";

            expressionSelector = new DialectExpressionSelector(connectionKey);

            propertyByName = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var pi in rowType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                propertyByName[pi.Name] = pi;

            origins = new Dictionary<string, OriginAttribute>(StringComparer.OrdinalIgnoreCase);

            joinPropertyByAlias = new Dictionary<string, Tuple<string, ForeignKeyAttribute, LeftJoinAttribute>>();
            foreach (var property in propertyByName.Values)
            {
                var originAttr = property.GetCustomAttribute<OriginAttribute>();
                if (originAttr != null)
                    origins[property.Name] = originAttr;

                var lj = property.GetCustomAttribute<LeftJoinAttribute>();
                var fk = property.GetCustomAttribute<ForeignKeyAttribute>();
                if (lj != null && fk != null)
                    joinPropertyByAlias[lj.Alias] = new Tuple<string, ForeignKeyAttribute, LeftJoinAttribute>(property.Name, fk, lj);
            }

            foreach (var attr in rowType.GetCustomAttributes<LeftJoinAttribute>())
                rowJoinByAlias[attr.Alias] = attr;

            foreach (var attr in rowType.GetCustomAttributes<InnerJoinAttribute>())
                rowJoinByAlias[attr.Alias] = attr;

            foreach (var attr in rowType.GetCustomAttributes<OuterApplyAttribute>())
                rowJoinByAlias[attr.Alias] = attr;

            originByAlias = origins.ToLookup(x => x.Value.Join);
            prefixByAlias = originByAlias.ToDictionary(x => x.Key, x =>
            {
                Tuple<string, ForeignKeyAttribute, LeftJoinAttribute> joinProperty;
                if (joinPropertyByAlias.TryGetValue(x.Key, out joinProperty))
                {
                    if (joinProperty.Item3.Prefix != null)
                        return joinProperty.Item3.Prefix;

                    if (joinProperty.Item1.EndsWith("ID") ||
                        joinProperty.Item1.EndsWith("Id"))
                        return joinProperty.Item1.Substring(0, joinProperty.Item1.Length - 2);
                }

                ISqlJoin join;
                if (rowJoinByAlias.TryGetValue(x.Key, out join))
                {
                    if (join.Prefix != null)
                        return join.Prefix;
                }

                return DeterminePrefix(x.Select(z => z.Key));
            });
        }

        private static string DeterminePrefix(IEnumerable<string> items)
        {
            if (items.Count() < 2)
                return "";

            var enumerator = items.GetEnumerator();
            enumerator.MoveNext();
            string prefix = enumerator.Current ?? "";
            while (prefix.Length > 0 && enumerator.MoveNext())
            {
                var current = enumerator.Current ?? "";
                if (current.Length < prefix.Length)
                    prefix = prefix.Substring(0, current.Length);

                while (!current.StartsWith(prefix) && prefix.Length > 0)
                    prefix = prefix.Substring(0, prefix.Length - 1);
            }

            return prefix;
        }

        public static RowPropertyDictionary Get(Type rowType)
        {
            RowPropertyDictionary dictionary;
            if (!cache.TryGetValue(rowType, out dictionary))
            {
                dictionary = new RowPropertyDictionary(rowType);
                cache[rowType] = dictionary;
            }
            return dictionary;
        }

        public ExpressionAttribute OriginExpression(PropertyInfo property, OriginAttribute origin,
            DialectExpressionSelector expressionSelector, string aliasPrefix, out Dictionary<string, ISqlJoin> originJoins)
        {
            if (aliasPrefix.Length >= 1000)
                throw new DivideByZeroException("Infinite origin recursion detected!");

            originJoins = null;
            var joinAlias = origin.Join;
            Tuple<string, ForeignKeyAttribute, LeftJoinAttribute> joinProperty;

            Type originRowType;
            ISqlJoin rowJoin;
            if (joinPropertyByAlias.TryGetValue(joinAlias, out joinProperty))
            {
                var joinPropertyName = joinProperty.Item1;
                var fk = joinProperty.Item2;
                var lj = joinProperty.Item3;
                originRowType = lj.RowType ?? fk.RowType;

                if (originRowType == null)
                {
                    throw new ArgumentOutOfRangeException("origin", String.Format(
                        "Property '{0}' on row type '{1}' has a [Origin] attribute, " +
                        "but [ForeignKey] and [LeftJoin] attributes on related join " +
                        "property '{2}' doesn't use a typeof(SomeRow)!",
                            property.Name, rowType.Name, joinPropertyName));
                }
            }
            else if (rowJoinByAlias.TryGetValue(joinAlias, out rowJoin))
            {
                originRowType = rowJoin.RowType;
                if (originRowType == null)
                {
                    throw new ArgumentOutOfRangeException("origin", String.Format(
                        "Property '{0}' on row type '{1}' has a [Origin] attribute, " +
                        "but related join declaration on row has no RowType!",
                            property.Name, rowType.Name, joinAlias));
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("origin", String.Format(
                    "Property '{0}' on row type '{1}' has a [Origin] attribute, " +
                    "but declaration of join '{2}' is not found!",
                        property.Name, rowType.Name, joinAlias));
            }

            var originDictionary = Get(originRowType);
            string originPropertyName;
            PropertyInfo originProperty = null;

            if (origin.Property != null)
                originPropertyName = origin.Property;
            else
            {
                var prefix = prefixByAlias[origin.Join];
                if (prefix != null &&
                    prefix.Length > 0 &&
                    property.Name.StartsWith(prefix) &&
                    property.Name.Length > prefix.Length &&
                    originDictionary.propertyByName.TryGetValue(
                        property.Name.Substring(prefix.Length), out originProperty))
                {
                    originPropertyName = originProperty.Name;
                }
                else
                    originPropertyName = property.Name;
            }

            if (originProperty == null &&
                !originDictionary.propertyByName.TryGetValue(originPropertyName, out originProperty))
            {
                throw new ArgumentOutOfRangeException("origin", String.Format(
                    "Property '{0}' on row type '{1}' has a [Origin] attribute, " +
                    "but its corresponding property '{2}' on row type '{3}' is not found!",
                        property.Name, rowType.Name, originPropertyName, originRowType.Name));
            }

            if (aliasPrefix.Length == 0)
                aliasPrefix = origin.Join;
            else
                aliasPrefix = aliasPrefix + "_" + origin.Join;

            var columnAttr = originProperty.GetCustomAttribute<ColumnAttribute>();
            if (columnAttr != null)
                return new ExpressionAttribute(aliasPrefix + "." + SqlSyntax.AutoBracket(columnAttr.Name));
            else
            {
                var expressionAttr = originProperty.GetCustomAttributes<ExpressionAttribute>();
                if (expressionAttr.Any())
                {
                    var expression = expressionSelector.GetBestMatch(expressionAttr, x => x.Dialect);
                    return new ExpressionAttribute(PrefixAliases(expression.Value, aliasPrefix, out originJoins));
                }
                else
                {
                    var originOrigin = originProperty.GetCustomAttribute<OriginAttribute>();
                    if (originOrigin != null)
                    {
                        Dictionary<string, ISqlJoin> originOriginJoins;


                        var expression = originDictionary.OriginExpression(originProperty, originOrigin, expressionSelector,
                            aliasPrefix, out originOriginJoins);

                        if (originOriginJoins != null)
                        {
                        }

                        return new ExpressionAttribute(PrefixAliases(expression.Value, origin.Join, out originJoins));
                    }
                    else
                        return new ExpressionAttribute(origin.Join + "." + SqlSyntax.AutoBracket(originProperty.Name));
                }
            }
        }

        private string PrefixAliases(string expression, string alias, 
            out Dictionary<string, ISqlJoin> joins)
        {
            joins = null;

            if (string.IsNullOrWhiteSpace(expression))
                return expression;

            Check.NotNullOrWhiteSpace(alias, "alias");

            var aliasPrefix = alias + "_";

            var mappedJoins = new Dictionary<string, ISqlJoin>();

            Func<string, string> mapAlias = null;

            Func<string, string> mapExpression = x =>
            {
                if (x == null)
                    return null;

                return JoinAliasLocator.ReplaceAliases(x, mapAlias);
            };

            mapAlias = x =>
            {
                if (x == "t0" || x == "T0")
                    return alias;

                ISqlJoin sqlJoin;
                if (mappedJoins.TryGetValue(x, out sqlJoin))
                    return sqlJoin.Alias;

                Tuple<string, ForeignKeyAttribute, LeftJoinAttribute> propJoin;
                if (joinPropertyByAlias.TryGetValue(x, out propJoin))
                    Console.WriteLine("hello");

                if (rowJoinByAlias.TryGetValue(x, out sqlJoin))
                {
                    var lja = sqlJoin as LeftJoinAttribute;
                    if (lja != null)
                    {
                        Console.WriteLine("hello2");
                    }

                    var ija = sqlJoin as InnerJoinAttribute;
                    if (ija != null)
                    {
                        Console.WriteLine("hello3");
                    }

                    var oaa = sqlJoin as OuterApplyAttribute;
                    if (oaa != null)
                    {
                        Console.WriteLine("hello4");
                    }
                }

                //if (recursiveJoins != null &&
                //    recursiveJoins.TryGetValue(x, out sqlJoin))
                //{
                //    var lja = sqlJoin as LeftJoinAttribute;
                //    if (lja != null)
                //    {
                //        Console.WriteLine("hello2");
                //    }

                //    var ija = sqlJoin as InnerJoinAttribute;
                //    if (ija != null)
                //    {
                //        Console.WriteLine("hello3");
                //    }

                //    var oaa = sqlJoin as OuterApplyAttribute;
                //    if (oaa != null)
                //    {
                //        Console.WriteLine("hello4");
                //    }
                //}

                return x;
            };

            return mapExpression(expression);
        }
    }
}