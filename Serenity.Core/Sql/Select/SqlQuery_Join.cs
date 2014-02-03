namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    public partial class SqlQuery : ParameterizedQuery, IDbFilterable, ISqlSelect, IFilterableQuery
    {
        public SqlQuery Join(Join join)
        {
            if (join == null)
                throw new ArgumentNullException("join");

            cachedQuery = null;

            AppendUtils.AppendWithSeparator(ref from, " \n", join.GetKeyword());

            from.Append(' ');
            from.Append(join.Table);

            // joinAlias belirtilmişse ekle
            if (!join.Name.IsEmptyOrNull())
            {
                from.Append(' ');
                from.Append(join.Name);

                if (joinAliases == null)
                    joinAliases = new HashSet<string>();

                joinAliases.Add(join.Name);
            }

            if (!Object.ReferenceEquals(null, join.OnCriteria) &&
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

            if (toTable.IsEmptyOrNull())
                throw new ArgumentNullException("alias.table");

            var join = new LeftJoin(toTable, alias.Name, onCriteria);

            Join(join);

            return this;
        }

        public SqlQuery LeftJoin(Alias alias, BaseCriteria onCriteria)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (alias.Table.IsEmptyOrNull())
                throw new ArgumentNullException("alias.table");

            var join = new LeftJoin(alias.Table, alias.Name, onCriteria);

            Join(join);

            return this;
        }
    }
}