namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;

    public partial class SqlQuery : ParameterizedQuery, IDbFilterable, ISqlSelect, IFilterableQuery
    {
        /// <summary>
        /// Adds a table to the FROM statement. When it is called more than once, puts a comma
        /// between table names (cross join)
        /// </summary>
        /// <param name="table">Table name</param>
        /// <returns>The query itself.</returns>
        public SqlQuery From(string table)
        {
            if (table.IsEmptyOrNull())
                throw new ArgumentNullException("table");

            cachedQuery = null;
            AppendUtils.AppendWithSeparator(ref from, Consts.Comma, table);

            return this;
        }

        /// <summary>
        /// Adds a table to the FROM statement with an alias. 
        /// When it is called more than once, puts a comma between table names (cross join)
        /// </summary>
        /// <param name="table">Table</param>
        /// <param name="alias">Alias for the table</param>
        /// <returns>The query itself.</returns>
        public SqlQuery From(string table, Alias alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (joinAliases != null &&
                joinAliases.Contains(alias.Name))
                throw new ArgumentOutOfRangeException("{0} alias is used more than once in the query!");

            From(table);

            from.Append(' ');
            from.Append(alias.Name);

            if (joinAliases == null)
                joinAliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            joinAliases.Add(alias.Name);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public SqlQuery From(Alias alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (alias.Table.IsEmptyOrNull())
                throw new ArgumentNullException("alias.table");

            return From(alias.Table, alias);
        }
    }
}