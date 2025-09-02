
namespace Serenity.Data;

/// <summary>
/// Capture logging handler abstraction
/// </summary>
public interface ICaptureLogHandler
{
    /// <summary>
    /// Logs the capture log operation
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="old">Old record</param>
    /// <param name="row">New record</param>
    /// <param name="userId">The user ID performing the operation</param>
    void Log(IUnitOfWork uow, IRow old, IRow row, object userId);
}