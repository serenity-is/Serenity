using Serenity.Data;
using System.Data.SqlClient;

namespace Serenity.Data
{
    /// <summary>
    ///   Basic interface for rows that has a display order field and provides a default sorting order.</summary>
    public interface IDisplayOrderRow
    {
        /// <summary>
        ///   Gets display order field for this row.</summary>
        Int32Field DisplayOrderField { get; }
        /*/// <summary>
        ///   Gets default sorting order for this row's records</summary>
        SortingOrder DefaultSortingOrder { get; }*/
    }
}