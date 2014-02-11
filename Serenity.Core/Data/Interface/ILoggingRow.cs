namespace Serenity.Data
{
    public interface ILoggingRow
    {
        IIdField InsertUserIdField { get; }
        DateTimeField InsertDateField { get; }
        IIdField UpdateUserIdField { get; }
        DateTimeField UpdateDateField { get; }
    }
}