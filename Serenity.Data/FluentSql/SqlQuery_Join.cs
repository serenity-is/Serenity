namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public partial class SqlQuery : QueryWithParams, IFilterableQuery, IGetExpressionByName
    {
        private void JoinToString(Join join, StringBuilder sb, bool modifySelf)
        {
            sb.Append(join.GetKeyword());
            sb.Append(' ');
            sb.Append(join.Table);

            // joinAlias belirtilmişse ekle
            if (!string.IsNullOrEmpty(join.Name))
            {
                sb.Append(' ');
                sb.Append(join.Name);
            }

            if (!ReferenceEquals(null, join.OnCriteria) &&
                !join.OnCriteria.IsEmpty)
            {
                sb.Append(" ON (");
                
                if (modifySelf)
                    sb.Append(join.OnCriteria.ToString(this));
                else
                    sb.Append(join.OnCriteria.ToStringIgnoreParams());

                sb.Append(')');
            }
        }

        public SqlQuery Join(Join join)
        {
            if (join == null)
                throw new ArgumentNullException("join");

            var sb = new StringBuilder();
            JoinToString(join, sb, modifySelf: false);
            string expression = sb.ToString();
            string existingExpression;

            if (!string.IsNullOrEmpty(join.Name) &&
                aliases != null && aliases.TryGetValue(join.Name, out existingExpression))
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
                ((ISqlQueryExtensible)this).Aliases[join.Name] = expression;

            return this;
        }

        public SqlQuery LeftJoin(string toTable, Alias alias, ICriteria onCriteria)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (string.IsNullOrEmpty(toTable))
                throw new ArgumentNullException("alias.table");

            var join = new LeftJoin(toTable, alias.Name, onCriteria);

            Join(join);

            return this;
        }

        public SqlQuery LeftJoin(Alias alias, ICriteria onCriteria)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (string.IsNullOrEmpty(alias.Table))
                throw new ArgumentNullException("alias.table");

            var join = new LeftJoin(alias.Table, alias.Name, onCriteria);

            Join(join);

            return this;
        }

        public SqlQuery RightJoin(string toTable, Alias alias, ICriteria onCriteria)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (string.IsNullOrEmpty(toTable))
                throw new ArgumentNullException("alias.table");

            var join = new RightJoin(toTable, alias.Name, onCriteria);

            Join(join);

            return this;
        }

        public SqlQuery RightJoin(Alias alias, ICriteria onCriteria)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (string.IsNullOrEmpty(alias.Table))
                throw new ArgumentNullException("alias.table");

            var join = new RightJoin(alias.Table, alias.Name, onCriteria);

            Join(join);

            return this;
        }

        public SqlQuery InnerJoin(Alias alias, ICriteria onCriteria)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (string.IsNullOrEmpty(alias.Table))
                throw new ArgumentNullException("alias.table");

            var join = new InnerJoin(alias.Table, alias.Name, onCriteria);

            Join(join);

            return this;
        }
    }
}