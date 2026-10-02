namespace Serenity.Data;

public partial class SqlQuery : QueryWithParams, IFilterableQuery, IGetExpressionByName
{
    private void JoinToString(Join join, StringBuilder sb, bool modifySelf, out int autoParamCount)
    {
        autoParamCount = 0;
        sb.Append(join.GetKeyword());
        sb.Append(' ');
        sb.Append(SqlSyntax.AutoBracketValid(join.Table, Dialect()));

        // append if joinAlias is defined
        if (!string.IsNullOrEmpty(join.Name))
        {
            sb.Append(' ');
            sb.Append(join.Name);

            if (!string.IsNullOrEmpty(join.TableHint))
            {
                sb.Append(" WITH(");
                sb.Append(join.TableHint);
                sb.Append(')');
            }
        }

        if (join.OnCriteria is object &&
            !join.OnCriteria.IsEmpty)
        {
            sb.Append(" ON ");
            if (join.OnCriteria is not BinaryCriteria)
                sb.Append('(');

            if (modifySelf)
                sb.Append(join.OnCriteria.ToString(this));
            else if (join.OnCriteria is BaseCriteria baseCriteria)
                sb.Append(baseCriteria.ToStringIgnoreParams(out autoParamCount));
            else
                sb.Append(join.OnCriteria.ToStringIgnoreParams());

            if (join.OnCriteria is not BinaryCriteria)
                sb.Append(')');
        }
    }

    /// <summary>
    /// Joins the specified join.
    /// </summary>
    /// <param name="join">The join.</param>
    /// <returns>The query itself.</returns>
    /// <exception cref="ArgumentNullException">join is null.</exception>
    /// <exception cref="InvalidOperationException">Another join with different expression is already in the query.</exception>
    public SqlQuery Join(Join join)
    {
        ArgumentNullException.ThrowIfNull(join);

        BeforeModify();

        var sb = new StringBuilder();
        JoinToString(join, sb, modifySelf: false, out int autoParamCount);
        string expression = sb.ToString();

        if (!string.IsNullOrEmpty(join.Name) &&
            GetAliasExpression(join.Name) is string existingExpression)
        {
            // The alias is already registered: either an innocent re-join of
            // the identical join (e.g. explicitly joining and also selecting
            // a field that auto-ensures it), which is gracefully skipped, or
            // a conflicting one, which throws. The compared texts are rendered
            // ignoring params, where every auto value becomes a deterministic
            // @pN placeholder — so text equality is only trustworthy when no
            // auto params were generated. Manually named parameters render
            // literally and stay comparable.
            if (autoParamCount == 0 && expression == existingExpression)
                return this;

            throw new InvalidOperationException(string.Format("Query already has a join '{0}' with expression '{1}'. " +
                "Attempted join expression is '{2}'" +
                (autoParamCount > 0 ? ". The attempted join contains parameters, so its equality cannot be verified." : ""),
                join.Name, existingExpression, expression));
        }

        if (from.Length > 0)
            from.Append(" \n");

        JoinToString(join, from, modifySelf: true, out _);

        if (!string.IsNullOrEmpty(join.Name))
        {
            SetAliasExpression(join.Name, expression);

            if (join is IHaveJoins haveJoins)
                AliasWithJoins[join.Name] = haveJoins;
        }

        return this;
    }

    /// <summary>
    /// Adds a LEFT JOIN to the query.
    /// </summary>
    /// <param name="toTable">To table.</param>
    /// <param name="alias">The alias.</param>
    /// <param name="onCriteria">The on criteria.</param>
    /// <returns>The query itself.</returns>
    /// <exception cref="ArgumentNullException">
    /// alias is null or alias.table is null or empty
    /// </exception>
    public SqlQuery LeftJoin(string toTable, IAlias alias, ICriteria onCriteria)
    {
        ArgumentNullException.ThrowIfNull(alias);

        if (string.IsNullOrEmpty(toTable))
            throw new ArgumentNullException("alias.table");

        BeforeModify();

        var join = new LeftJoin(toTable, alias.Name, onCriteria)
        {
            TableHint = AliasExtensions.GetTableHint(alias)
        };

        Join(join);

        if (alias is IHaveJoins haveJoins)
            AliasWithJoins[alias.Name] = haveJoins;

        return this;
    }

    /// <summary>
    /// Adds a LEFT JOIN to the query
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="onCriteria">The on criteria.</param>
    /// <returns>The query itself.</returns>
    /// <exception cref="ArgumentNullException">
    /// alias is null or alias.table is null or empty.
    /// </exception>
    public SqlQuery LeftJoin(IAlias alias, ICriteria onCriteria)
    {
        ArgumentNullException.ThrowIfNull(alias);

        if (string.IsNullOrEmpty(alias.Table))
            throw new ArgumentNullException("alias.table");

        BeforeModify();

        var join = new LeftJoin(alias.Table, alias.Name, onCriteria)
        {
            TableHint = AliasExtensions.GetTableHint(alias)
        };

        Join(join);

        if (alias is IHaveJoins haveJoins)
            AliasWithJoins[alias.Name] = haveJoins;

        return this;
    }

    /// <summary>
    /// Adds a right join to the query.
    /// </summary>
    /// <param name="toTable">Right join to table.</param>
    /// <param name="alias">The alias.</param>
    /// <param name="onCriteria">The on criteria.</param>
    /// <returns>SqlQuery itself.</returns>
    /// <exception cref="ArgumentNullException">
    /// alias is null
    /// or
    /// alias.table is null
    /// </exception>
    public SqlQuery RightJoin(string toTable, IAlias alias, ICriteria onCriteria)
    {
        ArgumentNullException.ThrowIfNull(alias);

        if (string.IsNullOrEmpty(toTable))
            throw new ArgumentNullException("alias.table");

        BeforeModify();

        var join = new RightJoin(toTable, alias.Name, onCriteria)
        {
            TableHint = AliasExtensions.GetTableHint(alias)
        };

        Join(join);

        if (alias is IHaveJoins haveJoins)
            AliasWithJoins[alias.Name] = haveJoins;

        return this;
    }

