namespace Serenity.Web
{
    public static class FilesToDeleteExtensions
    {
        public static void RegisterFilesToDelete(this IUnitOfWork unitOfWork, FilesToDelete filesToDelete)
        {
            unitOfWork.OnCommit += delegate ()
            {
                try
                {
                    filesToDelete.KeepNewFiles();
                    filesToDelete.Dispose();
                }
                catch
                {
                }
            };

            unitOfWork.OnRollback += delegate ()
            {
                try
                {
                    filesToDelete.Dispose();
                }
                catch
                {
                }
            };
        }
    }
}