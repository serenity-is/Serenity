using System.IO;

namespace Serenity.Web
{
    public interface IUploadProcessor
    {
        ProcessedUploadInfo Process(Stream fileContent, string filename, IUploadEditor attr);
    }
}