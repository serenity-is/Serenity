namespace Serenity.Web
{
    public interface IDiskUploadStore : IUploadStore
    {
        string DbFilePath(string dbFileName);
        public string RootPath { get; }
    }
}