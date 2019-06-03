namespace Serenity.Data
{
    using System;
    using System.Text;

    public partial class SqlQuery : QueryWithParams, IFilterableQuery, IGetExpressionByName
    {
        private void JoinToString(Join join, StringBuilder sb, bool modifySelf)
        {
            sb.Append(join.GetKeyword());
            sb.Append(' ');
            sb.Append(SqlSyntax.AutoBracketValid(join.Table));

            // joinAlias belirtilmişse ekle
            if (!string.IsNullOrEmpty(join.Name))
            {
                sb.Append(' ');
                sb.Append(join.Name);
            }

            if (!ReferenceEquals(null, join.OnCriteria) &&
                !join.OnCriteria.IsEmpty)
            {
                sb.Append(" ON ");
                if (!(join.OnCriteria is BinaryCriteria))
                    sb.Append('(');
                
                if (modifySelf)
                    sb.Append(join.OnCriteria.ToString(this));
                else
                    sb.Append(join.OnCriteria.ToStringIgnoreParams());

                if (!(join.OnCriteria is BinaryCriteria))
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
            string existingExpression;

            if (!string.IsNullOrEmpty(join.Name) &&
                aliasExpressions != null && aliasExpressions.TryGetValue(join.Name, out existingExpression))
            {
                if (expression == existingExpression)
                    return this;

                throw new InvalidOperationException(String.Format("Query already has a join '{0}' with expression '{1}'. " +
                    "Attempted join expression is '{2}'", join.Name, existingExpression, expression));
            }

            if (from.Length > 0)
                from.Append(" \n");

            JoinToString(join, from, modifySelf: true);

            if (!string.IsNullOrEmpty(join.Name))
            {
                AliasExpressions[join.Name] = expression;

                var haveJoins = join as IHaveJoins;
                if (haveJoins != null)
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

            var haveJoins = alias as IHaveJoins;
            if (haveJoins != null)
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

            var haveJoins = alias as IHaveJoins;
            if (haveJoins != null)
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
        /// <exception cref="System.ArgumentNullException">
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

            var haveJoins = alias as IHaveJoins;
            if (haveJoins != null)
                AliasWithJoins[alias.Name] = haveJoins;

            return this;
        }

        /// <summary>
        /// Adds a right join to the query.
        /// </summary>
        /// <param name="alias">The alias with tablename/alias name.</param>
        /// <param name="onCriteria">The ON criteria.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
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

            var haveJoins = alias as IHaveJoins;
            if (haveJoins != null)
                AliasWithJoins[alias.Name] = haveJoins;

            return this;
        }

        /// <summary>
        /// Adds an inner join to the query.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="onCriteria">The ON criteria.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
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

            var haveJoins = alias as IHaveJoins;
            if (haveJoins != null)
                AliasWithJoins[alias.Name] = haveJoins;

            return this;
        }

        void EnsureJoin(string joinAlias)
        {
            Join join;
            if (aliasWithJoins == null)
                return;

            foreach (var haveJoin in aliasWithJoins)
            {
                var alias = haveJoin.Value as IAlias;
                if (alias != null && haveJoin.Key == alias.Name)
                {
                    if (haveJoin.Value.Joins.TryGetValue(joinAlias, out join))
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

            string referencedJoin;
            var referencedJoins = JoinAliasLocator.LocateOptimized(expression, out referencedJoin);

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
        /// <exception cref="System.ArgumentNullException">join is null</exception>
        public SqlQuery EnsureJoin(Join join)
        {
            if (join == null)
                throw new ArgumentNullException("join");

            var ext = (ISqlQueryExtensible)this;

            var joinAlias = join.Name;
            if (aliasExpressions != null && aliasExpressions.ContainsKey(joinAlias))
                return this;

            if (join.Joins != null &&
                join.ReferencedAliases != null)
                foreach (var alias in join.ReferencedAliases)
                {
                    if (String.Compare(alias, joinAlias, StringComparison.OrdinalIgnoreCase) == 0)
                        continue;

                    Join other;
                    if (join.Joins.TryGetValue(alias, out other))
                        EnsureJoin(other);
                }

            return Join(join);
        }
    }
}