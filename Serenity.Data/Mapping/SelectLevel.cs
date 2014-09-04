namespace Serenity.Data
{
    public enum SelectLevel
    {
        /// <summary>
        ///   Eğer tablo alanıysa List'e, Foreign alansa Explicit'e eşit</summary>
        Default = 0,
        /// <summary>
        ///   Her durumda alan seçilir (özellikle çıkarılmak istense bile)</summary>
        Always = 1,
        /// <summary>
        ///   Kolon seçimi modu Lookup/Lİst/Details ise ya da özel olarak istendiyse seçilir</summary>
        Lookup = 2,
        /// <summary>
        ///   Kolon seçimi modu List/Detials ise ya da özel olarak istendiyse seçilir</summary>
        List = 3,
        /// <summary>
        ///   Kolon seçimi modu Details ise ya da özel olarak istendiyse seçilir</summary>
        Details = 4,
        /// <summary>
        ///   Kolon ancak özel olarak istendiyse seçilir</summary>
        Explicit = 5,
        /// <summary>
        ///   Kolon özel olarak istense bile seçilmez (şifre gibi gizli alanlar için)</summary>
        Never = 6
    }
}