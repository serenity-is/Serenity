using Serenity.Data.Mapping;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Serenity.Data
{
    internal class OriginPropertyDictionary
    {
        internal Type rowType;
        internal Dictionary<string, PropertyInfo> propertyByName;
        internal Dictionary<string, Tuple<string, ForeignKeyAttribute, ISqlJoin>> joinPropertyByAlias;
        internal Dictionary<string, ISqlJoin> rowJoinByAlias;
        internal Dictionary<string, OriginAttribute> origins;
        internal Dictionary<string, PropertyInfo> originPropertyByName;
        internal ILookup<string, KeyValuePair<string, OriginAttribute>> originByAlias;
        internal IDictionary<string, string> prefixByAlias;

        internal static ConcurrentDictionary<Type, OriginPropertyDictionary> cache =
            new ConcurrentDictionary<Type, OriginPropertyDictionary>();

        public OriginPropertyDictionary(Type rowType)
        {
            this.rowType = rowType;
            this.rowJoinByAlias = new Dictionary<string, ISqlJoin>(StringComparer.OrdinalIgnoreCase);

            propertyByName = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var pi in rowType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                propertyByName[pi.Name] = pi;

            origins = new Dictionary<string, OriginAttribute>(StringComparer.OrdinalIgnoreCase);

            joinPropertyByAlias = new Dictionary<string, Tuple<string, ForeignKeyAttribute, ISqlJoin>>();
            foreach (var property in propertyByName.Values)
            {
                var originAttr = property.GetCustomAttribute<OriginAttribute>();
                if (originAttr != null)
                    origins[property.Name] = originAttr;

                var lj = property.GetCustomAttribute<LeftJoinAttribute>();
                var fk = property.GetCustomAttribute<ForeignKeyAttribute>();
                if (lj != null && fk != null)
                    joinPropertyByAlias[lj.Alias] = new Tuple<string, ForeignKeyAttribute, ISqlJoin>(property.Name, fk, lj);
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
                Tuple<string, ForeignKeyAttribute, ISqlJoin> joinProperty;
                if (joinPropertyByAlias.TryGetValue(x.Key, out joinProperty))
                {
                    if (joinProperty.Item3.PropertyPrefix != null)
                        return joinProperty.Item3.PropertyPrefix;

                    if (joinProperty.Item1.EndsWith("ID") ||
                        joinProperty.Item1.EndsWith("Id"))
                        return joinProperty.Item1.Substring(0, joinProperty.Item1.Length - 2);
                }

                ISqlJoin join;
                if (rowJoinByAlias.TryGetValue(x.Key, out join))
                {
                    if (join.PropertyPrefix != null)
                        return join.PropertyPrefix;
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

        public static OriginPropertyDictionary GetPropertyDictionary(Type rowType)
        {
            OriginPropertyDictionary dictionary;
            if (!cache.TryGetValue(rowType, out dictionary))
            {
                dictionary = new OriginPropertyDictionary(rowType);
                cache[rowType] = dictionary;
            }
            return dictionary;
        }

        private PropertyInfo GetOriginProperty(string name)
        {
            PropertyInfo pi;

            var d = originPropertyByName;
            if (d == null)
            {
                d = new Dictionary<string, PropertyInfo>();
                d[name] = pi = GetOriginProperty(propertyByName[name], origins[name]);
                originPropertyByName = d;
            }
            else if (!d.TryGetValue(name, out pi))
            {
                d = new Dictionary<string, PropertyInfo>(d);
                d[name] = pi = GetOriginProperty(propertyByName[name], origins[name]);
                originPropertyByName = d;
            }

            return pi;
        }

        private PropertyInfo GetOriginProperty(PropertyInfo property, OriginAttribute origin)
        {
            var joinAlias = origin.Join;
            Tuple<string, ForeignKeyAttribute, ISqlJoin> joinProperty;

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

            var originDictionary = GetPropertyDictionary(originRowType);
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

            return originProperty;
        }

        public string OriginExpression(PropertyInfo property, OriginAttribute origin,
            DialectExpressionSelector expressionSelector, string aliasPrefix, List<Attribute> extraJoins)
        {
            if (aliasPrefix.Length >= 1000)
                throw new DivideByZeroException("Infinite origin recursion detected!");

            var originProperty = GetOriginProperty(property.Name);

            if (aliasPrefix.Length == 0)
                aliasPrefix = origin.Join;
            else
                aliasPrefix = aliasPrefix + "_" + origin.Join;

            var columnAttr = originProperty.GetCustomAttribute<ColumnAttribute>();
            if (columnAttr != null)
                return aliasPrefix + "." + SqlSyntax.AutoBracket(columnAttr.Name);
            else
            {
                var originDictionary = GetPropertyDictionary(originProperty.ReflectedType);

                var expressionAttr = originProperty.GetCustomAttributes<ExpressionAttribute>();
                if (expressionAttr.Any())
                {
                    var expression = expressionSelector.GetBestMatch(expressionAttr, x => x.Dialect);
                    return originDictionary.PrefixAliases(expression.Value, aliasPrefix, 
                        expressionSelector, extraJoins);
                }
                else
                {
                    var originOrigin = originProperty.GetCustomAttribute<OriginAttribute>();
                    if (originOrigin != null)
                    {
                        originDictionary.PrefixAliases(originOrigin.Join + ".Dummy", aliasPrefix, expressionSelector, extraJoins);
                        return originDictionary.OriginExpression(originProperty, originOrigin, expressionSelector, aliasPrefix, extraJoins);
                    }
                    else
                        return aliasPrefix + "." + SqlSyntax.AutoBracket(originProperty.Name);
                }
            }
        }

        public TAttr OriginAttribute<TAttr>(PropertyInfo property, OriginAttribute origin, int recursion = 0)
            where TAttr: Attribute
        {
            if (recursion++ > 1000)
                throw new DivideByZeroException("Infinite origin recursion detected!");

            var originProperty = GetOriginProperty(property.Name);
            var attr = originProperty.GetCustomAttribute(typeof(TAttr));
            if (attr != null)
                return (TAttr)attr;

            var originOrigin = originProperty.GetCustomAttribute<OriginAttribute>();
            if (originOrigin != null)
            {
                var originDictionary = GetPropertyDictionary(originProperty.ReflectedType);
                return originDictionary.OriginAttribute<TAttr>(originProperty, originOrigin);
            }

            return null;
        }

        public string OriginDisplayName(PropertyInfo property, OriginAttribute origin, int recursion = 0)
        {
            if (recursion++ > 1000)
                throw new DivideByZeroException("Infinite origin recursion detected!");

            DisplayNameAttribute attr;

            ISqlJoin join;
            string prefix = "";
            Tuple<string, ForeignKeyAttribute, ISqlJoin> propJoin;
            if (joinPropertyByAlias.TryGetValue(origin.Join, out propJoin))
            {
                if (propJoin.Item3.TitlePrefix != null)
                    prefix = propJoin.Item3.TitlePrefix;
                else
                {
                    attr = propertyByName[propJoin.Item1].GetCustomAttribute<DisplayNameAttribute>();
                    if (attr != null)
                        prefix = attr.DisplayName;
                    else
                        prefix = propJoin.Item1;
                }
            }
            else if (rowJoinByAlias.TryGetValue(origin.Join, out join) &&
                join.TitlePrefix != null)
            {
                prefix = join.TitlePrefix.Length > 0 ? join.TitlePrefix + " " : "";
            }

            Func<string, string> addPrefix = s =>
            {
                if (string.IsNullOrEmpty(prefix) || s == prefix || s.StartsWith(prefix + " "))
                    return s;

                return prefix + " " + s;
            };

            var originProperty = GetOriginProperty(property.Name);

            attr = originProperty.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
                return addPrefix(attr.DisplayName);

            var originOrigin = originProperty.GetCustomAttribute<OriginAttribute>();
            if (originOrigin != null)
            {
                var originDictionary = GetPropertyDictionary(originProperty.ReflectedType);
                return addPrefix(originDictionary.OriginDisplayName(originProperty, originOrigin));
            }

            return addPrefix(originProperty.Name);
        }

        internal string PrefixAliases(string expression, string alias, 
            DialectExpressionSelector expressionSelector, List<Attribute> extraJoins)
        {
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

                Tuple<string, ForeignKeyAttribute, ISqlJoin> propJoin;
                if (joinPropertyByAlias.TryGetValue(x, out propJoin))
                {
                    var propertyInfo = propertyByName[propJoin.Item1];
                    string leftExpression;
                    var newAlias = aliasPrefix + x;
                    var columnAttr = propertyInfo.GetCustomAttribute<ColumnAttribute>();
                    if (columnAttr != null)
                        leftExpression = alias + "." + SqlSyntax.AutoBracket(columnAttr.Name);
                    else
                    {
                        var expressionAttr = propertyInfo.GetCustomAttribute<ExpressionAttribute>();
                        if (expressionAttr != null)
                            leftExpression = mapExpression(expressionAttr.Value);
                        else
                        {
                            var origin = propertyInfo.GetCustomAttribute<OriginAttribute>();
                            if (origin != null)
                                leftExpression = OriginExpression(propertyInfo, origin, expressionSelector, alias, extraJoins);
                            else
                                leftExpression = alias + "." + SqlSyntax.AutoBracket(propertyInfo.Name);
                        }
                    }
                    
                    ISqlJoin srcJoin = propJoin.Item3;
                    var criteriax = leftExpression + " = " + newAlias + "." + SqlSyntax.AutoBracket(propJoin.Item2.Field);

                    if (srcJoin is LeftJoinAttribute)
                        srcJoin = new LeftJoinAttribute(newAlias, propJoin.Item2.Table, criteriax);
                    else if (srcJoin is InnerJoinAttribute)
                        srcJoin = new InnerJoinAttribute(newAlias, propJoin.Item2.Table, criteriax);
                    else
                        throw new ArgumentOutOfRangeException("joinType");

                    srcJoin.RowType = propJoin.Item2.RowType ?? propJoin.Item3.RowType;
                    mappedJoins[x] = srcJoin;
                    extraJoins.Add((Attribute)srcJoin);
                    return newAlias;
                }

                if (rowJoinByAlias.TryGetValue(x, out sqlJoin))
                {
                    var mappedCriteria = mapExpression(sqlJoin.OnCriteria);
                    var newAlias = aliasPrefix + x;
                    var rowType = sqlJoin.RowType;

                    var lja = sqlJoin as LeftJoinAttribute;
                    if (lja != null)
                        sqlJoin = new LeftJoinAttribute(lja.Alias, lja.ToTable, mappedCriteria);
                    else
                    {
                        var ija = sqlJoin as InnerJoinAttribute;
                        if (ija != null)
                        {
                            sqlJoin = new InnerJoinAttribute(ija.Alias, ija.ToTable, mappedCriteria);
                        }
                        else
                        {
                            var oaa = sqlJoin as OuterApplyAttribute;
                            if (oaa != null)
                                sqlJoin = new OuterApplyAttribute(ija.Alias, mappedCriteria);
                        }
                    }

                    sqlJoin.RowType = rowType;
                    mappedJoins[x] = sqlJoin;
                    extraJoins.Add((Attribute)sqlJoin);
                    return newAlias;
                }

                return x;
            };

            return mapExpression(expression);
        }
    }
}