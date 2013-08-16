using System;

namespace Serenity.Data
{
    public interface ICaptureLogRow : IIsActiveRow
    {
        Int32Field ChangingUserIdField { get; }
        DateTimeField ValidFromField { get; }
        DateTimeField ValidUntilField { get; }
    }
}