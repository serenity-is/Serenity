namespace Serenity.Web.EsBuild;

public class TarFileReaderTests
{
    internal static System.IO.MemoryStream CreateTar(params (string Name, string Content)[] entries)
    {
        var ms = new System.IO.MemoryStream();
        foreach (var (name, content) in entries)
        {
            var header = new byte[512];
            var nameBytes = Encoding.ASCII.GetBytes(name);
            Array.Copy(nameBytes, header, nameBytes.Length);
            var sizeBytes = Encoding.ASCII.GetBytes(
                Convert.ToString(content.Length, 8).PadLeft(11, '0') + "\0");
            Array.Copy(sizeBytes, 0, header, 124, 12);
            ms.Write(header);

            var data = Encoding.ASCII.GetBytes(content);
            ms.Write(data);
            var pad = (512 - (data.Length % 512)) % 512;
            ms.Write(new byte[pad]);
        }

        ms.Write(new byte[512]);
        ms.Position = 0;
        return ms;
    }

    [Fact]
    public void EnumerateEntries_Reads_Names_And_Sizes()
    {
        using var tar = CreateTar(("first.txt", "hello"), ("dir/second.bin", "world!"));

        var entries = TarFileReader.EnumerateEntries(tar).ToList();

        Assert.Equal(2, entries.Count);
        Assert.Equal("first.txt", entries[0].Name);
        Assert.Equal(5, entries[0].Size);
        Assert.Equal("dir/second.bin", entries[1].Name);
        Assert.Equal(6, entries[1].Size);
    }

    [Fact]
    public void EnumerateEntries_Returns_Empty_For_Empty_Block()
    {
        using var tar = CreateTar();

        Assert.Empty(TarFileReader.EnumerateEntries(tar));
    }

    [Fact]
    public void CopyEntryTo_Copies_Content()
    {
        using var tar = CreateTar(("file.txt", "some content"));
        var entry = TarFileReader.EnumerateEntries(tar).First();
        using var target = new System.IO.MemoryStream();

        TarFileReader.CopyEntryTo(tar, entry, target);

        Assert.Equal("some content", Encoding.ASCII.GetString(target.ToArray()));
    }
}
