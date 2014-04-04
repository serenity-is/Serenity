namespace Serenity.Data
{
    public interface IUpdateLogRow
    {
        IIdField UpdateUserIdField { get; }
        DateTimeField UpdateDateField { get; }
    }

    public interface IInsertLogRow
    {
        IIdField InsertUserIdField { get; }
        DateTimeField InsertDateField { get; }
    }

    public interface ILoggingRow : IUpdateLogRow, IInsertLogRow
    {
    }
}