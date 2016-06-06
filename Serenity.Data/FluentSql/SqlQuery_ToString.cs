using System;
using System.Text;

namespace Serenity.Data
{
    public partial class SqlQuery
    {
        /// <summary>
        ///   SqlSelect sorgusunu formatlayıp bir SELECT sorgusuna çevirir. Sayfalama sözkonusuysa 
        ///   (atlanan kayıt varsa) birden fazla sorgu arka arkaya oluşturulur.</summary>
        /// <returns>
        ///   Formatlanmış SELECT ifadesi</returns>
        public override string ToString()
        {
            // formatlamada kullanılacak StringBuilder nesnesi
            var sb = new StringBuilder();

            // sub queries should be enclosed in paranthesis
            if (this.parent != null)
                sb.Append("(");

            if (skip > 0 && orderBy == null && !dialect.CanUseSkipKeyword)
                throw new InvalidOperationException("A query must be ordered by unique fields " +
                    "to be able to skip records!");

            // ek filtremiz şimdilik yok
            string extraWhere = null;

            bool useSkipKeyword = skip > 0 && dialect.CanUseSkipKeyword;
            bool useOffset = skip > 0 && !useSkipKeyword && dialect.CanUseOffsetFetch;
            bool useRowNumber = skip > 0 && !useSkipKeyword && !useOffset && dialect.CanUseRowNumber;
            bool useRowNum = (skip > 0 || take > 0) && dialect.UseRowNum;
            bool useSecondQuery = skip > 0 && !useSkipKeyword && !useOffset && !useRowNumber;

            // atlanması istenen kayıt var mı?
            if (useRowNumber || useRowNum || useSecondQuery)
            {
                if (useRowNumber || useRowNum)
                {
                    sb.Append("SELECT * FROM (\n");
                }
                else
                {
                    const string AssignCmd = "@Value{0} = {1}";
                    const string DeclareCmd = "DECLARE @Value{0} SQL_VARIANT;\n";
                    const string Equality = "(({0} IS NULL AND @Value{1} IS NULL) OR ({0} = @Value{1}))";
                    const string Greater = "(({0} IS NOT NULL AND @Value{1} IS NULL) OR ({0} > @Value{1}))";
                    const string LessThan = "(({0} IS NULL AND @Value{1} IS NOT NULL) OR ({0} < @Value{1}))";

                    // atlanması istenen kayıtları geçip geri kalanları getirebilmek için öncelikle sorguyu 
                    // sadece anahtar sahaları seçecek şekilde çalıştırıyor, atlanması istenen kayıt kadar 
                    // ilerliyor, bu son kaydın sıralanan alanlarının değerlerini buluyoruz.

                    // Örneğin asıl sorgumuz 
                    // SELECT ID, A, B, C, D FROM TABLO WHERE X > Y ORDER BY ID olsun. SKIP 5 belirtilmişse 
                    // ilk 5 kaydın atlanıp 6 dan devam edilmesi gerekir. Bunun için,
                    // DECLARE @ID SQL_VARIANT; SELECT TOP 5 @ID = ID FROM TABLO ORDER BY ID
                    // gibi bir sorgu çalıştırılıp, 5. kaydın ID si bulunmuş olur. 5 ten sonraki kayıtları 
                    // almak için oluşturulması gereken sorgu ise,
                    // SELECT ID, A, B, C, D FROM TABLO WHERE ID > @ID AND X > Y ORDER BY ID
                    // Burada @ID 5. kaydın ID sidir.

                    // Sıralanan alanlar birden çok olduğunda, örneğin DATE, ID alanlarına göre sıralama 
                    // olduğunda, yine bu alanların son değerleri bulunur. Fakat bu sefer,
                    // DATE > @DATE AND ID > @ID yazılamaz. Çünkü sıralamaya göre bir kaydın DATE değeri 
                    // daha büyükse ID si ne olursa olsun daha alttadır. Bu tip bir koşulsa ID nin de 
                    // büyük olmasını gerektirir. Yazılması gereken koşul,
                    // (DATE > @DATE) OR (DATE = @DATE AND ID > @ID) olmalıdır. 

                    // Null değerleriyle karşılaştırmalarda sorun çıkmaması ve sıralamanın bozulmaması için 
                    // karşılaştırmalar Null değerleri de gözönüne alınarak yapılmalıdır. Yukarıdaki ifade NULL 
                    // durumları da gözönüne alınarak:
                    // ((DATE IS NOT NULL AND @DATE IS NULL) OR (DATE > @DATE)) OR 
                    // ((ID IS NULL AND @ID IS NULL) OR (ID = @ID))
                    // şeklinde yazılabilir. İlk satırda NULL değerlerinin sıralamada her zaman null olmayan 
                    // bir değerden önce geleceği kabul edilir. İkinci satırda ise iki NULL un sıralama olarak
                    // eşit oldukları kabul edilir.

                    // ikinci sorgulama için atlanan kayıtların altındaki kayıtları bulmayı sağlayacak koşulu 
                    // oluşturacağımız StringBuilder
                    var check = new StringBuilder();

                    // sıralanan alan adlarını sonlarındaki DESC atılmış şekilde tutacak dizi
                    var order = new string[orderBy.Count];

                    // alanların ters sıralı olma durumlarını tutacak dizi
                    var desc = new bool[orderBy.Count];

                    // tüm sıralama listesini tara
                    for (int i = 0; i < orderBy.Count; i++)
                    {
                        // her sıralı saha için bir SQL_VARIANT değişkeni tanımla
                        sb.AppendFormat(DeclareCmd, i);

                        // sıralanan alanı oku
                        string o = orderBy[i];

                        // eğer DESC ile bitiyorsa bu ters sıralamadır, karşılaştırmalarda gözönüne alınmalı
                        desc[i] = o.EndsWith(SqlKeywords.Desc, StringComparison.OrdinalIgnoreCase);

                        // ters sıralıysa DESC ifadesini sonundan atarak sıralı alan listesine ekle
                        if (desc[i])
                            order[i] = o.Substring(0, o.Length - SqlKeywords.Desc.Length);
                        else
                            order[i] = o;
                    }

                    // SELECT TOP...
                    sb.Append(SqlKeywords.Select);
                    if (distinct)
                        sb.Append(SqlKeywords.Distinct);

                    sb.Append(dialect.TakeKeyword);
                    sb.Append(' ');
                    // Atlanacak kayıt sayısı kadar
                    sb.Append(skip);
                    sb.Append(' ');

                    // Alan listesini SqlSelect in kendi seçilecek alan listesi yerine
                    // @Value1 = SiraliAlan1, @Value2 = SiraliAlan2 şeklinde düzenle.
                    for (int i = 0; i < order.Length; i++)
                    {
                        if (i > 0)
                            sb.Append(',');
                        sb.AppendFormat(AssignCmd, i, order[i]);
                    }

                    // SqlSelect'in diğer kısımlarını sorguya ekle
                    AppendFromWhereOrderByGroupByHaving(sb, null, true);

                    sb.Append(";\n");

                    // son kayıttan sonrakileri bulmaya yarayacak koşul, alan sayısı arttıkça biraz daha 
                    // karmaşıklaşmaktadır. Örneğin A, B, C, D gibi 4 alanlı bir sıralamada,
                    // (A > @A) OR 
                    // (A = @A AND B > @B) OR 
                    // (A = @A AND B = @B AND C > @C) OR
                    // (A = @A AND B = @B AND C = @C AND D > @D)
                    // Burada basitlik açısından NULL durumları yazılmamıştır. Görüldüğü gibi her satırda bir 
                    // öncekinden bir fazla karşılaştırma vardır, bunların satır numarası - 1 adedi
                    // eşitlik, bir tanesi büyüklük (ters sıralamaysa küçüklük) kontrolüdür.

                    // tüm satırları ortak paranteze al
                    check.Append('(');
                    for (int statement = 0; statement < order.Length; statement++)
                    {
                        // ilk satırdan sonra araya OR koy
                        if (statement > 0)
                            check.Append(" OR ");

                        // bu satırın açılış parantezi
                        check.Append('(');

                        // satır numarasından bir eksiğine kadar, eşitlik koşulları
                        for (int equality = 0; equality < statement; equality++)
                        {
                            // ilk koşuldan sonra araya AND koy
                            if (equality > 0)
                                check.Append(" AND ");

                            // null durumunu da gözönüne alan eşitlik karşılaştırmasını yaz
                            check.AppendFormat(Equality, order[equality], equality);
                        }

                        // büyüklük ya da küçüklük koşulundan önce araya AND koy. 
                        // ilk satırda eşitlik durumu olmadığından AND e gerek yok.
                        if (statement > 0)
                            check.Append(" AND ");

                        // sıralamanın ters olup olmamasına göre bu satır için büyüklük 
                        // ya da küçüklük koşulunu null durumunu da gözönüne alarak ekle
                        if (desc[statement])
                            check.AppendFormat(LessThan, order[statement], statement);
                        else
                            check.AppendFormat(Greater, order[statement], statement);

                        // bu satırın kapanış parantezi
                        check.Append(')');
                    }
                    // tüm satırların kapanış parantezi
                    check.Append(')');

                    // bunu bir sonraki asıl sorgu için ekstra filtre olarak belirle
                    extraWhere = check.ToString();
                }
            }

            // asıl SELECT sorugusu başlangıcı
            sb.Append(SqlKeywords.Select);

            if (distinct)
            {
                sb.Append(SqlKeywords.Distinct);
            }

            // alınacak kayıt sayısı sınırlanmışsa bunu TOP N olarak sorgu başına yaz
            if (take != 0 && (!useOffset) && (!useRowNum) && (useRowNumber || !dialect.UseTakeAtEnd))
            {
                sb.Append(dialect.TakeKeyword);
                sb.Append(' ');
                sb.Append(useRowNumber ? (skip + take) : take);
                sb.Append(' ');
            }

            if (useSkipKeyword)
            {
                sb.Append(dialect.SkipKeyword);
                sb.Append(' ');
                sb.Append(skip);
            }

            StringBuilder selCount = null;
            if (distinct)
                selCount = new StringBuilder();

            sb.Append('\n');
            // seçilecek alan listesini dolaş
            for (int i = 0; i < columns.Count; i++)
            {
                var s = columns[i];

                // ilk alan adından sonra araya virgül konmalı
                if (i > 0)
                {
                    sb.Append(",\n");
                    if (distinct)
                        selCount.Append(',');
                }

                // alan adını yaz
                sb.Append(s.Expression);

                if (distinct)
                    selCount.Append(s.Expression);

                // alana bir alias atanmışsa bunu yaz
                if (!string.IsNullOrEmpty(s.ColumnName))
                {
                    sb.Append(SqlKeywords.As);
                    var quoted = dialect.QuoteColumnAlias(s.ColumnName);
                    sb.Append(quoted);
                    if (distinct)
                    {
                        selCount.Append(SqlKeywords.As);
                        selCount.Append(quoted);
                    }
                }
            }

            if (useRowNumber)
            {
                if (columns.Count > 0)
                    sb.Append(", ");

                sb.Append("ROW_NUMBER() OVER (ORDER BY ");

                if(orderBy != null)
                for (int i = 0; i < orderBy.Count; i++)
                {
                    if (i > 0)
                        sb.Append(", ");

                    sb.Append(orderBy[i]);
                }
                sb.Append(") AS __num__");
            }

            // select sorgusunun kalan kısımlarını yaz
            AppendFromWhereOrderByGroupByHaving(sb, extraWhere, !useRowNumber);

            if (useRowNumber)
            {
                sb.Append(") __results__ WHERE __num__ > ");
                sb.Append(skip);
            }

            if (useRowNum)
            {
                sb.Append(") WHERE ROWNUM > " + skip +
                    " AND ROWNUM <= " + (skip + take));
            }

            if (take != 0 && (!useRowNum) && (!useOffset) && !useRowNumber && dialect.UseTakeAtEnd)
            {
                sb.Append(' ');
                sb.Append(dialect.TakeKeyword);
                sb.Append(' ');
                sb.Append(take);
            }

            if (useOffset)
            {
                if (take == 0)
                    sb.Append(String.Format(dialect.OffsetFormat, skip, take));
                else
                    sb.Append(String.Format(dialect.OffsetFetchFormat, skip, take));
            }

            if (!string.IsNullOrEmpty(forXml))
            {
                sb.Append(" FOR XML ");
                sb.Append(forXml);
            }

            if (countRecords)
            {
                if (!dialect.MultipleResultsets)
                    sb.Append("\n---\n"); // temporary fix till we find a better solution for firebird
                else
                    sb.Append(";\n");
                sb.Append(SqlKeywords.Select);
                sb.Append("count(*) ");

                if (distinct)
                {
                    sb.Append(SqlKeywords.From);
                    sb.Append('(');
                    sb.Append(SqlKeywords.Select);
                    sb.Append(SqlKeywords.Distinct);
                    sb.Append(selCount);
                }
                else if (groupBy != null && groupBy.Length > 0)
                {
                    sb.Append(SqlKeywords.From);
                    sb.Append('(');
                    sb.Append(SqlKeywords.Select);
                    sb.Append(" 1 as _alias_x_ ");
                }

                AppendFromWhereOrderByGroupByHaving(sb, null, false);

                if (distinct || (groupBy!= null && groupBy.Length > 0))
                {
                    sb.Append(") _alias_");
                }
            }

            // sub queries should be enclosed in paranthesis
            if (this.parent != null)
                sb.Append(")");

            // select sorgusunu döndür
            return sb.ToString();
        }

