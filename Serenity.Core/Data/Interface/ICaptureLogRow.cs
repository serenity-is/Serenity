using System;

namespace Serenity.Data
{
    public interface ICaptureLogRow : IIsActiveRow
    {
        IIdField ChangingUserIdField { get; }
        DateTimeField ValidFromField { get; }
        DateTimeField ValidUntilField { get; }
    }
}