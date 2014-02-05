# SqlQuery Nesnesi
Dinamik SQL SELECT sorguları hazırlamanız için StringBuilder benzeri bir nesnedir (aslında bu nesne kendi içinde birçok StringBuilder’dan faydalanır).


## Sağladığı Fayda ve Kolaylıklar

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


## Basit Bir SELECT Sorgusu

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string SimpleSelectFromOrderBy()
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


##Metod Çağırım Sırası ve Etkisi

Örnek koddaki “From”, “OrderBy” ve “Select” içeren satırları hangi sırada yazarsak yazalım sonuç değişmeyecekti. Ancak aşağıdaki gibi Select çağrılarının sırası değişirse yani Surname alanını Firstname’den önce seçseydik...

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string ReorderedQuery()
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


##Zincirleme Metod Çağrımı (Method Chaining)

Yukarıdaki kod listelerine dikkatli bakınca sorgumuzu düzenlediğimiz her satıra “query.” ile başladığımızı ve bunun göze pek de hoş gelmediğini farkedebilirsiniz. 

SqlQuery’nin, method chaining (zincirleme metod çağrımı) özelliğinden faydalanarak, bu sorguyu daha okunaklı ve kolay bir şekilde yazabiliriz: 

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string MethodChaining()
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
        public static string SimpleSelectRemoveVariable()
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

##Select Metodu

```csharp
public SqlQuery Select(string expression)
```

Şu ana kadar verdiğimiz örneklerde, Select metodunun yukarıdaki overload'ını kullandık. Expression (ifade) parametresi, bir alan adı ya da (Adi + Soyadi) gibi bir SQL ifadesi olabilir. Bu metodu her çağırdığınızda sorgunun SELECT listesine, verdiğiniz alan adı ya da ifade eklenir (araya virgül konarak).

Tek bir çağırımda, birden fazla alan seçmek isterseniz, şu overload'ı kullanabilirsiniz:

```csharp
public SqlQuery Select(params string[] expressions)
```

Örneğin:

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string SelectMultipleFieldsInOneCall()
        {
            return new SqlQuery()
                .Select("Firstname", "Surname", "Age", "Gender")
                .From("People")
                .ToString();
        }
    }
}
```

```sql
SELECT Firstname, Surname, Age, Gender FROM People
```

Seçtiğiniz bir alana kısa ad (column alias) atamak isterseniz SelectAs metodundan faydalanabilirsiniz:

```csharp
public SqlQuery SelectAs(string expression, string alias)
```

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string SelectAs()
        {
            return new SqlQuery()
                .SelectAs("(Firstname + Surname)", "Fullname")
                .From("People")
                .ToString();
        }
    }
}
```

```sql
SELECT (Firstname + Surname) Fullname FROM People
```


##From Metodu

```csharp
public SqlQuery From(string table)
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
        public static string CrossJoinWithFrom()
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

##Alias Nesnesi ve SqlQuery ile Kullanımı

Sorgularımız uzadıkça ve içindeki JOIN sayısı arttıkça, hem alan adı çakışmalarını engellemek, hem de istenen alanlara daha kolay ulaşmak için, kullandığımız tablolara aşağıdaki gibi kısa adlar (alias) vermeye başlarız. 

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string FromWithStringAliases()
        {
            return new SqlQuery()
                .Select("p.Firstname")
                .Select("p.Surname")
                .Select("p.CityName")
                .Select("p.CountryName")
                .From("Person p")
                .From("City c")
                .From("Country o")
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

Sorgu içinde kullandığımız bu kısa adları, dilersek SqlQuery ile birlikte kullanabileceğimiz Alias nesnesleri olarak ta tanımlayabiliriz.

```csharp
var p = new Alias("Person", "p");
```

Alias aslında bir string e verdiğiniz ad gibidir. Fakat string ten farklı olarak, SQL ifadelerinde kullanacabileceğimiz “kısa ad”.”alan adı” şeklinde metinleri, "+" operatörü aracılığıyla üretmemize yardımcı olur:

```csharp
p + "Surname"
```

>p.Surname

Bu işlem C#'ın "+" operatörünün overload edilmesi sayesinde gerçekleşmektedir. Bir alias'ı bir alan adı ile topladığınızda, alias'ın kısa adı ve alan adı, aralarına "." konarak birleştirilir 

> ne yazık ki C#'ın member access operatorünü (".") overload edemiyoruz, bu yüzden "+" kullanılmak durumunda.

Sorgumuzu Alias nesnesinden faydalanarak düzenleyelim:

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string FromUsingAlias()
        {
            var p = new Alias("People", "p");
            var c = new Alias("City", "c");
            var o = new Alias("Country", "o");

            return new SqlQuery()
                .Select(p + "Firstname")
                .Select(p + "Surname")
                .Select(c + "CityName")
                .Select(o + "CountryName")
                .From(p)
                .From(c)
                .From(o)
                .OrderBy(p + "Age")
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
        const string Firstname = "Firstname";
        const string Surname = "Surname";
        const string Age = "Age";
        const string CityName = "CityName";
        const string CountryName = "CountryName";

        public static string UsingFieldNameConsts()
        {
            var p = new Alias("People", "p");
            var c = new Alias("City", "c");
            var o = new Alias("Country", "o");

            return new SqlQuery()
                .Select(p + Firstname)
                .Select(p + Surname)
                .Select(c + CityName)
                .Select(o + CountryName)
                .From(p)
                .From(c)
                .From(o)
                .OrderBy(p + Age)
                .ToString();
        }
    }
}
```

