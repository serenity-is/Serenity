namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    public partial class SqlQuery : ParameterizedQuery, IDbFilterable, ISqlSelect, IFilterableQuery
    {
        private void EnsureForeignJoin(RowFieldsBase fields, string joinAlias)
        {
            joinAlias = joinAlias.TrimToNull();

            if (joinAlias == null)
                return;

            Join join;
            if (!_joinAliases.Contains(joinAlias) &&
                fields.Joins.TryGetValue(joinAlias, out join))
            {
                if (join.ReferencedAliases != null)
                    foreach (var alias in join.ReferencedAliases)
                    {
                        if (alias != null &&
                            String.Compare(alias, joinAlias, StringComparison.OrdinalIgnoreCase) != 0)
                            EnsureForeignJoin(fields, alias);
                    }

                Join(join);
            }
        }

        public SqlQuery EnsureJoin(Join join)
        {
            EnsureForeignJoin(join.Fields, join.Name);
            return this;
        }

        public SqlQuery EnsureJoinOf(Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            if (field.ReferencedJoins == null)
                return this;

            foreach (var alias in field.ReferencedJoins)
                EnsureForeignJoin(field.Fields, alias);

            return this;
        }

        void IFilterableQuery.EnsureForeignJoin(Field field)
        {
            this.EnsureJoinOf(field);
        }

        private void EnsureJoinsInCriteria(string criteria)
        {
            if (criteria.IsEmptyOrNull())
                return;

            if (this.IntoRow == null)
                return;

            var fields = this.IntoRow.GetFields();

            string referencedJoin;
            var referencedJoins = JoinAliasLocator.LocateOptimized(criteria, out referencedJoin);
            if (referencedJoin != null)
                EnsureForeignJoin(fields, referencedJoin);

            if (referencedJoins != null)
                foreach (var alias in referencedJoins)
                    EnsureForeignJoin(fields, alias);
        }
    }
}