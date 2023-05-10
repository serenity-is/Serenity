namespace Serenity.Data;

internal class OriginPropertyDictionary
{
    internal Type rowType;
    internal Dictionary<string, PropertyInfo> propertyByName;
    internal Dictionary<string, (string propertyName, ForeignKeyAttribute[] foreignKeys, ISqlJoin join)> joinPropertyByAlias;
    internal Dictionary<string, ISqlJoin> joinPropertyByName;
    internal Dictionary<string, ISqlJoin> rowJoinByAlias;
    internal Dictionary<string, OriginAttribute> originAttrByPropertyName;
    internal Dictionary<string, Tuple<PropertyInfo, Type>> originPropertyByName;
    internal ILookup<string, KeyValuePair<string, OriginAttribute>> originByAlias;
    internal IDictionary<string, string> prefixByAlias;

    internal static ConcurrentDictionary<Type, OriginPropertyDictionary> cache = new();

    public OriginPropertyDictionary(Type rowType)
    {
        this.rowType = rowType;
        rowJoinByAlias = new Dictionary<string, ISqlJoin>(StringComparer.OrdinalIgnoreCase);

        propertyByName = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (var pi in rowType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            propertyByName[pi.Name] = pi;

        originAttrByPropertyName = new Dictionary<string, OriginAttribute>(StringComparer.OrdinalIgnoreCase);

        joinPropertyByAlias = new(StringComparer.OrdinalIgnoreCase);
        joinPropertyByName = new(StringComparer.Ordinal);
        foreach (var property in propertyByName.Values)
        {
            var originAttr = property.GetCustomAttribute<OriginAttribute>();
            if (originAttr != null)
                originAttrByPropertyName[property.Name] = originAttr;

            var lj = property.GetCustomAttribute<LeftJoinAttribute>();
            var fk = property.GetCustomAttributes<ForeignKeyAttribute>().ToArray();
            if (lj != null && fk != null)
            {
                joinPropertyByAlias[lj.Alias] = new(property.Name, fk, lj);
                joinPropertyByName[property.Name] = lj;
            }
        }

        foreach (var attr in rowType.GetCustomAttributes().OfType<ISqlJoin>())
            rowJoinByAlias[attr.Alias] = attr;

        originByAlias = originAttrByPropertyName.ToLookup(x => GetJoinAlias(x.Value), StringComparer.OrdinalIgnoreCase);
        prefixByAlias = originByAlias.ToDictionary(x => x.Key, x =>
        {
            if (joinPropertyByAlias.TryGetValue(x.Key, out var joinProperty))
            {
                if (joinProperty.join.PropertyPrefix != null)
                    return joinProperty.join.PropertyPrefix;

                if (joinProperty.propertyName.EndsWith("ID") ||
                    joinProperty.propertyName.EndsWith("Id"))
                    return joinProperty.propertyName[0..^2];
            }

            if (rowJoinByAlias.TryGetValue(x.Key, out ISqlJoin join))
            {
                if (join.PropertyPrefix != null)
                    return join.PropertyPrefix;
            }

            return DeterminePrefix(x.Select(z => z.Key));
        });
    }

    private string GetJoinAlias(OriginAttribute attr)
    {
        var join = attr?.Join;
        if (!string.IsNullOrEmpty(join) &&
            !rowJoinByAlias.ContainsKey(join) &&
            !joinPropertyByAlias.ContainsKey(join) &&
            joinPropertyByName.TryGetValue(join, out ISqlJoin s))
            return s.Alias;

        return join;
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
                prefix = prefix[..current.Length];

            while (!current.StartsWith(prefix) && prefix.Length > 0)
                prefix = prefix[0..^1];
        }

        return prefix;
    }

    public static OriginPropertyDictionary GetPropertyDictionary(Type rowType)
    {
        if (!cache.TryGetValue(rowType, out OriginPropertyDictionary dictionary))
        {
            dictionary = new OriginPropertyDictionary(rowType);
            cache[rowType] = dictionary;
        }
        return dictionary;
    }

