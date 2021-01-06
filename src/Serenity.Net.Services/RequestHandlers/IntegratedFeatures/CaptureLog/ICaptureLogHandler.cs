
namespace Serenity.Data
{
    public interface ICaptureLogHandler
    {
        void Log(IUnitOfWork uow, IRow old, IRow row, object userId);
    }
}