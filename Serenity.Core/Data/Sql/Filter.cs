using System;
using System.Collections.Generic;
using System.Text;

namespace Serenity.Data
{
    /// <summary>
    ///   An object that is used to create filters by employing operator overloading 
    ///   features of C# language, instead of using string based filters.</summary>
    /// <remarks>
    ///   <p>This class is basically a StringBuilder to build filters.</p>
    ///   <p>Almost all operators are overloaded to modify the filter on left hand side.
    ///   For example: When "Filter1 &amp; Filter2" is processed the condition in Filter2 object is 
    ///   added to the condition in Filter1 by putting an AND operator between. And return value
    ///   will be the modified Filter1 object. Filter2 will stay as is.</p></remarks>
    public class Filter
    {
        // holds filter when it only contains a simple string
        private string _filterString;
        // the builder that will be used to generate filter
        private StringBuilder _filterBuilder;
        // single quote
        private const string Quote = "'";
        // double single quote
        private const string Quote2 = "''";
        // some constants used with filters
        private const string NotLeft = "NOT (";
        private const string Equal = " = ";
        private const string NotEqual = " != ";
        private const string GreaterThan = " > ";
        private const string GreaterThanEqual = " >= ";
        private const string LessThan = " < ";
        private const string LessThanEqual = " <= ";
        private const string And = " AND ";
        private const string Or = " OR ";
        private const string Xor = " XOR ";
        // LanguageID -> Collation name list
        private static Dictionary<Int64, string> _collations = new Dictionary<Int64, string>();

        /// <summary>
        ///   Creates an empty Filter</summary>
        public Filter()
        {
        }

        /// <summary>
        ///   Creates a new filter with given condition. This condition is usually a 
        ///   field name, but it can also be a filter text pre-generated.</summary>
        /// <remarks>
        ///   Usually used like: <c>new Filter("fieldname") >= 5</c>.</remarks>
        /// <param name="filter">
        ///   A field name or filter condition (can be null)</param>
        public Filter(string filter)
        {
            _filterString = filter;
        }

        /// <summary>
        ///   Creates a new filter that contains field name of the metafield.</summary>
        /// <param name="field">
        ///   Fieldfield (required).</param>
        public Filter(Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            _filterString = field.Expression ?? ("T0." + field.Name);
        }

        /// <summary>
        ///   Belirtilen tablo alias'ý ve alan adýný aralarýna nokta koyarak içeren yeni bir 
        ///   filtre oluþturur.</summary>
        /// <param name="joinAlias">
        ///   Tablo alias'ý. Null ya da boþ olursa önemsenmez.</param>
        /// <param name="field">
        ///   Alan adý (zorunlu).</param>
        public Filter(string joinAlias, string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            if (joinAlias == null || joinAlias.Length == 0)
                _filterString = field;
            else
                _filterString = joinAlias + "." + field;
        }

        /// <summary>
        ///   Belirtilen numerik tablo alias'ý (baþýna T konarak) ve alan adýný aralarýna 
        ///   nokta koyarak içeren yeni bir filtre oluþturur.</summary>
        /// <param name="joinNumber">
        ///   Join numarasý (T1 gibi kullanýlýr). Deðer sýfýrdan küçükse alan adý tek baþýna
        ///   kullanýlýr.</param>
        /// <param name="field">
        ///   Alan adý (zorunlu).</param>
        public Filter(int joinNumber, string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            if (joinNumber >= 0)
                _filterString = joinNumber.TableAliasDot() + field;
            else
                _filterString = field;
        }

        /// <summary>
        ///   Belirtilen join ve alan adýný aralarýna nokta koyarak içeren yeni bir filtre oluþturur.</summary>
        /// <param name="join">
        ///   Tablo alias bilgisini içeren LeftJoin nesnesi (zorunlu).</param>
        /// <param name="field">
        ///   Alan adý (zorunlu).</param>
        public Filter(LeftJoin join, string field)
            : this(join.JoinAlias, field)
        {           
        }

        /// <summary>
        ///   Belirtilen numerik tablo alias'ý (baþýna T konarak) ve alanýn adýný aralarýna 
        ///   nokta koyarak içeren yeni bir filtre oluþturur.</summary>
        /// <param name="joinNumber">
        ///   Join numarasý (T1 gibi kullanýlýr)</param>
        /// <param name="field">
        ///   Alan nesnesi (zorunlu).</param>
        public Filter(int joinNumber, Field field)
            : this(joinNumber, field.Name)
        {
        }

        /// <summary>
        ///   Belirtilen join ve meta alanýn adýný aralarýna nokta koyarak içeren yeni bir 
        ///   filtre oluþturur.</summary>
        /// <param name="join">
        ///   Tablo alias bilgisini içeren LeftJoin nesnesi (zorunlu).</param>
        /// <param name="field">
        ///   Field alan (zorunlu).</param>
        public Filter(LeftJoin join, Field field)
            : this(join.JoinAlias, field.Name)
        {
        }

