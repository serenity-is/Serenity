namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;

    public partial class SqlQuery : QueryWithParams, IFilterableQuery, IGetExpressionByName
    {
        public SqlQuery Join(Join join)
        {
            if (join == null)
                throw new ArgumentNullException("join");

            if (from.Length > 0)
                from.Append(" \n");

            from.Append(join.GetKeyword());
            from.Append(' ');
            from.Append(join.Table);

            // joinAlias belirtilmişse ekle
            if (!join.Name.IsNullOrEmpty())
            {
                from.Append(' ');
                from.Append(join.Name);

                if (aliases == null)
                    aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                aliases.Add(join.Name);
            }

            if (!ReferenceEquals(null, join.OnCriteria) &&
                !join.OnCriteria.IsEmpty)
            {
                from.Append(" ON (");
                from.Append(join.OnCriteria.ToString(this));
                from.Append(')');
            }

            return this;
        }

        public SqlQuery LeftJoin(string toTable, Alias alias, BaseCriteria onCriteria)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (toTable.IsNullOrEmpty())
                throw new ArgumentNullException("alias.table");

            var join = new LeftJoin(toTable, alias.Name, onCriteria);

            Join(join);

            return this;
        }

        public SqlQuery LeftJoin(Alias alias, BaseCriteria onCriteria)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (alias.Table.IsNullOrEmpty())
                throw new ArgumentNullException("alias.table");

            var join = new LeftJoin(alias.Table, alias.Name, onCriteria);

            Join(join);

            return this;
        }

        public SqlQuery InnerJoin(Alias alias, BaseCriteria onCriteria)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (alias.Table.IsNullOrEmpty())
                throw new ArgumentNullException("alias.table");

            var join = new InnerJoin(alias.Table, alias.Name, onCriteria);

            Join(join);

            return this;
        }
    }
}