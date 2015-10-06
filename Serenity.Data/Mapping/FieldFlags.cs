using System;

namespace Serenity.Data
{

    /// <summary>
    ///   Bir alanın temel özelliklerini belirleyen flag'lar</summary>
    /// <remarks>
    ///   Bilgi amaçlı olarak kullanıldığı gibi, PagerPanel gibi bazı nesneler tarafından dinamik 
    ///   INSERT, UPDATE sorgularının üretilmesinde de kullanılır. Örneğin Insertable olmayan alanlar 
    ///   değerleri alınsa da INSERT sorgusuna dahil edilmez. ConvertEmptyStringToNull ise
    ///   INSERT, UPDATE esnasında grid ya da details view den gelen string alan değerlerinin SQL 
    ///   parametrelerine çevrilirken, eğer değer boşsa NULL yapılıp yapılmayacağını belirler.</remarks>
    [Flags]
    public enum FieldFlags
    {
        /// <summary>
        ///   Hiçbir flag set edilmemiş.</summary>
        None = 0,
        /// <summary>
        ///   Hiçbir flag set edilmemiş.</summary>
        Internal = 0,
        /// <summary>
        ///   INSERT esnasında değer verilebilir mi? Diğer tablolardan gelen alanlar ve 
        ///   identity gibi alanların bu flag'i olmamalı.</summary>
        Insertable = 1,
        /// <summary>
        ///   UPDATE esnasında güncellenebilir mi? Diğer tablolardan gelen alanlar ve identity 
        ///   gibi alanların bu flag'i olmamalı.</summary>
        Updatable = 2,
        /// <summary>
        ///   NULL olabilir mi. Gridview ve Detailsview'de validator'ların Required özelliklerinin
        ///   belirlenmesi için.</summary>
        NotNull = 4,
        /// <summary>
        ///   Alan anahtar saha ya da sahalardan biri.</summary>
        PrimaryKey = 8,
        /// <summary>
        ///   Otomatik artan integer saha.</summary>
        AutoIncrement = 16,
        /// <summary>
        ///   LEFT OUTER JOIN ile diğer tablolardan gelen alanların bu flag'i set edilmeli.</summary> 
        Foreign = 32,
        /// <summary>
        ///   Hesaplanan alan, Foreign varsa diğer tablolardan da gelen alanların karışımı olabilir.</summary>
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
        ///   Unique.</summary>
        Unique = 4096,
        /// <summary>
        ///   Yeni bir <see cref="Field"/> üretirken alan özellikleri belirtilmediğinde
        ///   kullanılacak öndeğer özellikler. Eklenebilir, güncellenebilir, NULL yapılabilir,
        ///   boş string'ler NULL'a çevrilir.</summary>
        Default = Insertable | Updatable | Trim,
        /// <summary>
        ///   Default'tan farkı zorunlu alan olması.</summary>
        Required = Default | NotNull,
        /// <summary>
        ///   Otomatik artan anahtar ID alanları için kullanılacak flag seti.</summary>
        Identity = PrimaryKey | AutoIncrement | NotNull
    }
}