# SqlQuery Nesnesi
Dinamik SQL SELECT sorguları hazırlamanız için StringBuilder benzeri bir nesnedir (aslında bu nesne kendi içinde birçok StringBuilder’dan faydalanır).


## SAĞLADIĞI FAYDA ve KOLAYLIKLAR

Öncelikle bu nesneyi kullanmanın bize sağlayacağı bazı avantajları sıralayalım:

* Dinamik SQL sorgusu oluştururken Visual Studio’nun IntelliSense özelliğinden faydalanmamızı sağlar.

* Yapabileceğiniz dizilim (syntax) hatalarından birçoğunun kontrolü derleyici tarafından yapılacağından, sorgu inşaa ederken karşılaşabileceğiniz yazım hataları azalır. Çalışma zamanı yerine derleme anında bu hataları yakalar ve zaman kazanırsınız.

* Alan listesinin arasına virgül koymayı, ya da WHERE deyimindeki koşulların arasına AND koymayı unutmak gibi basit hatalar yapmazsınız.

* SELECT, FROM, WHERE gibi deyimleri kullandığınız sıranın hiçbir önemi yoktur. Sorguyu metne çevirirken bu sorgu kısımları olması gereken yere konumlandırılır. 

* Yine SELECT, WHERE, ORDER BY gibi deyimleri istediğiniz sayıda çağırabilirsiniz. Mesela önce temel sorgunuzu hazırlayıp, devamındaki satırlarda verinin kullanıcının istediği sırada (mesela grid’de başlığına tıkladığı kolon sırasında) gelmesi için ikinci bir ORDER BY ifadesi, ya da belli metni içeren kayıtları süzmek için ikinci bir WHERE koşulu ekleyebilirsiniz. Bu sizi hataya müsait birçok IF bloğu yazmaktan kurtarır.

* Sorgunun parçalarının dinamik modifiye edilebilmesi sayesinde, temel bir sorgu hazırlayıp bunu özelleştirilmiş benzer sorgularda baz olarak kullanabilirsiniz (polymorphism gibi).

* Parametrik sorgular oluşturmak için ekstra uğraşmanıza gerek kalmaz. SqlQuery yazdığınız koşullardaki sabit değerleri otomatik olarak parametrelere çevirir.

* SQL 2000 benzeri sayfalama desteklemeyen (SKIP ifadesi olmayan) sunucular için otomatik olarak sayfalama sağlayan sorgular oluşturabilir.

* Dialect desteği sayesinde yazdığınız sorgu, Firebird, MSSQL2005, MSSQL2012 gibi farklı sunucu ya da versiyonlarda, o sunucuya uygun SQL ifadesi üretebilir. (Not: Bu özellik henüz ideal seviyesinde değil. Sadece Firebird ve SQL server için sınırlı kapsamda çalışıyor)

* Serenity entity yapısıyla birlikte kullanıldığında sorgu çalıştırıldıktan sonra sonuçlarının reader dan entity objelerine yüklenmesini basitleştirir ve bunu yaparken reflection kullanılmadığından ek yük getirmez.

* Yine Serenity entity yapısıyla kullanıldığında, entity içinde tanımlı LEFT JOIN ifadelerini ilk gerektiğinde sorguya otomatik olarak ekler, sizin hatırlamanız gerekmez.


