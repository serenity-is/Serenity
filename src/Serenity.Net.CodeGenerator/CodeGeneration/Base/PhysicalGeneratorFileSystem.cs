namespace Serenity.CodeGeneration;

public class PhysicalGeneratorFileSystem : PhysicalFileSystem, IGeneratorFileSystem
{
    public DateTime GetLastWriteTime(string path)
    {
        return System.IO.File.GetLastWriteTime(path);
    }

#if ISSOURCEGENERATOR
    // https://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path/32113484#32113484
    public override string GetRelativePath(string fromPath, string toPath)
    {
        if (string.IsNullOrEmpty(fromPath))
            throw new ArgumentNullException("fromPath");

        if (string.IsNullOrEmpty(toPath))
            throw new ArgumentNullException("toPath");

        fromPath = fromPath.Replace("/", "\\", StringComparison.Ordinal);
        toPath = toPath.Replace("/", "\\", StringComparison.Ordinal);

        if (!fromPath.Contains(':', StringComparison.Ordinal))
            fromPath = "z:\\" + fromPath;

        if (!toPath.Contains(':', StringComparison.Ordinal))
            toPath = "z:\\" + toPath;

        Uri fromUri = new(AppendDirectorySeparatorChar(fromPath));
        Uri toUri = new(AppendDirectorySeparatorChar(toPath));

        if (fromUri.Scheme != toUri.Scheme)
            return toPath;

        Uri relativeUri = fromUri.MakeRelativeUri(toUri);
        string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

        if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
            relativePath = relativePath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

        return relativePath;
    }

    private static string AppendDirectorySeparatorChar(string path)
    {
        if (!System.IO.Path.HasExtension(path) &&
            !path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            return path + System.IO.Path.DirectorySeparatorChar;

        return path;
    }
#endif
}