        /// <summary>
        ///   Belirtilen join ve meta alanýn adýný aralarýna nokta koyarak içeren yeni bir 
        ///   filtre oluþturur.</summary>
        /// <param name="join">
        ///   Tablo alias bilgisini içeren LeftJoin nesnesi (zorunlu).</param>
        /// <param name="field">
        ///   Field alan (zorunlu).</param>
        public Filter(string join, Field field)
            : this(join, field.Name)
        {
        }


        /// <summary>
        ///   Verilen alan adýný köþeli parantez içine alarak yeni bir filtre oluþturur.
        ///   SQL'de boþluk içeren ya da keyword olan alan adlarýnýn kullanýlabilmesi 
        ///   için gerekebilir.</summary>
        /// <param name="fieldName">
        ///   Köþeli parantez içine alýnýp filtreye çevrilecek alan adý (zorunlu).</param>
        /// <returns>
        ///   Alan adýný köþeli parantez içinde içeren yeni bir filtre.</returns>
        public static Filter Bracket(string fieldName)
        {
            if (fieldName == null || fieldName.Length == 0)
                throw new ArgumentNullException("fieldName");

            return new Filter("[" + fieldName + "]");
        }

        /// <summary>
        ///   Verilen string'i _filter, ya da _filterSB alanlarýna ekler.</summary>
        /// <param name="str"></param>
        private void Append(string str)
        {
            if (str != null && str.Length > 0)
            {
                if (_filterBuilder != null)
                    _filterBuilder.Append(str);
                else if (_filterString != null && _filterString.Length > 0)
                {
                    _filterBuilder = new StringBuilder(_filterString, _filterString.Length + str.Length + 32);
                    _filterBuilder.Append(str);
                    _filterString = null;
                }
                else
                    _filterString = str;
            }
        }

        /// <summary>
        ///   Verilen iki string'i _filterSB alanýna ekler.</summary>
        /// <param name="str1">
        ///   Ýlk string.</param>
        /// <param name="str2">
        ///   Ýkinci string.</param>
        private void Append(string str1, string str2)
        {
            if (_filterBuilder == null)
            {
                if (_filterString != null)
                {
                    _filterBuilder = new StringBuilder(_filterString,
                        _filterString.Length + str1.Length + str2.Length + 32);
                    _filterBuilder.Append(str1);
                    _filterBuilder.Append(str2);
                    _filterString = null;
                }
                else
                {
                    _filterBuilder = new StringBuilder(str1, str1.Length + str2.Length + 32);
                    _filterBuilder.Append(str2);
                }
            }
            else
            {
                _filterBuilder.Append(str1);
                _filterBuilder.Append(str2);
            }
        }

        /// <summary>
        ///   Verilen string'i _filter, ya da _filterSB alanýnýn baþýna ekler.</summary>
        /// <param name="str">
        ///   Baþa eklenecek string.</param>
        private void AppendStart(string str)
        {
            if (str != null && str.Length > 0)
            {
                if (_filterBuilder != null)
                    _filterBuilder.Insert(0, str);
                else if (_filterString != null && _filterString.Length > 0)
                {
                    _filterBuilder = new StringBuilder(str, _filterString.Length + str.Length);
                    _filterBuilder.Append(_filterString);
                    _filterString = null;
                }
                else
                    _filterString = str;
            }
        }
    
        /// <summary>
        ///   Bir string deðerin implicit olarak (kendiliðinden) Filter nesnesine çevrilmesini 
        ///   saðlayan operatör overload</summary>
        /// <param name="str">
        ///   Filter nesnesine çevrilecek string.</param>
        /// <returns>
        ///   String'i SQL e uygun görünümünde içeren yeni bir Filter nesnesi.</returns>
        public static explicit operator Filter(string str)
        {
            return new Filter(str.ToSql());
        }

        /// <summary>
        ///   Bir Int32 deðerin implicit olarak (kendiliðinden) Filter nesnesine çevrilmesini 
        ///   saðlayan operatör overload</summary>
        /// <param name="value">
        ///   Filter nesnesine çevrilecek Int32 deðer.</param>
        /// <returns>
        ///   Int32 deðeri SQL e uygun görünümünde içeren yeni bir Filter nesnesi.</returns>
        public static explicit operator Filter(Int32 value)
        {
            return new Filter(value.ToInvariant());
        }

        /// <summary>
        ///   Bir Int64 deðerin implicit olarak (kendiliðinden) Filter nesnesine çevrilmesini 
        ///   saðlayan operatör overload</summary>
        /// <param name="value">
        ///   Filter nesnesine çevrilecek Int32 deðer.</param>
        /// <returns>
        ///   Int32 deðeri SQL e uygun görünümünde içeren yeni bir Filter nesnesi.</returns>
        public static explicit operator Filter(Int64 value)
        {
            return new Filter(value.ToInvariant());
        }

        /// <summary>
        ///   Bir Double deðerin implicit olarak (kendiliðinden) Filter nesnesine çevrilmesini 
        ///   saðlayan operatör overload</summary>
        /// <param name="value">
        ///   Filter nesnesine çevrilecek Double deðer.</param>
        /// <returns>
        ///   Double deðeri SQL e uygun görünümünde içeren yeni bir Filter nesnesi.</returns>
        public static explicit operator Filter(Double value)
        {
            return new Filter(value.ToInvariant());
        }