    private Tuple<PropertyInfo, Type> GetOriginProperty(string propertyName, DialectExpressionSelector expressionSelector)
    {
        Type originType;
        Tuple<PropertyInfo, Type> pi;
        var d = originPropertyByName;
        if (d == null)
        {
            d = new Dictionary<string, Tuple<PropertyInfo, Type>>
            {
                [propertyName] = pi = new Tuple<PropertyInfo, Type>(
                    GetOriginProperty(propertyByName[propertyName],
                        originAttrByPropertyName[propertyName], expressionSelector, 
                        out originType), originType)
            };
            originPropertyByName = d;
        }
        else if (!d.TryGetValue(propertyName, out pi))
        {
            d = new Dictionary<string, Tuple<PropertyInfo, Type>>
            {
                [propertyName] = pi = new Tuple<PropertyInfo, Type>(
                    GetOriginProperty(propertyByName[propertyName],
                        originAttrByPropertyName[propertyName], expressionSelector, 
                        out originType), originType)
            };
            originPropertyByName = d;
        }

        return pi;
    }

    private PropertyInfo GetOriginProperty(PropertyInfo property, OriginAttribute origin,
        DialectExpressionSelector expressionSelector, out Type originRowType)
    {
        var joinAlias = GetJoinAlias(origin);

        if (joinPropertyByAlias.TryGetValue(joinAlias, out var joinProperty))
        {
            var joinPropertyName = joinProperty.propertyName;
            var fk = expressionSelector.GetBestMatch(joinProperty.foreignKeys, x => x.Dialect);
            var lj = joinProperty.join;
            originRowType = lj.RowType ?? fk.RowType;

            if (originRowType == null)
            {
                throw new ArgumentOutOfRangeException("origin", string.Format(
                    "Property '{0}' on row type '{1}' has a [Origin] attribute, " +
                    "but [ForeignKey] and [LeftJoin] attributes on related join " +
                    "property '{2}' doesn't use a typeof(SomeRow)!",
                        property.Name, rowType.Name, joinPropertyName));
            }
        }
        else if (rowJoinByAlias.TryGetValue(joinAlias, out ISqlJoin rowJoin))
        {
            originRowType = rowJoin.RowType;
            if (originRowType == null)
            {
                throw new ArgumentOutOfRangeException("origin", string.Format(
                    "Property '{0}' on row type '{1}' has a [Origin] attribute, " +
                    "but related join declaration on row has no RowType!",
                        property.Name, rowType.Name, joinAlias));
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException("origin", string.Format(
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
            var prefix = prefixByAlias[GetJoinAlias(origin)];
            if (prefix != null &&
                prefix.Length > 0 &&
                property.Name.StartsWith(prefix) &&
                property.Name.Length > prefix.Length &&
                originDictionary.propertyByName.TryGetValue(
                    property.Name[prefix.Length..], out originProperty))
            {
                originPropertyName = originProperty.Name;
            }
            else
                originPropertyName = property.Name;
        }

        if (originProperty == null &&
            !originDictionary.propertyByName.TryGetValue(originPropertyName, out originProperty))
        {
            throw new ArgumentOutOfRangeException("origin", string.Format(
                "Property '{0}' on row type '{1}' has a [Origin] attribute, " +
                "but its corresponding property '{2}' on row type '{3}' is not found!",
                    property.Name, rowType.Name, originPropertyName, originRowType.Name));
        }

        return originProperty;
    }

    public string OriginExpression(string propertyName, OriginAttribute origin,
        DialectExpressionSelector expressionSelector, string aliasPrefix, List<Attribute> extraJoins)
    {
        if (aliasPrefix.Length >= 1000)
            throw new DivideByZeroException("Infinite origin recursion detected!");

        var org = GetOriginProperty(propertyName, expressionSelector);
        var originProperty = org.Item1;

        var joinAlias = GetJoinAlias(origin);
        if (aliasPrefix.Length == 0)
            aliasPrefix = joinAlias;
        else
            aliasPrefix = aliasPrefix + "_" + joinAlias;

        var columnAttr = originProperty.GetCustomAttribute<ColumnAttribute>();
        if (columnAttr != null)
            return aliasPrefix + "." + SqlSyntax.AutoBracket(columnAttr.Name);
        else
        {
            var originDictionary = GetPropertyDictionary(org.Item2);

            var expressionAttr = originProperty.GetCustomAttributes<BaseExpressionAttribute>();
            if (expressionAttr.Any())
            {
                var expression = expressionSelector.GetBestMatch(expressionAttr, 
                    x => x is ExpressionAttribute exp ? exp.Dialect : null);
                return originDictionary.PrefixAliases(expression.ToString(expressionSelector.Dialect), aliasPrefix,
                    expressionSelector, extraJoins);
            }
            else
            {
                var originOrigin = originProperty.GetCustomAttribute<OriginAttribute>();
                if (originOrigin != null)
                {
                    originDictionary.PrefixAliases(GetJoinAlias(originOrigin) + ".Dummy", aliasPrefix, expressionSelector, extraJoins);
                    return originDictionary.OriginExpression(originProperty.Name, originOrigin, expressionSelector, aliasPrefix, extraJoins);
                }
                else
                    return aliasPrefix + "." + SqlSyntax.AutoBracket(originProperty.Name);
            }
        }
    }

    public TAttr OriginAttribute<TAttr>(string propertyName, 
        DialectExpressionSelector expressionSelector, int recursion = 0)
        where TAttr : Attribute
    {
        if (recursion++ > 1000)
            throw new DivideByZeroException("Infinite origin recursion detected!");

        var org = GetOriginProperty(propertyName, expressionSelector);
        var originProperty = org.Item1;

        var attr = originProperty.GetCustomAttribute(typeof(TAttr));
        if (attr != null)
            return (TAttr)attr;

        var originOrigin = originProperty.GetCustomAttribute<OriginAttribute>();
        if (originOrigin != null)
        {
            var originDictionary = GetPropertyDictionary(org.Item2);
            return originDictionary.OriginAttribute<TAttr>(originProperty.Name, expressionSelector, recursion + 1);
        }

        return null;
    }

    public string OriginDisplayName(string propertyName, OriginAttribute origin, 
        DialectExpressionSelector expressionSelector, int recursion = 0)
    {
        if (recursion++ > 1000)
            throw new DivideByZeroException("Infinite origin recursion detected!");

        DisplayNameAttribute attr;
        string prefix = "";
        var joinAlias = GetJoinAlias(origin);

        if (joinPropertyByAlias.TryGetValue(joinAlias, out var propJoin))
        {
            if (propJoin.join.TitlePrefix != null)
                prefix = propJoin.join.TitlePrefix;
            else
            {
                attr = propertyByName[propJoin.propertyName].GetCustomAttribute<DisplayNameAttribute>();
                if (attr != null)
                    prefix = attr.DisplayName;
                else
                    prefix = propJoin.propertyName;
            }
        }
        else if (rowJoinByAlias.TryGetValue(joinAlias, out ISqlJoin join) &&
            join.TitlePrefix != null)
        {
            prefix = join.TitlePrefix.Length > 0 ? join.TitlePrefix + " " : "";
        }

        string addPrefix(string s)
        {
            if (string.IsNullOrEmpty(prefix) || s == prefix || s.StartsWith(prefix + " "))
                return s;

            return prefix + " " + s;
        }

        var org = GetOriginProperty(propertyName, expressionSelector);
        var originProperty = org.Item1;

        attr = originProperty.GetCustomAttribute<DisplayNameAttribute>();
        if (attr != null)
            return addPrefix(attr.DisplayName);

        var originOrigin = originProperty.GetCustomAttribute<OriginAttribute>();
        if (originOrigin != null)
        {
            var originDictionary = GetPropertyDictionary(org.Item2);
            return addPrefix(originDictionary.OriginDisplayName(originProperty.Name, originOrigin, expressionSelector));
        }

        return addPrefix(originProperty.Name);
    }

    internal string PrefixAliases(string expression, string alias,
        DialectExpressionSelector expressionSelector, List<Attribute> extraJoins)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return expression;

        if (string.IsNullOrWhiteSpace(alias))
            throw new ArgumentNullException(nameof(alias));

        var aliasPrefix = alias + "_";

        var mappedJoins = new Dictionary<string, ISqlJoin>();

        Func<string, string> mapAlias = null;

        string mapExpression(string x)
        {
            if (x == null)
                return null;

            return JoinAliasLocator.ReplaceAliases(x, mapAlias);
        }

        mapAlias = x =>
        {
            if (x == "t0" || x == "T0")
                return alias;

            if (mappedJoins.TryGetValue(x, out ISqlJoin sqlJoin))
                return sqlJoin.Alias;

            if (joinPropertyByAlias.TryGetValue(x, out var propJoin))
            {
                var propertyInfo = propertyByName[propJoin.propertyName];
                string leftExpression;
                var newAlias = aliasPrefix + x;
                var columnAttr = propertyInfo.GetCustomAttribute<ColumnAttribute>();
                if (columnAttr != null)
                    leftExpression = alias + "." + SqlSyntax.AutoBracket(columnAttr.Name);
                else
                {
                    var expressionAttr = propertyInfo.GetCustomAttributes<BaseExpressionAttribute>();
                    var expression = expressionSelector.GetBestMatch(expressionAttr,
                        x => x is ExpressionAttribute exp ? exp.Dialect : null);
                    if (expression != null)
                        leftExpression = mapExpression(expression.ToString(expressionSelector.Dialect));
                    else
                    {
                        var origin = propertyInfo.GetCustomAttribute<OriginAttribute>();
                        if (origin != null)
                            leftExpression = OriginExpression(propertyInfo.Name, origin, expressionSelector, alias, extraJoins);
                        else
                            leftExpression = alias + "." + SqlSyntax.AutoBracket(propertyInfo.Name);
                    }
                }

                ISqlJoin srcJoin = propJoin.join;
                var fkAttr = expressionSelector.GetBestMatch(propJoin.foreignKeys, x => x.Dialect);
                var criteriax = leftExpression + " = " + newAlias + "." + SqlSyntax.AutoBracket(fkAttr.Field);

                var frgTable = fkAttr.Table ??
                    expressionSelector.GetBestMatch(fkAttr.RowType
                        .GetCustomAttributes<TableNameAttribute>(), x => x.Dialect).Name;

                if (srcJoin is LeftJoinAttribute)
                    srcJoin = new LeftJoinAttribute(newAlias, frgTable, criteriax);
                else if (srcJoin is InnerJoinAttribute)
                    srcJoin = new InnerJoinAttribute(newAlias, frgTable, criteriax);
                else
                    throw new ArgumentOutOfRangeException("joinType");

                srcJoin.RowType = fkAttr.RowType ?? propJoin.join.RowType;
                mappedJoins[x] = srcJoin;
                extraJoins.Add((Attribute)srcJoin);
                return newAlias;
            }

            if (rowJoinByAlias.TryGetValue(x, out sqlJoin))
            {
                var mappedCriteria = mapExpression(sqlJoin.OnCriteria);
                var newAlias = aliasPrefix + x;
                var rowType = sqlJoin.RowType;

                if (sqlJoin is LeftJoinAttribute lja)
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
                        if (sqlJoin is OuterApplyAttribute oaa)
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