…Visual Studio’nun IntelliSense özelliğinden daha çok faydalanmış ve birçok yazım hatasını derleme anında yakalamış olabilirdik.

Tabi örnekteki gibi, her sorguyu yazmadan önce, alan isimlerini sabit olarak tanımlamak çok ta mantıklı ve kolay değil. Dolayısıyla bunların merkezi bir yerde tanımlanmış olması lazım (hatta, direk entity tanımımızdan bulunması, ki buna daha sonra değineceğiz)

Bu örnekte ayrıca SqlQuery.From'un aşağıdaki gibi Alias parametresi alan overload'ından faydalandık:

```csharp
public SqlQuery From(Alias alias)
```

Bu fonksiyon çağrıldığında, SQL sorgusunun FROM ifadesine, alias oluşturulurken tanımlanan tablo, kısa adıyla birlikte eklenir.


Eğer alias'ınızı oluştururken bir tablo adı belirtmediyseniz (new Alias("c") gibi) şu overload'ı kullanabilirsiniz:

```csharp
public SqlQuery From(string table, Alias alias)
```

##OrderBy Metodu

```csharp
public SqlQuery OrderBy(string expression, bool desc = false)
```

OrderBy metodu da Select gibi bir alan adı ya da ifadesiyle çağrılabilir. "Desc" opsiyonel parametresine true atarsanız, alan adı ya da ifadenizin sonuna " DESC" getirilir.

OrderBy metodu, verdiğiniz ifadeleri ORDER BY deyiminin sonuna ekler. Bazen alanı listenin başına getirmek te isteyebiliriz. Örneğin çeşitli alanlara göre sıralanmış bir sorgu hazırladıktan sonra, kullanıcı arayüzünde tıklanan kolona göre sıralamanın değişmesi (önceki sıralamayı tümüyle kaybetmeden) gerekebilir. Bunun için SqlQuery, OrderByFirst metodunu sağlar:


```csharp
public SqlQuery OrderByFirst(string expression, bool desc = false)
```

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string OrderByFirst()
        {
            var query = new SqlQuery()
                .Select("Firstname")
                .Select("Surname")
                .From("Person")
                .OrderBy("PersonID");

			return query.OrderByFirst("Age")
                .ToString();
        }
    }
}
```

```sql
SELECT Firstname, Surname FROM Person ORDER BY Age, PersonID
```

Order by kullanmış olsaydık şunu elde edecektik:

```sql
SELECT Firstname, Surname FROM Person ORDER BY PersonID, Age
```

##Distinct Metodu

```csharp
public SqlQuery Distinct(bool distinct)
```

DISTINCT deyimini içeren bir sorgu üretmek istediğinizde bu metodu kullanabilirsiniz:

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string DistinctMethod()
        {
			return new SqlQuery()
				.From("Person")
				.Distinct(true)
				.Select("Firstname")
				.ToString();
        }
    }
}
```

```sql
SELECT DISTINCT Firstname FROM Person 
```

##Group By Metodu

```csharp
public SqlQuery GroupBy(string expression)
```

GroupBy metodu bir alan adı ya da ifadesiyle çağrılır ve bu ifadeyi sorgunun GROUP BY deyiminin sonuna ekler.


```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string GroupByMethod()
        {
			new SqlQuery()
				.From("Person")
				.Select("Firstname", "Lastname", "Count(*)")
				.GroupBy("Firstname")
				.GroupBy("LastName")
				.ToString();
        }
    }
}
```

