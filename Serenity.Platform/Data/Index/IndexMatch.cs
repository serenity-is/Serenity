namespace Serenity.Data
{
    /// <summary>
    ///   DbIndex.Match fonksiyonunun döndüreceği kayıt grubununun sınırlarını belirleyen 
    ///   operatör.</summary>
    public enum IndexMatch
    {
        /// <summary>sıralı alanları anahtar değere eşit olanlar</summary>
        EqualTo,
        /// <summary>sıralı alanları anahtar değerle aynı ya da konumu sonra olanlar</summary>
        GreaterEqual,
        /// <summary>sıralı alanlara göre konumları anahtar değerden sonra olanlar</summary>
        GreaterThan,
        /// <summary>sıralı alanları anahtar değerle aynı ya da konumu önce olanlar</summary>
        LessEqual,
        /// <summary>sıralı alanlara göre konumları anahtar değerden önce olanlar</summary>
        LessThan
    }
}