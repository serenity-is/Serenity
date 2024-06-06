using System.IO;

namespace Serenity.Web.EsBuild;

internal static class TarFileReader
{
    public record TarFileEntry(string Name, long Position, long Size);

    public static IEnumerable<TarFileEntry> EnumerateEntries(Stream tarStream)
    {
        var buffer = new byte[512];

        string name;
        long size;
        while (tarStream.Position < tarStream.Length)
        {
            tarStream.Read(buffer, 0, 100);
            name = Encoding.ASCII.GetString(buffer, 0, 100).TrimEnd('\0').Trim();

            if (string.IsNullOrEmpty(name))
                break;

            tarStream.Seek(24, SeekOrigin.Current);
            tarStream.Read(buffer, 0, 12);
            size = Convert.ToInt64(Encoding.ASCII.GetString(buffer, 0, 12).TrimEnd('\0').Trim(), 8);
            tarStream.Seek(376, SeekOrigin.Current);

            yield return new TarFileEntry(name, tarStream.Position, size);

            var offset = 512 - (tarStream.Position + size) % 512;
            if (offset == 512)
                offset = 0;

            tarStream.Seek(size + offset, SeekOrigin.Current);
        }
    }

    public static void CopyEntryTo(Stream tarStream, TarFileEntry entry, Stream targetStream)
    {
        var buffer = new byte[4096];
        var remaining = entry.Size;
        while (remaining > 0)
        {
            var read = tarStream.Read(buffer, 0, (int)Math.Min(remaining, buffer.Length));
            targetStream.Write(buffer, 0, read);
            remaining -= read;
        }
        targetStream.Flush();
    }
}