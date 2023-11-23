#nullable enable

namespace Serenity;

/// <summary>
/// Physical file sytem
/// </summary>
public class TemporaryPhysicalFileSystem : PhysicalFileSystem, ITemporaryFileSystem
{
    /// <inheritdoc/>
    public TemporaryFileInfo[] GetTemporaryFileInfos(string path)
    {
        return new System.IO.DirectoryInfo(path).GetFiles().Select(x => new TemporaryFileInfo
        {
            Name = x.Name,
            FullName = x.FullName,
            CreationTime = x.CreationTime
        }).ToArray();
    }
}
