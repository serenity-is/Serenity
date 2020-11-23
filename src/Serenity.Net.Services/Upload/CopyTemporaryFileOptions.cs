namespace Serenity.Web
{
    public class CopyTemporaryFileOptions : FormatDbFilenameOptions
    {
        public string TemporaryFile { get; set; }
        public IFilesToDelete FilesToDelete { get; set; }
    }
}