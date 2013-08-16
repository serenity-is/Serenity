namespace Serenity.Data
{
    public interface ILoggingRow
    {
        Int32Field InsertUserIdField { get; }
        DateTimeField InsertDateField { get; }
        Int32Field UpdateUserIdField { get; }
        DateTimeField UpdateDateField { get; }
    }
}