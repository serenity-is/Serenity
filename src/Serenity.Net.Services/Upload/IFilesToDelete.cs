namespace Serenity.Web
{
    public interface IFilesToDelete
    {
        void RegisterNewFile(string file);
        void RegisterOldFile(string file);
        void KeepNewFiles();
    }
}