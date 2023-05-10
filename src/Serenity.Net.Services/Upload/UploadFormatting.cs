using Serenity.IO;
using Path = System.IO.Path;

namespace Serenity.Web;

/// <summary>
/// Contains helper methods for upload file name formatting
/// </summary>
public static class UploadFormatting
{
    /// <summary>
    /// Formats file name
    /// </summary>
    /// <param name="options">Formatting options</param>
    /// <exception cref="ArgumentNullException">options or options.OriginalName is null</exception>
    public static string FormatFilename(FormatFilenameOptions options)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        object groupKey;
        string s;
        object identity = options.EntityId;
        if (identity == null)
            groupKey = "_";
        else if (identity is Guid g)
        {
            s = g.ToString("N");
            identity = s;
            groupKey = s.Substring(0, 2);
        }
        else
        {
            s = identity.ToString();
            if (long.TryParse(s, out long l))
                groupKey = l / 1000;
            else if (s.Length == 0)
                groupKey = "_";
            else
                groupKey = s.SafeSubstring(0, 2);
        }

        var originalName = options.OriginalName;
        if (string.IsNullOrEmpty(originalName))
            throw new ArgumentNullException(nameof(originalName));

        var formatted = string.Format(options.Format, identity, groupKey, 
            TemporaryFileHelper.RandomFileCode(), DateTime.Now,
            Path.GetFileNameWithoutExtension(originalName)) + Path.GetExtension(options.OriginalName);

        if (options.PostFormat != null)
            formatted = options.PostFormat(formatted);

        return formatted;
    }

    /// <summary>
    /// Gets a display string for file name and size
    /// </summary>
    /// <param name="name">File name</param>
    /// <param name="bytes">Size in bytes</param>
    public static string FileNameSizeDisplay(string name, int bytes)
    {
        return name + " (" + FileSizeDisplay(bytes) + ')';
    }

    /// <summary>
    /// Gets a display string for a file size
    /// </summary>
    /// <param name="bytes">File size in bytes</param>
    public static string FileSizeDisplay(int bytes)
    {
        var byteSize = (Math.Round(bytes * 100m / 1024m) * 0.01m);
        var suffix = "KB";
        if (byteSize > 1000)
        {
            byteSize = (Math.Round(byteSize * 0.001m * 100m) * 0.01m);
            suffix = "MB";
        }

        var sizeParts = byteSize.ToString(Invariants.NumberFormat).Split('.');
        string value;
        if (sizeParts.Length > 1)
            value = sizeParts[0] + "." + sizeParts[1].Substring(0, 2);
        else
            value = sizeParts[0];

        return value + " " + suffix;
    }
}
