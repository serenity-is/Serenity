namespace Serenity.Web
{
    public class CopyTemporaryFileOptions : FormatDbFilenameOptions
    {
        public string DbTemporaryFile { get; set; }
        public IFilesToDelete FilesToDelete { get; set; }
    }
}