        /// <summary>
        ///   Verilen StringBuilder nesnesine SqlSelect'in FROM, WHERE, ORDER BY, GROUP BY, HAVING
        ///   kısımlarını, belirtilirse bir ek filtre de gözönüne alınarak ekler.</summary>
        /// <param name="sb">
        ///   SqlSelect nesnesinin mevcut from, where... kısımlarının formatlanıp ekleneceği
        ///   StringBuilder nesnesi.</param>
        /// <param name="extraWhere">
        ///   Belirtilirse WHERE koşullarına AND'lenerek eklenecek ekstra filtre.</param>
        /// <param name="includeOrderBy">
        ///   Sonuçta ORDER BY kısmı bulunsun mu?</param>
        /// <remarks>
        ///   Sayfalama için üretilen sorgularda SqlSelect'in bu kısımları iki ayrı yerde (birinde ek bir 
        ///   koşulla birlikte) kullanıldığından, bu şekilde yapılarak, kod tekrarının önüne 
        ///   geçilmiştir.</remarks>
        private void AppendFromWhereOrderByGroupByHaving(StringBuilder sb, string extraWhere,
            bool includeOrderBy)
        {
            if (from.Length > 0)
            {
                // FROM yaz
                sb.Append(SqlKeywords.From);
                // tablo listesini yaz ("A LEFT OUTER JOIN B ON (...) ....")
                sb.Append(from.ToString());
            }

            // ekstra filtre belirtilmişse ya da mevcut bir filtre varsa sorgunun
            // WHERE kısmı olacaktır
            if (extraWhere != null || where != null)
            {
                // WHERE yaz
                sb.Append(SqlKeywords.Where);

                // Varsa, SqlSelect'te hazırlanmış filtreleri yaz
                if (where != null)
                    sb.Append(@where);

                // Ekstra filtre belirtilmişse...
                if (extraWhere != null)
                {
                    // SqlSelect'in kendi filtresi de varsa araya AND koyulmalı
                    if (where != null)
                        sb.Append(" AND ");
                    // Ekstra filtreyi ekle
                    sb.Append(extraWhere);
                }
            }

            // Gruplama varsa ekle
            if (groupBy != null)
            {
                sb.Append(SqlKeywords.GroupBy);
                sb.Append(groupBy);
            }

            // Grup koşulu varsa ekle
            if (having != null)
            {
                sb.Append(SqlKeywords.Having);
                sb.Append(having);
            }

            // Sıralanmış alanlar varsa
            if (includeOrderBy && orderBy != null)
            {
                // ORDER BY yaz
                sb.Append(SqlKeywords.OrderBy);

                // Sıralanan tüm alanları aralarına "," koyarak ekle
                for (int i = 0; i < orderBy.Count; i++)
                {
                    if (i > 0)
                        sb.Append(", ");

                    sb.Append(orderBy[i]);
                }
            }
        }
    }
}