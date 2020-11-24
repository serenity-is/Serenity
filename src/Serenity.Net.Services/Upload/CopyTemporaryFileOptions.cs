namespace Serenity.Web
{
    public class CopyTemporaryFileOptions : FormatFilenameOptions
    {
        public string TemporaryFile { get; set; }
        public IFilesToDelete FilesToDelete { get; set; }
    }
}