
namespace Serenity.Web
{
    public class ProcessedUploadInfo
    {
        public string ErrorMessage { get; set; }
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
        public bool IsImage { get; set; }
        public long FileSize { get; set; }
        public bool Success { get; set; }
        public string TemporaryFile { get; set; }
    }
}