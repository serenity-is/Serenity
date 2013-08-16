using System.ComponentModel;

namespace Serenity.Data
{
    public enum CustomFieldType
    {
        [Description("Metin (String)")]
        String = 1,
        [Description("Tamsayı (Int32)")]
        Int32 = 2,
        [Description("Tamsayı (Int64)")]
        Int64 = 3,
        [Description("Ondalıklı Sayı (Decimal)")]
        Decimal = 4,
        [Description("Tarih (Date)")]
        Date = 5,
        [Description("Tarih / Zaman (DateTime)")]
        DateTime = 6,
        [Description("Mantıksal (Boolean)")]
        Boolean = 7
    }
}