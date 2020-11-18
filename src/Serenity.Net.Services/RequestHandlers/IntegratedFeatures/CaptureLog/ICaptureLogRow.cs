
namespace Serenity.Data
{
    public interface ICaptureLogRow : IIdRow
    {
        EnumField<CaptureOperationType> OperationTypeField { get; }
        Field ChangingUserIdField { get; }
        DateTimeField ValidFromField { get; }
        DateTimeField ValidUntilField { get; }
    }
}