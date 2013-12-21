namespace Serenity.Data
{
    using System;

    public partial class SqlQuery : ParameterizedQuery, IDbFilterable, ISqlSelect, IFilterableQuery
    {
        /// <summary>
        /// Alanı ya da verilen ifadeyi (expression) select listesine ekler.
        /// </summary>
        /// <param name="expression">Alan adı ya da SQL ifadesi</param>
        /// <returns>Sorgunun kendisi</returns>
        /// <remarks>Alana herhangi bir alias atanmaz</remarks>
        public SqlQuery Select(string expression)
        {
            if (expression == null || expression.Length == 0)
                throw new ArgumentNullException("expression");

            _cachedQuery = null;
            _columns.Add(new Column(expression, null, _intoIndex, null));
            return this;
        }

        /// <summary>
        /// Alan ya da ifadeleri (expressions) select listesine ekler
        /// </summary>
        /// <param name="expressions">Alan adı ya da SQL ifadeleri</param>
        /// <returns>Sorgunun kendisi</returns>
        /// <remarks>Select listesine eklenen alanlara herhangi bir alias atanmaz</remarks>
        public SqlQuery Select(params string[] expressions)
        {
            foreach (var s in expressions)
                Select(s);

            return this;
        }

        /// <summary>
        /// Alan adını select listesine başına verilen Alias ı getirerek ekler
        /// </summary>
        /// <param name="of">Alan adının başına arasına "." koyarak getirilecek kısa ad (tablo alias'ı)</param>
        /// <param name="fieldName">Alan adı</param>
        /// <returns>Sorgunun kendisi</returns>
        /// <remarks>Select listesine eklenen alana herhangi bir alias atanmaz</remarks>
        public SqlQuery Select(Alias of, string fieldName)
        {
            Select(of[fieldName]);

            return this;
        }

        /// <summary>
        /// Alan adlarını select listesine başlarına verilen Alias ı getirerek ekler
        /// </summary>
        /// <param name="of">Alan adlarının başına aralarına "." koyarak getirilecek kısa ad (tablo alias'ı)</param>
        /// <param name="fieldNames">Alan adları</param>
        /// <returns>Sorgunun kendisi</returns>
        /// <remarks>Select listesine eklenen alanlara herhangi bir alias atanmaz</remarks>
        public SqlQuery Select(Alias of, params string[] fieldNames)
        {
            foreach (var s in fieldNames)
                Select(of[s]);

            return this;
        }

        /// <summary>
        /// Alan ya da ifadeyi bir alias ile birlikte select listesine ekler
        /// </summary>
        /// <param name="expression">Alana adı ya da SQL ifadesi</param>
        /// <param name="alias">Seçilecek alana atanacak kısa ad (alias)</param>
        /// <returns>Sorgunun kendisi</returns>
        public SqlQuery SelectAs(string expression, string alias)
        {
            if (expression == null || expression.Length == 0)
                throw new ArgumentNullException("expression");

            if (alias == null || alias.Length == 0)
                throw new ArgumentNullException("alias");

            _cachedQuery = null;
            _columns.Add(new Column(expression, alias, _intoIndex, null));

            return this;
        }

        /// <summary>
        /// Verilen alan nesnesini select listesine, kendi alan ifadesini kullanarak ekler ve
        /// alanın adına göre de alias tanımlar. Eğer alan ifadesinde join ler geçiyorsa, yani
        /// başka bir tablodan gelen bir alan ise, ilgili joinler tespit edilip otomatik
        /// olarak sorguya dahil edilir.
        /// </summary>
        /// <param name="field">Alan nesnesi</param>
        /// <returns>Sorgunun kendisi</returns>
        public SqlQuery Select(Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            _cachedQuery = null;

            if (field.Expression == null)
            {
                _columns.Add(new Column(field.QueryExpression, field.Name, _intoIndex, field));
                return this;
            }

            EnsureJoinOf(field);
            _columns.Add(new Column(field.Expression, field.Name, _intoIndex, field));

            return this;
        }

        /// <summary>
        /// Verilen alan nesnelerini select listesine, kendi alan ifadelerini kullanarak ekler ve
        /// alanların adlarına göre de alias tanımlar. Eğer alan ifadelerinde join ler geçiyorsa, yani
        /// başka bir tablodan gelen bir alan ise, ilgili joinler tespit edilip otomatik
        /// olarak sorguya dahil edilir.
        /// </summary>
        /// <param name="field">Alan nesneleri</param>
        /// <returns>Sorgunun kendisi</returns>
        public SqlQuery Select(params Field[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            foreach (Field field in fields)
                Select(field);

            return this;
        }

        /// <summary>
        /// Verilen ifadeyi bir alanın alias'ı ile birlikte select listesine ekler,
        /// ve alanı sorgu sonuçları reader dan okunacağında hedef olarak belirler.
        /// </summary>
        /// <param name="expression">Alana adı ya da SQL ifadesi</param>
        /// <param name="alias">Seçilecek alana atanacak kısa ad (alias)</param>
        /// <returns>Sorgunun kendisi</returns>
        public SqlQuery SelectAs(string expression, Field alias)
        {
            if (expression == null || expression.Length == 0)
                throw new ArgumentNullException("field");

            if (alias == null)
                throw new ArgumentNullException("alias");

            _cachedQuery = null;
            _columns.Add(new Column(expression, alias.Name, _intoIndex, alias));
            return this;
        }
    }
}