```sql
SELECT Firstname, Lastname, Count(*) FROM Person GROUP BY Firstname, LastName
```


##Having Metodu

```csharp
public SqlQuery Having(string expression)
```

GroupBy metodu ile birlikte kullanabileceğiniz Having metodu, mantıksal bir ifadeyle çağrılır ve bu ifadeyi sorgunun HAVING deyiminin sonuna ekler.


```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string HavingMethod()
        {
			new SqlQuery()
				.From("Person")
				.Select("Firstname", "Lastname", "Count(*)")
				.GroupBy("Firstname")
				.GroupBy("LastName")
				.Having("Count(*) > 5")
				.ToString();
        }
    }
}
```

```sql
SELECT Firstname, Lastname, Count(*) FROM Person GROUP BY Firstname, LastName HAVING Count(*) > 5
```

##Sayfalama İşlemleri (SKIP / TAKE / TOP / LIMIT)

```csharp
public SqlQuery Skip(int skipRows)

public SqlQuery Take(int rowCount)
```

SqlQuery, LINQ'de Take ve Skip olarak geçen sayfalama metodlarını destekler. Bunlardan Take, Microsoft SQL Server'da TOP'a karşılık gelirken, SKIP'in direk bir karşılığı olmadığından SqlQuery, ROW_NUMBER() fonksiyonundan faydalanır. Bu nedenle SQL server için SKIP kullanmak istediğinizde, sorgunuzda en az bir ORDER BY ifadesinin de olması gerekir.

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string SkipTakePaging()
        {
        	new SqlQuery()
        		.From("Person")
        		.Select("Firstname", "Lastname")
        		.OrderBy("PersonId")
        		.Skip(300)
        		.Take(100)
        		.ToString();
        }
    }
}
```

```sql
SELECT * FROM (
    SELECT TOP 400 Firstname, Lastname, ROW_NUMBER() OVER (ORDER BY PersonId) AS _row_number_ FROM Person
) _row_number_results_ WHERE _row_number_ > 300
```

##Farklı Veritabanları Desteği

Sayfalama işlemlerindeki örneğimizde, SqlQuery, Microsoft SQL Server 2008'e uygun bir sayfalama metodu kullandı.

Dialect metodu ile SQL query nin kullandığı DIALECT ya da sunucu tipini değiştirebiliriz:

public SqlQuery Dialect(SqlDialect dialect)

Şu an için aşağıdaki sunucu tipleri desteklenmektedir:

```csharp
[Flags]
public enum SqlDialect
{
    MsSql = 1,
    Firebird = 2,
    UseSkipKeyword = 512,
    UseRowNumber = 1024,
    UseOffsetFetch = 2048,
    MsSql2005 = MsSql | UseRowNumber,
    MsSql2012 = MsSql | UseOffsetFetch | UseRowNumber
}
```

Örneğin SqlDialect.MsSql2012'yi seçip, SQL Server 2012 ile gelen OFFSET FETCH deyimlerinden faydalanmak isteseydik:

```csharp
namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public static string SkipTakePaging()
        {
        	new SqlQuery()
        	    .Dialect(SqlDialect.MsSql2012)
        		.From("Person")
        		.Select("Firstname", "Lastname")
        		.OrderBy("PersonId")
        		.Skip(300)
        		.Take(100)
        		.ToString();
        }
    }
}
```

```sql
SELECT Firstname, Lastname FROM Person ORDER BY PersonId 
OFFSET 300 ROWS FETCH NEXT 100 ROWS ONLY
```

SqlDialect.Firebird'ü tercih etseydik şu şekilde bir sorgu oluşurdu:

```sql
SELECT FIRST 100 SKIP 300 Firstname, Lastname FROM Person ORDER BY PersonId
```

SqlQuery'nin Sunucu/Dialect desteği henüz mükemmel olmasa da, temel işlemlerde sorun çıkarmayacak düzeydedir.

Uygulamanızda tek tipte sunucu kullanıyorsanız, her sorgunuzda, dialect ayarlamak istemeyebilirsiniz. Bunun için SqlSettings.CurrentDialect'i değiştirmelisiniz. Örneğin aşağıdaki kodu program başlangıcında, ya da global.asax dosyanızdan çağırabilirsiniz: 

```csharp
SqlSettings.CurrentDialect = SqlDialect.MsSql2012;
```

