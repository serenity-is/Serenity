
namespace Serenity.Web.FilterPanel
{
    /// <summary>
    ///   Filter field types for PI Filter Panel</summary>
    public enum FilterFieldType
    {
        /// <summary>boolean</summary>
        Boolean,
        /// <summary>date</summary>
        DateOnly,
        /// <summary>date/time in local</summary>
        DateTimeLocal,
        /// <summary>date/time in UTC</summary>
        DateTimeUTC,
        /// <summary>decimal</summary>
        Decimal,
        /// <summary>integer</summary>
        Integer,
        /// <summary>list</summary>
        List,
        /// <summary>lookup</summary>
        Lookup,
        /// <summary>string</summary>
        String,
        /// <summary>string</summary>
        Custom       
    }
}