        /// <summary>
        ///   Bir Decimal deðerin implicit olarak (kendiliðinden) Filter nesnesine çevrilmesini 
        ///   saðlayan operatör overload</summary>
        /// <param name="value">
        ///   Filter nesnesine çevrilecek Decimal deðer.</param>
        /// <returns>
        ///   Decimal deðeri SQL e uygun görünümünde içeren yeni bir Filter nesnesi.</returns>
        public static explicit operator Filter(Decimal value)
        {
            return new Filter(value.ToInvariant());
        }

        /// <summary>
        ///   Bir DateTime deðerin implicit olarak (kendiliðinden) Filter nesnesine çevrilmesini 
        ///   saðlayan overload</summary>
        /// <param name="value">
        ///   Filter nesnesine çevrilecek DateTime deðer.</param>
        /// <returns>
        ///   DateTime'ý SQL e uygun görünümünde içeren yeni bir Filter nesnesi.</returns>
        public static explicit operator Filter(DateTime value)
        {
            return new Filter(((DateTime?)value).ToSql());
        }

        /// <summary>
        ///   C#'ýn "!" operatörünün Filter nesnesi için overload edilmiþ hali.</summary>
        /// <param name="filter">
        ///   Mevcut koþulu NOT(...) içine alýnacak Filter nesnesi</param>
        /// <returns>
        ///   Filtre nesnesinin içindeki koþulu NOT(...) içerisine alýp, nesnenin kendisini döndürür.</returns>
        public static Filter operator !(Filter filter)
        {
            filter.AppendStart(NotLeft);
            filter.Append(")");
            return filter;
        }

        /// <summary>
        ///   Ýki stringi aralarýna op ile belirtilen ayýracý da koyarak birleþtirir.</summary>
        /// <remarks>
        ///   <p>Ýki string de boþsa boþ string döndürülür.</p>
        ///   <p>Ýki string de doluysa aralarýna op ayýracý konulup yeni bir string döndürülür.</p>
        ///   <p>Ýkisinden biri doluysa dolu olan döndürülür.</p>
        /// </remarks>
        /// <param name="str1">
        ///   Birinci string (null olabilir).</param>
        /// <param name="str2">
        ///   Ýkinci string (null olabilir).</param>
        /// <param name="separator">
        ///   Ýkisi de doluysa aralarýna koyulacak ayýraç.</param>
        /// <returns>
        ///   Ýki string'den biri ya da aralarýna separator koyularak oluþturulmuþ yeni bir string.</returns>
        internal static string JoinIf(string str1, string str2, string separator)
        {
            str1 = str1 ?? String.Empty;
            str2 = str2 ?? String.Empty;

            if (str1.Length == 0 && str2.Length == 0)
                return String.Empty;
            else if (str1.Length != 0)
            {
                if (str2.Length != 0)
                    return str1 + separator + str2;
                else
                    return str1;
            }
            else
                return str2;
        }

        /// <summary>
        ///   Ýki filtrenin koþullarýný aralarýna op ile belirtilen operatörü de koyarak birleþtirir ve 
        ///   ikisinden birini döndürür.</summary>
        /// <remarks>
        ///   <p>Ýki filtre de doluysa (koþullarý boþ deðilse), birincinin koþuluna, ikincinin koþulunu araya 
        ///   op koyarak ekler, birinciyi döndürür.</p>
        ///   <p>Sadece birinci ya da ikincinin koþulu doluysa, dolu olaný döndürür. Ýkisi de boþsa 
        ///   birinci döner.</p></remarks>
        /// <param name="filter1">
        ///   Birinci filtre</param>
        /// <param name="filter2">
        ///   Ýkinci filtre</param>
        /// <param name="op">
        ///   Ýkisinin de koþullarý doluysa aralarýna koyulacak operatör. (AND, OR gibi)</param>
        /// <returns>
        ///   Ýki filtreden biri (ikisi de doluysa birinci filtrenin koþulunun sonuna op ayýracý ve 
        ///   ikincinin koþulu eklenir)</returns>
        private static Filter JoinIf(Filter filter1, Filter filter2, string op)
        {
            bool filter1LengthZero = filter1.IsEmpty;
            bool filter2LengthZero = filter2.IsEmpty;
            if (filter1LengthZero && filter2LengthZero)
                return filter1;
            else if (!filter1LengthZero)
            {
                if (!filter2LengthZero)
                {
                    filter1.Append(op, filter2.Text);
                    return filter1;
                }
                else
                    return filter1;
            }
            else
                return filter2;
        }

        /// <summary>
        ///   Verilen string boþ deðilse parantez içerisine alýr.</summary>
        /// <param name="str">
        ///   Parantez içine alýnacak string.</param>
        /// <returns>
        ///   Boþ bir string ya da str'in parantez içine alýnmýþ hali.</returns>
        internal static string ParantezIf(string str)
        {
            if (str != null && str.Length > 0)
                return "(" + str + ")";
            else
                return str;
        }

