
using System;

namespace Serenity.Data
{
    public interface ICaptureLogHandler
    {
        void LogSave(IUnitOfWork uow, Row row, Int64 userId);
        void LogDelete(IUnitOfWork uow, Row old, Int64 userId);
    }
}