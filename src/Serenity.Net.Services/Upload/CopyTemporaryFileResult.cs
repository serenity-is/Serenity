namespace Serenity.Web
{
    public class CopyTemporaryFileResult
    {
        public string Path { get; set; }
        public string OriginalName { get; set; }
        public bool HasThumbnail { get; set; }
        public long FileSize { get; set; }
    }
}