        /// <summary>
        ///   Parametre adý içeren bir filtre oluþturur.</summary>
        /// <param name="paramName">
        ///   Parametrenin adý. Baþýna @ getirilecek.</param>
        /// <returns>
        ///   Baþýna @ iþareti getirilmiþ parametreyi içeren yeni bir filtre.</returns>
        public static Filter Param(string paramName)
        {
            return new Filter("@" + paramName);
        }

        /// <summary>
        ///   Filtrenin sonuna IS NULL ekler.</summary>
        /// <returns>
        ///   Filter nesnesinin kendisi.</returns>
        public Filter IsNull()
        {
            Append(" IS NULL");
            return this;
        }

        /// <summary>
        ///   Filtrenin baþýna "datepart(part", sonuna ")" ekler.</summary>
        /// <returns>
        ///   Filter nesnesinin kendisi.</returns>
        public Filter Datepart(string part)
        {
            AppendStart(",");
            AppendStart(part);
            AppendStart("datepart(");
            Append(")");
            return this;
        }

        /// <summary>
        ///   Filtrenin sonuna IS NOT NULL ekler.</summary>
        /// <returns>
        ///   Filter nesnesinin kendisi.</returns>
        public Filter IsNotNull()
        {
            Append(" IS NOT NULL");
            return this;
        }

        /// <summary>
        ///   Filtreye LIKE ve verilen bir maskeyi ekler.</summary>
        /// <param name="mask">
        ///   LIKE operatörü uygulanacak maske (zorunlu).</param>
        /// <returns>
        ///   Filter nesnesinin kendisi.</returns>
        public Filter Like(string mask)
        {
            if (mask == null)
                throw new ArgumentNullException("mask");

            if (SqlSettings.IsCaseSensitive)
            {
                this._filterBuilder = this._filterBuilder ?? new StringBuilder(this._filterString);
                this._filterString = null;
                this._filterBuilder.Insert(0, "UPPER(");
                this._filterBuilder.Append(")");
                mask = mask.ToUpper();
            }

            Append(" LIKE ", mask.ToSql());
            return this;
        }

        /// <summary>
        ///   Filtreye LIKE ve baþýna % koyarak verilen bir maskeyi ekler.</summary>
        /// <param name="mask">
        ///   LIKE operatörüne uygulanacak maske. Baþýna % getirilir.</param>
        /// <returns>
        ///   Filter nesnesinin kendisi.</returns>
        public Filter StartsWith(string mask)
        {
            if (mask == null)
                throw new ArgumentNullException("mask");

            return Like(mask + "%");
        }

        /// <summary>
        ///   Filtreye LIKE ve sonuna % koyarak verilen bir maskeyi ekler.</summary>
        /// <param name="mask">
        ///   LIKE operatörüne uygulanacak maske. Sonuna % getirilir.</param>
        /// <returns>
        ///   Filter nesnesinin kendisi.</returns>
        public Filter EndsWith(string mask)
        {
            if (mask == null)
                throw new ArgumentNullException("mask");

            return Like("%" + mask);
        }

        /// <summary>
        ///   Filtreye LIKE ve baþýna/sonuna % koyarak verilen bir maskeyi ekler.</summary>
        /// <param name="mask">
        ///   LIKE operatörüne uygulanacak maske. Baþýna ve sonuna % getirilir.</param>
        /// <returns>
        ///   Filter nesnesinin kendisi.</returns>
        public Filter Contains(string mask)
        {
            return Like("%" + mask +"%");
        }

        /// <summary>
        ///   Filtreye sayýsal bir IN (...) ifadesi ekler.</summary>
        /// <param name="values">
        ///   IN(...) kýsmýna eklenecek sayý listesi.</param>
        /// <returns>
        ///   Filter nesnesinin kendisi.</returns>
        public Filter In(params int[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentNullException("values");

            Append(" IN (");
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                    Append(",");
                Append(values[i].ToInvariant());
            }
            Append(")");
            return this;
        }

        /// <summary>
        ///   Filtreye sayýsal bir IN (...) ifadesi ekler.</summary>
        /// <param name="values">
        ///   IN(...) kýsmýna eklenecek sayý listesi.</param>
        /// <returns>
        ///   Filter nesnesinin kendisi.</returns>
        public Filter In(params Int64[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentNullException("values");

            Append(" IN (");
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                    Append(",");
                Append(values[i].ToInvariant());
            }
            Append(")");
            return this;
        }

        /// <summary>
        ///   Filtreye sayýsal bir IN (...) ifadesi ekler.</summary>
        /// <param name="query">
        ///   Query (required).</param>
        /// <param name="values">
        ///   Parameter values</param>
        /// <returns>
        ///   Filter nesnesinin kendisi.</returns>
        public Filter InParams<T>(IDbParameterized query, params T[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentNullException("values");

            Append(" IN (");
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                    Append(",");
                var prm = query.AutoParam();
                Append(prm.Name);
                query.SetParam(prm.Name, values[i]);
            }
            Append(")");
            return this;
        }

