using Serenity.Services;
using System;

namespace Serenity.Data
{
    public interface ICaptureLogHandler
    {
        void Log(IUnitOfWork uow, Row old, Row row, object userId);
    }
}