    /// <summary>
    /// Adds a right join to the query.
    /// </summary>
    /// <param name="alias">The alias with table name/alias name.</param>
    /// <param name="onCriteria">The ON criteria.</param>
    /// <returns>The query itself.</returns>
    /// <exception cref="ArgumentNullException">
    /// alias is null
    /// or
    /// alias.table is null
    /// </exception>
    public SqlQuery RightJoin(IAlias alias, ICriteria onCriteria)
    {
        ArgumentNullException.ThrowIfNull(alias);

        if (string.IsNullOrEmpty(alias.Table))
            throw new ArgumentNullException("alias.table");

        BeforeModify();

        var join = new RightJoin(alias.Table, alias.Name, onCriteria)
        {
            TableHint = AliasExtensions.GetTableHint(alias)
        };

        Join(join);

        if (alias is IHaveJoins haveJoins)
            AliasWithJoins[alias.Name] = haveJoins;

        return this;
    }

    /// <summary>
    /// Adds an inner join to the query.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="onCriteria">The ON criteria.</param>
    /// <returns>The query itself.</returns>
    /// <exception cref="ArgumentNullException">
    /// alias is null 
    /// or
    /// alias.table is null
    /// </exception>
    public SqlQuery InnerJoin(IAlias alias, ICriteria onCriteria)
    {
        ArgumentNullException.ThrowIfNull(alias);

        if (string.IsNullOrEmpty(alias.Table))
            throw new ArgumentNullException("alias.table");

        BeforeModify();

        var join = new InnerJoin(alias.Table, alias.Name, onCriteria)
        {
            TableHint = AliasExtensions.GetTableHint(alias)
        };

        Join(join);

        if (alias is IHaveJoins haveJoins)
            AliasWithJoins[alias.Name] = haveJoins;

        return this;
    }

    void EnsureJoin(string joinAlias)
    {
        if (TryFindJoin(joinAlias, out var owner, out var join))
            owner.EnsureJoin(join);
    }

    private bool TryFindJoin(string joinAlias, out SqlQuery owner, out Join join)
    {
        for (SqlQuery? current = this; current is not null;)
        {
            if (current.aliasWithJoins is null)
            {
                current = GetJoinSourceParent(current);
                continue;
            }

            foreach (var haveJoin in current.aliasWithJoins)
            {
                if (haveJoin.Value is IAlias alias &&
                    string.Equals(haveJoin.Key, alias.Name, StringComparison.OrdinalIgnoreCase) &&
                    TryGetJoin(haveJoin.Value.Joins, joinAlias, out var found) && found is not null)
                {
                    owner = current;
                    join = found;
                    return true;
                }
            }

            current = GetJoinSourceParent(current);
        }

        owner = null!;
        join = null!;
        return false;
    }

    private static SqlQuery? GetJoinSourceParent(SqlQuery query)
    {
        if (query.parent is not SqlQuery parentQuery)
            return null;

        // UNION legs share the parameter/cache parent, but are separate SQL
        // scopes. Skip the other leg while still allowing a UNION subquery to
        // continue searching its actual outer query ancestors.
        return ReferenceEquals(parentQuery.unionQuery, query)
            ? parentQuery.parent as SqlQuery
            : parentQuery;
    }

    private static bool TryGetJoin(IDictionary<string, Join> joins, string joinAlias, out Join? join)
    {
        if (joins.TryGetValue(joinAlias, out join))
            return true;

        foreach (var pair in joins)
        {
            if (string.Equals(pair.Key, joinAlias, StringComparison.OrdinalIgnoreCase))
            {
                join = pair.Value;
                return true;
            }
        }

        join = null;
        return false;
    }

    /// <summary>
    /// Ensures the joins in expression. For this to work, into row must provide
    /// a list of joins and their expressions.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>The query itself.</returns>
    public SqlQuery EnsureJoinsInExpression(string expression)
    {
        if (string.IsNullOrEmpty(expression))
            return this;

        BeforeModify();

        var referencedJoins = JoinAliasLocator.LocateOptimized(expression, out string? referencedJoin);

        if (referencedJoin != null)
            EnsureJoin(referencedJoin);

        if (referencedJoins != null)
            foreach (var alias in referencedJoins)
                EnsureJoin(alias);

        return this;
    }

    /// <summary>
    /// Ensures the join.
    /// </summary>
    /// <param name="join">The join.</param>
    /// <returns>The query itself.</returns>
    /// <exception cref="ArgumentNullException">join is null.</exception>
    public SqlQuery EnsureJoin(Join join)
    {
        ArgumentNullException.ThrowIfNull(join);

        BeforeModify();

        var joinAlias = join.Name;
        if (GetAliasExpression(joinAlias) is not null)
            return this;

        if (join.ReferencedAliases != null)
            foreach (var alias in join.ReferencedAliases)
            {
                if (string.Compare(alias, joinAlias, StringComparison.OrdinalIgnoreCase) == 0)
                    continue;

                if (join.Joins is not null && TryGetJoin(join.Joins, alias, out var other) && other is not null)
                    EnsureJoin(other);
                else
                    EnsureJoin(alias);
            }

        return Join(join);
    }
}
