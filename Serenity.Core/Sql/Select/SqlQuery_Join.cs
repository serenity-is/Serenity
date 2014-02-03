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

            _cachedQuery = null;

            AppendUtils.AppendWithSeparator(ref _from, " \n", join.GetKeyword());

            _from.Append(' ');
            _from.Append(join.Table);

            // joinAlias belirtilmişse ekle
            if (!join.Name.IsEmptyOrNull())
            {
                _from.Append(' ');
                _from.Append(join.Name);

                if (_joinAliases == null)
                    _joinAliases = new HashSet<string>();

                _joinAliases.Add(join.Name);
            }

            if (!Object.ReferenceEquals(null, join.OnCriteria) &&
                !join.OnCriteria.IsEmpty)
            {
                _from.Append(" ON (");
                _from.Append(join.OnCriteria.ToString(this));
                _from.Append(')');
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