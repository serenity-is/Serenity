namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;

    public partial class SqlQuery : ParameterizedQuery, IDbFilterable, ISqlSelect, IFilterableQuery
    {
        /// <summary>
        /// Tabloyu FROM listesine ekler. Birden fazla kez çağrıldığında tablo isimlerinin
        /// arasına virgül konur (CROSS JOIN).
        /// </summary>
        /// <param name="table">Tablo adı</param>
        /// <returns>Sorgunun kendisi</returns>
        public SqlQuery From(string table)
        {
            if (table.IsEmptyOrNull())
                throw new ArgumentNullException("table");

            _cachedQuery = null;
            AppendUtils.AppendWithSeparator(ref _from, Consts.Comma, table);

            if (_mainTableName == null)
                _mainTableName = table;

            return this;
        }

        /// <summary>
        /// Tabloyu FROM listesine, bir alias atayarak ekler.
        /// </summary>
        /// <param name="table">Tablo adı</param>
        /// <param name="alias">Tabloya atanacak kısa ad (alias)</param>
        /// <returns>Sorgunun kendisi</returns>
        public SqlQuery FromAs(string table, Alias alias)
        {
            if (alias == null)
                throw new ArgumentNullException("tableAlias");

            if (_joinAliases != null &&
                _joinAliases.Contains(alias.Name))
                throw new ArgumentOutOfRangeException("{0} alias'ı sorguda ikinci kez kullanılmaya çalışıldı!");

            From(table);

            _from.Append(' ');
            _from.Append(alias.Name);

            if (_joinAliases == null)
                _joinAliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            _joinAliases.Add(alias.Name);

            return this;
        }

        public SqlQuery From(Row row)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            return FromAs(row.Table, Alias.T0).Into(row);
        }

        public SqlQuery From(Alias alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            if (alias.Table.IsEmptyOrNull())
                throw new ArgumentNullException("alias.ToTable");

            return FromAs(alias.Table, alias);
        }
    }
}