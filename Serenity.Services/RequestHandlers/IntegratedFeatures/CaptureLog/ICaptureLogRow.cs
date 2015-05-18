
namespace Serenity.Data
{
    public interface ICaptureLogRow : IIsActiveDeletedRow
    {
        IIdField ChangingUserIdField { get; }
        DateTimeField ValidFromField { get; }
        DateTimeField ValidUntilField { get; }
    }
}