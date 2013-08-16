using System;
using System.Text;

namespace Serenity.Data
{
    public partial class SqlSelect
    {
        /// <summary>
        ///   SqlSelect sorgusunu formatlayıp bir SELECT sorgusuna çevirir. Sayfalama sözkonusuysa 
        ///   (atlanan kayıt varsa) birden fazla sorgu arka arkaya oluşturulur.</summary>
        /// <returns>
        ///   Formatlanmış SELECT ifadesi</returns>
        public override string ToString()
        {
            if (_cachedQuery != null)
                return _cachedQuery;

            // formatlamada kullanılacak StringBuilder nesnesi
            StringBuilder sb = new StringBuilder();

            // kayıt atlama istenmiş ve anahtar sahalar belirtilmiş mi? kayıt atlama istense de sıralanan 
            // alanların mevcut olması ve bu sıraya göre kayıtların unique olması gerekir.
            // sıralama unique alanlara göre değilse tablonun primary  key'inin OrderBy listesinin sonuna 
            // eklenmesi gerekir.
            bool willSkipRecords = _skip != 0 && _orderBy != null &&
                !SqlSettings.CanSkipRecords;

            if (_skip > 0 && _orderBy == null)
                throw new InvalidOperationException("A query must be ordered by unique fields " +
                    "to be able to skip records!");

            // ek filtremiz şimdilik yok
            string extraWhere = null;

            // atlanması istenen kayıt var mı?
            if (willSkipRecords)
            {
                if (_useRowNumber)
                {
                    sb.Append("SELECT * FROM (");
                }
                else
                {
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
                    StringBuilder check = new StringBuilder();

                    // sıralanan alan adlarını sonlarındaki DESC atılmış şekilde tutacak dizi
                    string[] order = new string[_orderBy.Count];

                    // alanların ters sıralı olma durumlarını tutacak dizi
                    bool[] desc = new bool[_orderBy.Count];

                    // tüm sıralama listesini tara
                    for (int i = 0; i < _orderBy.Count; i++)
                    {
                        // her sıralı saha için bir SQL_VARIANT değişkeni tanımla
                        sb.AppendFormat(Consts.DeclareCmd, i);

                        // sıralanan alanı oku
                        string o = _orderBy[i];

                        // eğer DESC ile bitiyorsa bu ters sıralamadır, karşılaştırmalarda gözönüne alınmalı
                        desc[i] = o.EndsWith(Sql.Keyword.Desc, StringComparison.OrdinalIgnoreCase);

                        // ters sıralıysa DESC ifadesini sonundan atarak sıralı alan listesine ekle
                        if (desc[i])
                            order[i] = o.Substring(0, o.Length - Sql.Keyword.Desc.Length);
                        else
                            order[i] = o;
                    }

                    // SELECT TOP...
                    sb.Append(Sql.Keyword.Select);
                    if (_distinct)
                        sb.Append(Sql.Keyword.Distinct);

                    sb.Append(SqlSettings.TakeKeyword);
                    sb.Append(' ');
                    // Atlanacak kayıt sayısı kadar
                    sb.Append(_skip);
                    sb.Append(' ');

                    // Alan listesini SqlSelect in kendi seçilecek alan listesi yerine
                    // @Value1 = SiraliAlan1, @Value2 = SiraliAlan2 şeklinde düzenle.
                    for (int i = 0; i < order.Length; i++)
                    {
                        if (i > 0)
                            sb.Append(',');
                        sb.AppendFormat(Consts.AssignCmd, i, order[i]);
                    }

                    // SqlSelect'in diğer kısımlarını sorguya ekle
                    AppendFromWhereOrderByGroupByHaving(sb, null, true);

                    sb.Append('\n');

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
                            check.AppendFormat(Consts.Equality, order[equality], equality);
                        }

                        // büyüklük ya da küçüklük koşulundan önce araya AND koy. 
                        // ilk satırda eşitlik durumu olmadığından AND e gerek yok.
                        if (statement > 0)
                            check.Append(" AND ");

                        // sıralamanın ters olup olmamasına göre bu satır için büyüklük 
                        // ya da küçüklük koşulunu null durumunu da gözönüne alarak ekle
                        if (desc[statement])
                            check.AppendFormat(Consts.LessThan, order[statement], statement);
                        else
                            check.AppendFormat(Consts.Greater, order[statement], statement);

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
            sb.Append(Sql.Keyword.Select);

            if (_distinct)
            {
                sb.Append(Sql.Keyword.Distinct);
            }

            // alınacak kayıt sayısı sınırlanmışsa bunu TOP N olarak sorgu başına yaz
            if (_take != 0)
            {
                sb.Append(SqlSettings.TakeKeyword);
                sb.Append(' ');
                sb.Append((willSkipRecords && _useRowNumber) ? (_skip + _take) : _take);
                sb.Append(' ');
            }

            if (_skip > 0 && SqlSettings.CanSkipRecords)
            {
                sb.Append(SqlSettings.SkipKeyword);
                sb.Append(' ');
                sb.Append(_skip);
            }

            StringBuilder selCount = null;
            if (_distinct)
                selCount = new StringBuilder();

            // seçilecek alan listesini dolaş
            for (int i = 0; i < _columns.Count; i++)
            {
                Column s = _columns[i];

                // ilk alan adından sonra araya virgül konmalı
                if (i > 0)
                {
                    sb.Append(',');
                    if (_distinct)
                        selCount.Append(',');
                }

                // alan adını yaz
                sb.Append(s.Expression);

                if (_distinct)
                    selCount.Append(s.Expression);

                // alana bir alias atanmışsa bunu yaz
                if (s.AsAlias != null)
                {
                    sb.Append(Sql.Keyword.As);
                    sb.Append(s.AsAlias);
                    if (_distinct)
                    {
                        selCount.Append(Sql.Keyword.As);
                        selCount.Append(s.AsAlias);
                    }
                }
            }

            if (willSkipRecords && _useRowNumber)
            {
                if (!SqlSettings.CanUseRowNumber)
                    throw new InvalidOperationException();

                if (_columns.Count > 0)
                    sb.Append(',');
                sb.Append("ROW_NUMBER() OVER (ORDER BY ");

                for (int i = 0; i < _orderBy.Count; i++)
                {
                    if (i > 0)
                        sb.Append(',');

                    sb.Append(_orderBy[i].ToString());
                }
                sb.Append(") AS _row_number_");
            }

            // select sorgusunun kalan kısımlarını yaz
            AppendFromWhereOrderByGroupByHaving(sb, extraWhere, !(willSkipRecords && _useRowNumber));

            if (willSkipRecords && _useRowNumber)
            {
                sb.Append(") _row_number_results_ WHERE _row_number_ > ");
                sb.Append(_skip);
            }

            if (_countRecords)
            {
                if (!SqlSettings.MultipleResultsets)
                    sb.Append("\n---\n"); // temporary fix till we find a better solution for firebird
                else
                    sb.Append(";\n");
                sb.Append(Sql.Keyword.Select);
                sb.Append("count(*) ");

                if (_distinct)
                {
                    sb.Append(Sql.Keyword.From);
                    sb.Append('(');
                    sb.Append(Sql.Keyword.Select);
                    sb.Append(Sql.Keyword.Distinct);
                    sb.Append(selCount.ToString());
                }
                else if (_groupBy != null && _groupBy.Length > 0)
                {
                    sb.Append(Sql.Keyword.From);
                    sb.Append('(');
                    sb.Append(Sql.Keyword.Select);
                    sb.Append(" 1 as _alias_x_ ");
                }

                AppendFromWhereOrderByGroupByHaving(sb, null, false);

                if (_distinct || (_groupBy != null && _groupBy.Length > 0))
                {
                    sb.Append(") _alias_");
                }
            }

            _cachedQuery = sb.ToString();

            // select sorgusunu döndür
            return _cachedQuery;
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
            // FROM yaz
            sb.Append(Sql.Keyword.From);
            // tablo listesini yaz ("A LEFT OUTER JOIN B ON (...) ....")
            sb.Append(_from.ToString());

            // ekstra filtre belirtilmişse ya da mevcut bir filtre varsa sorgunun
            // WHERE kısmı olacaktır
            if (extraWhere != null || _where != null)
            {
                // WHERE yaz
                sb.Append(Sql.Keyword.Where);

                // Varsa, SqlSelect'te hazırlanmış filtreleri yaz
                if (_where != null)
                    sb.Append(_where.ToString());

                // Ekstra filtre belirtilmişse...
                if (extraWhere != null)
                {
                    // SqlSelect'in kendi filtresi de varsa araya AND koyulmalı
                    if (_where != null)
                        sb.Append(" AND ");
                    // Ekstra filtreyi ekle
                    sb.Append(extraWhere);
                }
            }

            // Gruplama varsa ekle
            if (_groupBy != null)
            {
                sb.Append(Sql.Keyword.GroupBy);
                sb.Append(_groupBy.ToString());
            }

            // Grup koşulu varsa ekle
            if (_having != null)
            {
                sb.Append(Sql.Keyword.Having);
                sb.Append(_having.ToString());
            }

            // Sıralanmış alanlar varsa
            if (includeOrderBy && _orderBy != null)
            {
                // ORDER BY yaz
                sb.Append(Sql.Keyword.OrderBy);

                // Sıralanan tüm alanları aralarına "," koyarak ekle
                for (int i = 0; i < _orderBy.Count; i++)
                {
                    if (i > 0)
                        sb.Append(',');

                    sb.Append(_orderBy[i].ToString());
                }
            }
        }
    }
}