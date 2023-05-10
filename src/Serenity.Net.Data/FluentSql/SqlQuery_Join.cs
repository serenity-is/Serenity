namespace Serenity.Data;

public partial class SqlQuery : QueryWithParams, IFilterableQuery, IGetExpressionByName
{
    private void JoinToString(Join join, StringBuilder sb, bool modifySelf)
    {
        sb.Append(join.GetKeyword());
        sb.Append(' ');
        sb.Append(SqlSyntax.AutoBracketValid(join.Table));

        // append if joinAlias is defined
        if (!string.IsNullOrEmpty(join.Name))
        {
            sb.Append(' ');
            sb.Append(join.Name);
        }

        if (join.OnCriteria is object &&
            !join.OnCriteria.IsEmpty)
        {
            sb.Append(" ON ");
            if (join.OnCriteria is not BinaryCriteria)
                sb.Append('(');

            if (modifySelf)
                sb.Append(join.OnCriteria.ToString(this));
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
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">join</exception>
    /// <exception cref="InvalidOperationException">Another join with different expression is already in the query.</exception>
    public SqlQuery Join(Join join)
    {
        if (join == null)
            throw new ArgumentNullException("join");

        var sb = new StringBuilder();
        JoinToString(join, sb, modifySelf: false);
        string expression = sb.ToString();

        if (!string.IsNullOrEmpty(join.Name) &&
            aliasExpressions != null && aliasExpressions.TryGetValue(join.Name, out string existingExpression))
        {
            if (expression == existingExpression)
                return this;

            throw new InvalidOperationException(string.Format("Query already has a join '{0}' with expression '{1}'. " +
                "Attempted join expression is '{2}'", join.Name, existingExpression, expression));
        }

        if (from.Length > 0)
            from.Append(" \n");

        JoinToString(join, from, modifySelf: true);

        if (!string.IsNullOrEmpty(join.Name))
        {
            AliasExpressions[join.Name] = expression;

            if (join as IHaveJoins != null)
                AliasWithJoins[join.Name] = join as IHaveJoins;
        }

        return this;
    }

    /// <summary>
    /// Adds a LEFT JOIN to the query.
    /// </summary>
    /// <param name="toTable">To table.</param>
    /// <param name="alias">The alias.</param>
    /// <param name="onCriteria">The on criteria.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    /// alias is null or alias.table is null or empty
    /// </exception>
    public SqlQuery LeftJoin(string toTable, IAlias alias, ICriteria onCriteria)
    {
        if (alias == null)
            throw new ArgumentNullException("alias");

        if (string.IsNullOrEmpty(toTable))
            throw new ArgumentNullException("alias.table");

        var join = new LeftJoin(toTable, alias.Name, onCriteria);

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
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    /// alias is null or alias.table is null or empty.
    /// </exception>
    public SqlQuery LeftJoin(IAlias alias, ICriteria onCriteria)
    {
        if (alias == null)
            throw new ArgumentNullException("alias");

        if (string.IsNullOrEmpty(alias.Table))
            throw new ArgumentNullException("alias.table");

        var join = new LeftJoin(alias.Table, alias.Name, onCriteria);

        Join(join);

        if (alias as IHaveJoins != null)
            AliasWithJoins[alias.Name] = alias as IHaveJoins;

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
        if (alias == null)
            throw new ArgumentNullException("alias");

        if (string.IsNullOrEmpty(toTable))
            throw new ArgumentNullException("alias.table");

        var join = new RightJoin(toTable, alias.Name, onCriteria);

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
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    /// alias is null
    /// or
    /// alias.table is null
    /// </exception>
    public SqlQuery RightJoin(IAlias alias, ICriteria onCriteria)
    {
        if (alias == null)
            throw new ArgumentNullException("alias");

        if (string.IsNullOrEmpty(alias.Table))
            throw new ArgumentNullException("alias.table");

        var join = new RightJoin(alias.Table, alias.Name, onCriteria);

        Join(join);

        if (alias as IHaveJoins != null)
            AliasWithJoins[alias.Name] = alias as IHaveJoins;

        return this;
    }

    /// <summary>
    /// Adds an inner join to the query.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="onCriteria">The ON criteria.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    /// alias is null 
    /// or
    /// alias.table is null
    /// </exception>
    public SqlQuery InnerJoin(IAlias alias, ICriteria onCriteria)
    {
        if (alias == null)
            throw new ArgumentNullException("alias");

        if (string.IsNullOrEmpty(alias.Table))
            throw new ArgumentNullException("alias.table");

        var join = new InnerJoin(alias.Table, alias.Name, onCriteria);

        Join(join);

        if (alias is IHaveJoins haveJoins)
            AliasWithJoins[alias.Name] = haveJoins;

        return this;
    }

    void EnsureJoin(string joinAlias)
    {
        if (aliasWithJoins == null)
            return;

        foreach (var haveJoin in aliasWithJoins)
        {
            if (haveJoin.Value is IAlias alias && haveJoin.Key == alias.Name)
            {
                if (haveJoin.Value.Joins.TryGetValue(joinAlias, out Join join))
                {
                    EnsureJoin(join);
                    break;
                }
            }
        }
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

        var referencedJoins = JoinAliasLocator.LocateOptimized(expression, out string referencedJoin);

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
    /// <exception cref="ArgumentNullException">join is null</exception>
    public SqlQuery EnsureJoin(Join join)
    {
        if (join == null)
            throw new ArgumentNullException("join");

        var joinAlias = join.Name;
        if (aliasExpressions != null && aliasExpressions.ContainsKey(joinAlias))
            return this;

        if (join.Joins != null &&
            join.ReferencedAliases != null)
            foreach (var alias in join.ReferencedAliases)
            {
                if (string.Compare(alias, joinAlias, StringComparison.OrdinalIgnoreCase) == 0)
                    continue;

                if (join.Joins.TryGetValue(alias, out Join other))
                    EnsureJoin(other);
            }

        return Join(join);
    }
}