        /// <summary>
        ///   Filtreye sayýsal bir IN (...) ifadesi ekler.</summary>
        /// <param name="paramCount">
        ///   Total parameter count.</param>
        /// <param name="indexParam">
        ///   Index of next parameter. Has the value indexParam + paramCount when function returns.".</param>
        /// <returns>
        ///   Filter nesnesinin kendisi.</returns>
        public Filter InParams(int paramCount, ref int indexParam)
        {
            if (paramCount <= 0)
                throw new ArgumentOutOfRangeException("paramCount");

            Append(" IN (");
            for (int i = 0; i < paramCount; i++)
            {
                if (i > 0)
                    Append(",");
                string prm = indexParam.IndexParam();
                Append(prm);
                indexParam++;
            }
            Append(")");
            return this;
        }

        /// <summary>
        ///   C#'ýn "==" operatörünü overload eder ve iki filtrenin koþullarýný aralarýna "=" koyarak, 
        ///   soldaki filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sol filtre. Sað filtrenin koþu araya "=" koyularak eklenecek.</param>
        /// <param name="filter2">
        ///   Sað filtre.</param>
        /// <returns>
        ///   Sol filtre (filter1)</returns>
        public static Filter operator ==(Filter filter1, Filter filter2)
        {
            filter1.Append(Equal, filter2.Text);
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "==" operatörünü overload eder ve bir filtreyle, Int32 deðerini aralarýna "=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "=" ve Int32 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int32 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator ==(Filter filter1, Parameter param)
        {
            filter1.Append(Equal, param.Name);
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "==" operatörünü overload eder ve bir filtreyle, Int32 deðerini aralarýna "=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "=" ve Int32 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int32 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator ==(Filter filter1, int value)
        {
            filter1.Append(Equal, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "==" operatörünü overload eder ve bir filtreyle, Int64 deðerini aralarýna "=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "=" ve Int64 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int64 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator ==(Filter filter1, Int64 value)
        {
            filter1.Append(Equal, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "==" operatörünü overload eder ve bir filtreyle, String deðerini aralarýna "=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "=" ve String deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator ==(Filter filter1, string value)
        {
            filter1.Append(Equal, value.ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "==" operatörünü overload eder ve bir filtreyle, Double deðerini aralarýna "=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "=" ve Double deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator ==(Filter filter1, Double value)
        {
            filter1.Append(Equal, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "==" operatörünü overload eder ve bir filtreyle, Decimal deðerini aralarýna "=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "=" ve Decimal deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator ==(Filter filter1, Decimal value)
        {
            filter1.Append(Equal, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "==" operatörünü overload eder ve bir filtreyle, DateTime deðerini aralarýna "=" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "=" ve DateTime deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator ==(Filter filter1, DateTime value)
        {
            filter1.Append(Equal, ((DateTime?)value).ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "==" operatörünü overload eder ve bir filtreyle, Guid deðerini aralarýna "=" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "=" ve Guid deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator ==(Filter filter1, Guid value)
        {
            filter1.Append(Equal, ((Guid?)value).ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "!=" operatörünü overload eder ve iki filtrenin koþullarýný aralarýna "&lt;&gt;" koyarak, 
        ///   soldaki filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sol filtre. Sað filtrenin koþu araya "=" koyularak eklenecek.</param>
        /// <param name="filter2">
        ///   Sað filtre.</param>
        /// <returns>
        ///   Sol filtre (filter1)</returns>
        public static Filter operator !=(Filter filter1, Filter filter2)
        {
            filter1.Append(NotEqual, filter2.Text);
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "==" operatörünü overload eder ve bir filtreyle, Int32 deðerini aralarýna "!=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "!=" ve Int32 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int32 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator !=(Filter filter1, Parameter param)
        {
            filter1.Append(NotEqual, param.Name);
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "==" operatörünü overload eder ve bir filtreyle, Int32 deðerini aralarýna "!=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "!=" ve Int32 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int32 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator !=(Filter filter1, Int32 value)
        {
            filter1.Append(NotEqual, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "==" operatörünü overload eder ve bir filtreyle, Int64 deðerini aralarýna "!=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "!=" ve Int64 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int64 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator !=(Filter filter1, Int64 value)
        {
            filter1.Append(NotEqual, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "!=" operatörünü overload eder ve bir filtreyle, String deðerini aralarýna "!=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "!=" ve String deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator !=(Filter filter1, string value)
        {
            filter1.Append(NotEqual, value.ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "!=" operatörünü overload eder ve bir filtreyle, Double deðerini aralarýna "!=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "!=" ve Double deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator !=(Filter filter1, Double value)
        {
            filter1.Append(NotEqual, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "!=" operatörünü overload eder ve bir filtreyle, Decimal deðerini aralarýna "!=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "!=" ve Decimal deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator !=(Filter filter1, Decimal value)
        {
            filter1.Append(NotEqual, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "!=" operatörünü overload eder ve bir filtreyle, DateTime deðerini aralarýna "!=" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "!=" ve DateTime deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator !=(Filter filter1, DateTime value)
        {
            filter1.Append(NotEqual, ((DateTime?)value).ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "!=" operatörünü overload eder ve bir filtreyle, Guid deðerini aralarýna "!=" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "!=" ve Guid deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator !=(Filter filter1, Guid value)
        {
            filter1.Append(NotEqual, ((Guid?)value).ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;" operatörünü overload eder ve iki filtrenin koþullarýný aralarýna "&gt;" koyarak, 
        ///   soldaki filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sol filtre. Sað filtrenin koþu araya "&gt;" koyularak eklenecek.</param>
        /// <param name="filter2">
        ///   Sað filtre.</param>
        /// <returns>
        ///   Sol filtre (filter1)</returns>
        public static Filter operator >(Filter filter1, Filter filter2)
        {
            filter1.Append(GreaterThan, filter2.Text);
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;" operatörünü overload eder ve bir filtreyle, Int32 deðerini aralarýna "&gt;" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;" ve Int32 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int32 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >(Filter filter1, Parameter param)
        {
            filter1.Append(GreaterThan, param.Name);
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;" operatörünü overload eder ve bir filtreyle, Int32 deðerini aralarýna "&gt;" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;" ve Int32 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int32 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >(Filter filter1, Int32 value)
        {
            filter1.Append(GreaterThan, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;" operatörünü overload eder ve bir filtreyle, Int64 deðerini aralarýna "&gt;" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;" ve Int64 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int64 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >(Filter filter1, Int64 value)
        {
            filter1.Append(GreaterThan, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;" operatörünü overload eder ve bir filtreyle, String deðerini aralarýna "&gt;" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;" ve String deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >(Filter filter1, string value)
        {
            filter1.Append(GreaterThan, value.ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;" operatörünü overload eder ve bir filtreyle, Double deðerini aralarýna "&gt;" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;" ve Double deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >(Filter filter1, Double value)
        {
            filter1.Append(GreaterThan, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;" operatörünü overload eder ve bir filtreyle, Decimal deðerini aralarýna "&gt;" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;" ve Decimal deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >(Filter filter1, Decimal value)
        {
            filter1.Append(GreaterThan, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;" operatörünü overload eder ve bir filtreyle, DateTime deðerini aralarýna "&gt;" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;" ve DateTime deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >(Filter filter1, DateTime value)
        {
            filter1.Append(GreaterThan, ((DateTime?)value).ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;" operatörünü overload eder ve bir filtreyle, Guid deðerini aralarýna "&gt;" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;" ve Guid deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >(Filter filter1, Guid value)
        {
            filter1.Append(GreaterThan, ((Guid?)value).ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;=" operatörünü overload eder ve iki filtrenin koþullarýný aralarýna "&gt;=" koyarak, 
        ///   soldaki filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sol filtre. Sað filtrenin koþu araya "&gt;=" koyularak eklenecek.</param>
        /// <param name="filter2">
        ///   Sað filtre.</param>
        /// <returns>
        ///   Sol filtre (filter1)</returns>
        public static Filter operator >=(Filter filter1, Filter filter2)
        {
            filter1.Append(GreaterThanEqual, filter2.Text);
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;=" operatörünü overload eder ve bir filtreyle, Int32 deðerini aralarýna "&gt;=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;=" ve Int32 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int32 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >=(Filter filter1, Parameter param)
        {
            filter1.Append(GreaterThanEqual, param.Name);
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;=" operatörünü overload eder ve bir filtreyle, Int32 deðerini aralarýna "&gt;=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;=" ve Int32 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int32 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >=(Filter filter1, int value)
        {
            filter1.Append(GreaterThanEqual, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;=" operatörünü overload eder ve bir filtreyle, Int64 deðerini aralarýna "&gt;=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;=" ve Int64 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int64 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >=(Filter filter1, long value)
        {
            filter1.Append(GreaterThanEqual, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;=" operatörünü overload eder ve bir filtreyle, String deðerini aralarýna "&gt;=" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;=" ve String deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >=(Filter filter1, string value)
        {
            filter1.Append(GreaterThanEqual, value.ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;=" operatörünü overload eder ve bir filtreyle, Double deðerini aralarýna "&gt;=" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;=" ve Double deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >=(Filter filter1, double value)
        {
            filter1.Append(GreaterThanEqual, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;=" operatörünü overload eder ve bir filtreyle, Decimal deðerini aralarýna "&gt;=" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;=" ve Decimal deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >=(Filter filter1, decimal value)
        {
            filter1.Append(GreaterThanEqual, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;=" operatörünü overload eder ve bir filtreyle, DateTime deðerini aralarýna "&gt;=" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;=" ve DateTime deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >=(Filter filter1, DateTime value)
        {
            filter1.Append(GreaterThanEqual, ((DateTime?)value).ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&gt;=" operatörünü overload eder ve bir filtreyle, Guid deðerini aralarýna "&gt;=" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&gt;=" ve Guid deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator >=(Filter filter1, Guid value)
        {
            filter1.Append(GreaterThanEqual, ((Guid?)value).ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;" operatörünü overload eder ve iki filtrenin koþullarýný aralarýna "&lt;" koyarak, 
        ///   soldaki filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sol filtre. Sað filtrenin koþu araya "&lt;" koyularak eklenecek.</param>
        /// <param name="filter2">
        ///   Sað filtre.</param>
        /// <returns>
        ///   Sol filtre (filter1)</returns>
        public static Filter operator <(Filter filter1, Filter filter2)
        {
            filter1.Append(LessThan, filter2.Text);
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;" operatörünü overload eder ve bir filtreyle, Int32 deðerini aralarýna "&lt;" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;" ve Int32 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int32 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <(Filter filter1, Parameter param)
        {
            filter1.Append(LessThan, param.Name);
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;" operatörünü overload eder ve bir filtreyle, Int32 deðerini aralarýna "&lt;" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;" ve Int32 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int32 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <(Filter filter1, int value)
        {
            filter1.Append(LessThan, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;" operatörünü overload eder ve bir filtreyle, Int64 deðerini aralarýna "&lt;" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;" ve Int64 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int64 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <(Filter filter1, long value)
        {
            filter1.Append(LessThan, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;" operatörünü overload eder ve bir filtreyle, String deðerini aralarýna "&lt;" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;" ve String deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <(Filter filter1, string value)
        {
            filter1.Append(LessThan, value.ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;" operatörünü overload eder ve bir filtreyle, Double deðerini aralarýna "&lt;" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;" ve Double deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <(Filter filter1, double value)
        {
            filter1.Append(LessThan, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;" operatörünü overload eder ve bir filtreyle, Double deðerini aralarýna "&lt;" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;" ve Double deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <(Filter filter1, decimal value)
        {
            filter1.Append(LessThan, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;" operatörünü overload eder ve bir filtreyle, DateTime deðerini aralarýna "&lt;" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;" ve DateTime deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <(Filter filter1, DateTime value)
        {
            filter1.Append(LessThan, ((DateTime?)value).ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;" operatörünü overload eder ve bir filtreyle, Guid deðerini aralarýna "&lt;" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;" ve Guid deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <(Filter filter1, Guid value)
        {
            filter1.Append(LessThan, ((Guid?)value).ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;=" operatörünü overload eder ve iki filtrenin koþullarýný aralarýna "&lt;=" koyarak, 
        ///   soldaki filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sol filtre. Sað filtrenin koþu araya "&lt;=" koyularak eklenecek.</param>
        /// <param name="filter2">
        ///   Sað filtre.</param>
        /// <returns>
        ///   Sol filtre (filter1)</returns>
        public static Filter operator <=(Filter filter1, Filter filter2)
        {
            filter1.Append(LessThanEqual, filter2.Text);
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;=" operatörünü overload eder ve bir filtreyle, Int32 deðerini aralarýna "&lt;=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;=" ve Int32 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int32 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <=(Filter filter1, Parameter param)
        {
            filter1.Append(LessThanEqual, param.Name);
            return filter1;
        }

         /// <summary>
        ///   C#'ýn "&lt;=" operatörünü overload eder ve bir filtreyle, Int32 deðerini aralarýna "&lt;=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;=" ve Int32 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int32 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <=(Filter filter1, int value)
        {
            filter1.Append(LessThanEqual, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;=" operatörünü overload eder ve bir filtreyle, Int64 deðerini aralarýna "&lt;=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;=" ve Int64 deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   Int64 tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <=(Filter filter1, Int64 value)
        {
            filter1.Append(LessThanEqual, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;=" operatörünü overload eder ve bir filtreyle, String deðerini aralarýna "&lt;=" koyarak, 
        ///   filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;=" ve String deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <=(Filter filter1, string value)
        {
            filter1.Append(LessThanEqual, value.ToSql());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;=" operatörünü overload eder ve bir filtreyle, Double deðerini aralarýna "&lt;=" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;=" ve Double deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <=(Filter filter1, double value)
        {
            filter1.Append(LessThanEqual, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;=" operatörünü overload eder ve bir filtreyle, decimal deðerini aralarýna "&lt;=" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;=" ve decimal deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <=(Filter filter1, decimal value)
        {
            filter1.Append(LessThanEqual, value.ToInvariant());
            return filter1;
        }

        /// <summary>
        ///   C#'ýn "&lt;=" operatörünü overload eder ve bir filtreyle, DateTime deðerini aralarýna "&lt;=" 
        ///   koyarak, filtre içerisinde birleþtirir.</summary>
        /// <param name="filter1">
        ///   Sonuna "&lt;=" ve DateTime deðeri SQL sabitine çevirilip eklenecek filtre.</param>
        /// <param name="value">
        ///   String tipinde deðer.</param>
        /// <returns>
        ///   Filter nesnesi (filter1)</returns>
        public static Filter operator <=(Filter filter1, DateTime value)
        {
            filter1.Append(LessThanEqual, ((DateTime?)value).ToSql());
            return filter1;
        }

        /// <summary>
        ///   Overloads C#'s "&lt;=" operator and joins a filter and a value by putting a "&lt;=" between in 
        ///   left filter.</summary>
        /// <param name="filter1">
        ///   Filter.</param>
        /// <param name="value">
        ///   Value.</param>
        /// <returns>
        ///   filter1</returns>
        public static Filter operator <=(Filter filter1, Guid value)
        {
            filter1.Append(LessThanEqual, ((Guid?)value).ToSql());
            return filter1;
        }

        /// <summary>
        ///   Overloads C#'s "&amp;" operator and joins two filters by putting a "AND" between in 
        ///   left filter.</summary>
        /// <param name="filter1">
        ///   Filter 1.</param>
        /// <param name="filter2">
        ///   Filter 2.</param>
        /// <returns>
        ///   filter1</returns>
        public static Filter operator &(Filter filter1, Filter filter2)
        {
            return JoinIf(filter1, filter2, And);
        }

        /// <summary>
        ///   Overloads C#'s "^" operator and joins two filters by putting a "OR" between in 
        ///   left filter.</summary>
        /// <param name="filter1">
        ///   Filter 1.</param>
        /// <param name="filter2">
        ///   Filter 2.</param>
        /// <returns>
        ///   filter1</returns>
        public static Filter operator |(Filter filter1, Filter filter2)
        {
            return JoinIf(filter1, filter2, Or);
        }

        /// <summary>
        ///   Overloads C#'s "^" operator and joins two filters by putting a "XOR" between in 
        ///   left filter.</summary>
        /// <param name="filter1">
        ///   Filter 1.</param>
        /// <param name="filter2">
        ///   Filter 2.</param>
        /// <returns>
        ///   filter1</returns>
        public static Filter operator ^(Filter filter1, Filter filter2)
        {
            filter1.Append(Xor, filter2.Text);
            return filter1;
        }

        /// <summary>
        ///   Overloads C#'s "~" operator and puts filter in paranthesis.</summary>
        /// <param name="filter">
        ///   Filter.</param>
        /// <returns>
        ///   filter in paranthesis</returns>
        public static Filter operator ~(Filter filter)
        {
            if (!filter.IsEmpty)
            {
                filter.AppendStart("(");
                filter.Append(")");
            }
            return filter;
        }

        /// <summary>
        ///   Adds "COLLATION " text and name of the collation specified to the filter.</summary>
        /// <param name="collation">
        ///   Collation name (can be null or empty, if so nothing changes)</param>
        /// <returns>
        ///   Filter object itself.</returns>
        public Filter Collate(string collation)
        {
            if (collation != null && collation.Length > 0)
            {
                Append(" COLLATE ", collation);
            }
            return this;
        }

        /// <summary>
        ///   Adds "COLLATION " text and name of the collation matching a language ID specified to the filter.</summary>
        /// <param name="languageID">
        ///   Language ID</param>
        /// <returns>
        ///   Filter object itself.</returns>
        public Filter Collate(Int64 languageID)
        {
            return Collate(GetCollation(languageID));
        }

        /// <summary>
        ///   Gets matching collation name for a language ID.</summary>
        /// <param name="languageID">
        ///   Language ID.</param>
        /// <returns>
        ///   Collation name. If not found, null.</returns>
        /// <seealso cref="SetCollations"/>
        public static string GetCollation(Int64 languageID)
        {
            string collation;
            if (_collations.TryGetValue(languageID, out collation))
                return collation;
            else
                return null;
        }

        /// <summary>
        ///   Sets the language ID -&gt; Collection name dictionary.</summary>
        /// <param name="collations">
        ///   LanguageID -&gt; Collation dictionary.</param>
        public static void SetCollations(IDictionary<Int64, string> collations)
        {
            if (collations == null)
                throw new ArgumentNullException("sqlCollations");

            Dictionary<Int64, string> newCollations = new Dictionary<Int64, string>();
            foreach (var p in collations)
                newCollations.Add(p.Key, p.Value);
            _collations = newCollations;
        }

        /// <summary>
        ///   Compares this filter with another object.</summary>
        /// <param name="obj">
        ///   Object to compare to.</param>
        /// <returns>
        ///   If object is of type Filter and its condition is equal, True.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Filter && ((Filter)obj).Text.Equals(this.Text));
        }

        /// <summary>
        ///   Creates a new EXISTS filter</summary>
        /// <param name="expression">
        ///   Expression</param>
        /// <returns></returns>
        public static Filter Exists(string expression)
        {
            var flt = new Filter("EXISTS (");
            flt.Append(expression);
            flt.Append(")");
            return flt;
        }

        /// <summary>
        ///   Returns an hashcode for the filter.</summary>
        /// <returns>
        ///   Hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        ///   Gets if filter is empty.</summary>
        public bool IsEmpty
        {
            get 
            {
                return
                    (_filterBuilder == null && (_filterString == null || _filterString.Length == 0)) ||
                    (_filterBuilder != null && _filterBuilder.Length == 0);
            }
        }

        /// <summary>
        ///   Converts filter to a string.</summary>
        /// <returns>
        ///   Filter condition as string.</returns>
        public override string ToString()
        {
            if (_filterBuilder != null)
                return _filterBuilder.ToString();
            else
                return _filterString ?? String.Empty;
        }

        /// <summary>
        ///   Gets filter as string.</summary>
        public string Text
        {
            get 
            {
                if (_filterBuilder != null)
                    return _filterBuilder.ToString();
                else
                    return _filterString ?? String.Empty;
            }
        }
    }
}