## ÖRNEK 1 - BASİT BİR SQL İFADESİ

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing1_SimpleSelectFromOrderBy()
        {
            var query = new SqlQuery();
            query.Select("Firstname");
            query.Select("Surname");
            query.From("People");
            query.OrderBy("Age");

            return query.ToString();
        }
    }
}
```

Programı çalıştırdığımızda fonksiyon aşağıdaki gibi bir sonuç verir:

```sql
SELECT Firstname, Surname FROM People ORDER BY Age
```

İlk satırda SqlQuery nesnemizi yegane parametresiz constructor’ı ile oluşturduk. Eğer bu noktada ToString() metodunu çağırsaydık aşağıdaki gibi bir sonuç alacaktık:

```sql
SELECT FROM
```

SqlQuery sorgunuza herhangi bir doğrulama yapmaz. Sadece çağrılarınızla ürettiğiniz sorguyu metne çevirir. SELECT ve FROM deyimleri siz hiçbir alan seçmeseniz de, tablo adı belirtmeseniz de metinsel gösterimde yukarıdaki gibi çıkacaktır. SqlQuery bu deyimleri içermeyen bir sorgu üretemez.

Ardından Select metodunu “Firstname” parametresiyle çağırdık. Sorgumuz aşağıdaki hale geldi:

```sql
SELECT Firstname FROM
```

Yine Select metodunu, bu sefer “Surname” parametresiyle çağırınca, SqlQuery bir önceki seçilen alan ile “Surname” arasına virgül koyarak sorguyu aşağıdaki gibi üretti:

```sql
SELECT Firstname, Surname FROM
```

From ve OrderBy metodlarını da çağırarak sorgumuza nihai halini verdik:

```sql
SELECT Firstname, Surname FROM People ORDER BY Age
```


##METOD ÇAĞIRIM SIRASI ve ETKİSİ

Örnek koddaki “From”, “OrderBy” ve “Select” içeren satırları hangi sırada yazarsak yazalım sonuç değişmeyecekti. Ancak aşağıdaki gibi Select çağrılarının sırası değişirse yani Surname alanını Firstname’den önce seçseydik...

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing2_SimpleSelectReordered()
        {
            var query = new SqlQuery();
            query.OrderBy("Age");
            query.Select("Surname");
            query.From("People");
            query.Select("Firstname");

            return query.ToString();
        }
    }
}
```

...sonuçta, sadece alanların SELECT ifadesinde görünme sıraları değişecekti:

```sql
SELECT Surname, Firstname FROM People ORDER BY Age
```


##ZİNCİRLEME METOD ÇAĞIRIMI (METHOD CHAINING)

Yukarıdaki kod listelerine dikkatli bakınca sorgumuzu düzenlediğimiz her satıra “query.” ile başladığımızı ve bunun göze pek de hoş gelmediğini farkedebilirsiniz. 

SqlQuery’nin, method chaining (zincirleme metod çağrımı) özelliğinden faydalanarak, bu sorguyu daha okunaklı ve kolay bir şekilde yazabiliriz: 

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing3_SimpleSelectMethodChaining()
        {
            var query = new SqlQuery()
                .Select("Firstname")
                .Select("Surname")
                .From("People")
                .OrderBy("Age");

            return query.ToString();
        }
    }
}
```

jQuery yada LINQ kullanıyorsanız bu zincirleme çağırım şekline aşina olmalısınız.

İstersek “query” yerel değişkenininden de kurtulabiliriz:

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing4_SimpleSelectRemoveVariable()
        {
            return new SqlQuery()
                .Select("Firstname")
                .Select("Surname")
                .From("People")
                .OrderBy("Age")
                .ToString();
        }
    }
}
```

FROM METODU

```csharp
public SqlQuery From(string table);
```

SqlQuery’nin From metodu, sorgunun FROM ifadesini üretmek için en az (ve genellikle) bir kez çağrılmalıdır. İlk çağırdığınızda sorgunuzun ana tablo ismini belirlemiş olursunuz.

İkinci bir kez çağırırsanız, verdiğiniz tablo ismi, sorgunun FROM kısmına, asıl tablo adıyla aralarına virgül konarak eklenir. Bu durumda da CROSS JOIN yapmış olursunuz.

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing5_CrossJoin()
        {
            return new SqlQuery()
                .Select("Firstname")
                .Select("Surname")
                .From("People")
                .From("City")
                .From("Country")
                .OrderBy("Age")
                .ToString();
        }
    }
}
```

Oluşan sorgu aşağıdaki gibi olacaktır:

```sql
SELECT Firstname, Surname FROM People, City, Country ORDER BY Age
```

##FROM AS METODU

```csharp
public SqlQuery FromAs(string table, string tableAlias);
```

Sorgularımız uzadıkça ve içindeki JOIN sayısı arttıkça, tablolarımıza kısa adlar (alias) vermeye başlarız. 

Bunun için SqlQuery’nin FromAs metodundan faydalanabiliriz.

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing6_FromAs()
        {
            return new SqlQuery()
                .Select("p.Firstname")
                .Select("p.Surname")
                .Select("c.CityName")
                .Select("o.CountryName")
                .FromAs("People", "p")
                .FromAs("City", "c")
                .FromAs("Country", "o")
                .OrderBy("p.Age")
                .ToString();
        }
    }
}
```

