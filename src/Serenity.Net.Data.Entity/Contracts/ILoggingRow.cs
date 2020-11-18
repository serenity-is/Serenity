namespace Serenity.Data
{
    public interface IUpdateLogRow
    {
        Field UpdateUserIdField { get; }
        DateTimeField UpdateDateField { get; }
    }

    public interface IInsertLogRow
    {
        Field InsertUserIdField { get; }
        DateTimeField InsertDateField { get; }
    }

    public interface IDeleteLogRow
    {
        Field DeleteUserIdField { get; }
        DateTimeField DeleteDateField { get; }
    }

    public interface ILoggingRow : IUpdateLogRow, IInsertLogRow
    {
    }
}