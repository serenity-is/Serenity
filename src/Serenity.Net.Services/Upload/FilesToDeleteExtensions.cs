namespace Serenity.Web;

/// <summary>
/// Extension methods for <see cref="FilesToDelete"/>
/// </summary>
public static class FilesToDeleteExtensions
{
    /// <summary>
    /// Registers a <see cref="FilesToDelete"/> in the target unit of work. 
    /// This deletes the old files on commit, and new files on rollback
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="filesToDelete">Files to delete container</param>
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