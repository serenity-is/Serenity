namespace Serenity.Web
{
    public class CopyTemporaryFileResult
    {
        public string TemporaryFilePath { get; set; }
        public string DbFileName { get; set; }
        public string OriginalName { get; set; }
        public string FilePath { get; set; }
        public bool HasThumbnail { get; set; }
        public long FileSize { get; set; }
    }
}