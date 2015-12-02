
namespace Serenity.Data
{
    public interface ICaptureLogRow : IIdRow
    {
        Int16Field OperationTypeField { get; }
        Field ChangingUserIdField { get; }
        DateTimeField ValidFromField { get; }
        DateTimeField ValidUntilField { get; }
    }
}