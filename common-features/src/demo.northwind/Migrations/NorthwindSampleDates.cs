using System.Data;
using System.Globalization;

namespace Serenity.Demo.Northwind;

/// <summary>
/// Moves the dates of the Northwind sample orders forward, so the latest order is today while keeping how far apart the dates
/// are, which lets the sample data look recent, e.g. for a dashboard of the last 30 days. It uses the date functions of the
/// dialect, so it works with all the supported databases.
/// </summary>
public static class NorthwindSampleDates
{
    /// <summary>
    /// Moves the dates of the orders, and the last contact dates of the customer details, forward by the number of days
    /// between the latest order and today. It does nothing when the latest order is already today or later, so it can
    /// be run any number of times.
    /// </summary>
    /// <param name="connection">An open connection to the Northwind database.</param>
    /// <param name="transaction">The transaction of the connection, if any.</param>
    /// <param name="dialect">The dialect of the database.</param>
    /// <param name="today">The date to move the latest order to, which is the current date by default.</param>
    /// <returns>The number of days the dates are moved by, which is zero when they are not.</returns>
    public static int Shift(IDbConnection connection, IDbTransaction? transaction, ISqlDialect dialect, DateTime? today = null)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(dialect);

        string q(string name) => dialect.QuoteIdentifier(name);

        void execute(string sql)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        object? latest;
        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = $"SELECT MAX({q("OrderDate")}) FROM {q("Orders")}";
            latest = command.ExecuteScalar();
        }

        if (latest is null or DBNull)
            return 0;

        var days = (int)(today ?? DateTime.Today).Date.Subtract(Convert.ToDateTime(latest, CultureInfo.InvariantCulture).Date).TotalDays;
        if (days <= 0)
            return 0;

        string add(string column) => new DateAddAttribute(DateParts.Day, days, q(column)).ToString(dialect);

        // a date that is null stays null
        execute($"UPDATE {q("Orders")} SET {q("OrderDate")} = {add("OrderDate")}, " +
            $"{q("RequiredDate")} = {add("RequiredDate")}, {q("ShippedDate")} = {add("ShippedDate")}");

        execute($"UPDATE {q("CustomerDetails")} SET {q("LastContactDate")} = {add("LastContactDate")} " +
            $"WHERE {q("LastContactDate")} IS NOT NULL");

        return days;
    }
}