```sql
SELECT p.Firstname, p.Surname, c.CityName, o.CountryName FROM People p, City c, Country o ORDER BY p.Age
```

Görüleceği üzere alanları seçerken de başlarına tablolarına atadığımız kısa adları (p.Surname gibi) getirdik. Bu sayede tablolardaki alan isimleri çakışsa da (aynı alan adı People, City, Country tablolarında olsa da) sorun çıkmasını engellemiş olduk.


##ALIAS NESNESİ ve SQLQUERY İLE KULLANIMI

Kod Listesi 1.6’daki örnek sorgumuzda tablolara kısa adlar atamıştık. Bu kısa adları SqlQuery ile birlikte kullanabileceğimiz Alias nesnesleri olarak ta tanımlayabiliriz.

```csharp
var p = new Alias("p");
```

Alias aslında bir string e verdiğiniz ad gibidir. Fakat string ten farklı olarak, SQL ifadelerinde kullanacabileceğimiz “kısa ad”.”alan adı” şeklinde metinleri, indeksleyicisi (indexer’ı) aracılığıyla üretmemize yardımcı olur:

```csharp
p["Surname"]
```

```
p.Surname
```

Sorgumuzu Alias nesnesinden faydalanarak düzenleyelim:

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string Sample1_Listing7_FromAsAlias()
        {
            var p = new Alias("p");
            var c = new Alias("c");
            var o = new Alias("o");

            return new SqlQuery()
                .Select(p["Firstname"])
                .Select(p["Surname"])
                .Select(c["CityName"])
                .Select(o["CountryName"])
                .FromAs("People", p)
                .FromAs("City", c)
                .FromAs("Country", o)
                .OrderBy(p["Age"])
                .ToString();
        }
    }
}
```

```sql
SELECT p.Firstname, p.Surname, c.CityName, o.CountryName FROM People p, City c, Country o ORDER BY p.Age
```

Görüldüğü gibi sonuç aynı, ancak yazdığımız kod biraz daha uzadı. Peki Alias nesnesi kullanmak burada bize ne kazandırdı?

Şu haliyle değerlendirildiğinde avantajı ilk bakışta görülmeyebilir. Ancak aşağıdaki koddaki gibi alan isimlerini tutan sabitlerimiz olsaydı…

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        const string People = "People";
        const string Firstname = "Firstname";
        const string Surname = "Surname";
        const string Age = "Age";
        const string CityName = "CityName";
        const string CountryName = "CountryName";

        public static string Sample1_Listing8_UsingFieldNameConsts()
        {
            var p = new Alias("p");
            var c = new Alias("c");
            var o = new Alias("o");

            return new SqlQuery()
                .Select(p[Firstname])
                .Select(p[Surname])
                .Select(c[CityName])
                .Select(o[CountryName])
                .FromAs("People", p)
                .FromAs("City", c)
                .FromAs("Country", o)
                .OrderBy(p[Age])
                .ToString();
        }
    }
}
```

…Visual Studio’nun IntelliSense özelliğinden daha çok faydalanmış ve birçok yazım hatasını derleme anında yakalamış olabilirdik.

Tabi örnekteki gibi, her sorguyu yazmadan önce, alan isimlerini sabit olarak tanımlamak çok ta mantıklı ve kolay değil. Dolayısıyla bunların merkezi bir yerde tanımlanmış olması lazım (hatta, direk entity tanımımızdan bulunması, ki buna daha sonra değineceğiz)

