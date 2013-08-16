using System;

namespace Serenity.Data
{

    /// <summary>
    ///   Bir alanýn temel özelliklerini belirleyen flag'lar</summary>
    /// <remarks>
    ///   Bilgi amaçlý olarak kullanýldýðý gibi, PagerPanel gibi bazý nesneler tarafýndan dinamik 
    ///   INSERT, UPDATE sorgularýnýn üretilmesinde de kullanýlýr. Örneðin Insertable olmayan alanlar 
    ///   deðerleri alýnsa da INSERT sorgusuna dahil edilmez. ConvertEmptyStringToNull ise
    ///   INSERT, UPDATE esnasýnda grid ya da details view den gelen string alan deðerlerinin SQL 
    ///   parametrelerine çevrilirken, eðer deðer boþsa NULL yapýlýp yapýlmayacaðýný belirler.</remarks>
    [Flags]
    public enum FieldFlags
    {
        /// <summary>
        ///   Hiçbir flag set edilmemiþ.</summary>
        Internal = 0,
        /// <summary>
        ///   INSERT esnasýnda deðer verilebilir mi? Diðer tablolardan gelen alanlar ve 
        ///   identity gibi alanlarýn bu flag'i olmamalý.</summary>
        Insertable = 1,
        /// <summary>
        ///   UPDATE esnasýnda güncellenebilir mi? Diðer tablolardan gelen alanlar ve identity 
        ///   gibi alanlarýn bu flag'i olmamalý.</summary>
        Updatable = 2,
        /// <summary>
        ///   NULL olabilir mi. Gridview ve Detailsview'de validator'larýn Required özelliklerinin
        ///   belirlenmesi için.</summary>
        NotNull = 4,
        /// <summary>
        ///   Alan anahtar saha ya da sahalardan biri.</summary>
        PrimaryKey = 8,
        /// <summary>
        ///   Otomatik artan integer saha.</summary>
        AutoIncrement = 16,
        /// <summary>
        ///   LEFT OUTER JOIN ile diðer tablolardan gelen alanlarýn bu flag'i set edilmeli.</summary> 
        Foreign = 32,
        /// <summary>
        ///   Hesaplanan alan, Foreign varsa diðer tablolardan da gelen alanlarýn karýþýmý olabilir.</summary>
        Calculated = 64,
        /// <summary>
        ///   Just reflects another field value (e.g. negative/absolute of it), so doesn't have client and server side storage of its own,
        ///   and setting it just sets another field.</summary>
        Reflective = 128,
        /// <summary>
        ///   Field which is just a container to use in client side code (might also be client side calculated / reflective).</summary>
        ClientSide = 256,
        /// <summary>
        ///   Trim.</summary>
        Trim = 512,
        /// <summary>
        ///   TrimToEmpty.</summary>
        TrimToEmpty = 512 + 1024,
        /// <summary>
        ///   DenyFiltering.</summary>
        DenyFiltering = 2048,
        /// <summary>
        ///   Yeni bir <see cref="Field"/> üretirken alan özellikleri belirtilmediðinde
        ///   kullanýlacak öndeðer özellikler. Eklenebilir, güncellenebilir, NULL yapýlabilir,
        ///   boþ string'ler NULL'a çevrilir.</summary>
        Default = Insertable | Updatable | Trim,
        /// <summary>
        ///   Default'tan farký zorunlu alan olmasý.</summary>
        Required = Default | NotNull,
        /// <summary>
        ///   Otomatik artan anahtar ID alanlarý için kullanýlacak flag seti.</summary>
        Identity = PrimaryKey | AutoIncrement | NotNull
    }
}