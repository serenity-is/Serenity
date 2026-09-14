using System.IO;

namespace Serenity.CodeGenerator;

public class GeneratedFileWriterTests
{
    private static void AddFile(MockFileSystem fileSystem, string path, string content)
    {
        var directory = fileSystem.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            fileSystem.CreateDirectory(directory);
        fileSystem.WriteAllText(path, content);
    }

    [Fact]
    public void Constructor_Throws_ForNullArguments()
    {
        var fileSystem = new MockFileSystem();
        var console = new MockGeneratorConsole();

        Assert.Throws<ArgumentNullException>(() => new GeneratedFileWriter(null!, console));
        Assert.Throws<ArgumentNullException>(() => new GeneratedFileWriter(fileSystem, null!));
    }

    [Fact]
    public void ToUTF8BOM_PrependsPreamble()
    {
        var bytes = GeneratedFileWriter.ToUTF8BOM("abc");

        var preamble = Encoding.UTF8.GetPreamble();
        Assert.Equal(preamble, bytes.Take(preamble.Length).ToArray());
        Assert.Equal("abc", Encoding.UTF8.GetString(bytes, preamble.Length, 3));
    }

    [Fact]
    public void WriteAllText_CreatesDirectoryAndFile_WhenTargetMissing()
    {
        var fileSystem = new MockFileSystem();
        var console = new MockGeneratorConsole();
        var writer = new GeneratedFileWriter(fileSystem, console);

        writer.WriteAllText("/root/sub/a.txt", "hello");

        Assert.True(fileSystem.FileExists("/root/sub/a.txt"));
        Assert.Equal("hello", fileSystem.ReadAllText("/root/sub/a.txt"));
        Assert.Empty(console.WriteCalls);
    }

    [Fact]
    public void WriteAllText_Skips_WhenContentSame()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/a.txt", "hello");
        var console = new MockGeneratorConsole();
        var writer = new GeneratedFileWriter(fileSystem, console);

        writer.WriteAllText("/root/a.txt", "hello");

        Assert.Equal("hello", fileSystem.ReadAllText("/root/a.txt"));
        Assert.Empty(console.WriteCalls);
    }

    [Fact]
    public void WriteAllText_Skips_WhenContentSame_IgnoringLineEndings()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/a.txt", "hello\r\nworld\r\n");
        var console = new MockGeneratorConsole();
        var writer = new GeneratedFileWriter(fileSystem, console);

        writer.WriteAllText("/root/a.txt", "hello\nworld\n");

        Assert.Equal("hello\r\nworld\r\n", fileSystem.ReadAllText("/root/a.txt"));
        Assert.Empty(console.WriteCalls);
    }

    [Fact]
    public void WriteAllText_BacksUpExistingFile_WhenNotInteractive()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/a.txt", "old");
        var console = new MockGeneratorConsole();
        var writer = new GeneratedFileWriter(fileSystem, console)
        {
            Interactive = false
        };

        writer.WriteAllText("/root/a.txt", "new");

        Assert.Equal("new", fileSystem.ReadAllText("/root/a.txt"));
        var backups = fileSystem.GetFiles("/root", "*.bak");
        Assert.Single(backups);
        Assert.Equal("old", fileSystem.ReadAllText(backups[0]));
        Assert.Empty(console.WriteCalls);
    }

    [Fact]
    public void WriteAllText_Overwrites_WhenInteractiveYes()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/a.txt", "old");
        var console = new MockGeneratorConsole();
        console.ReadLineAnswers.Enqueue("y");
        var writer = new GeneratedFileWriter(fileSystem, console);

        writer.WriteAllText("/root/a.txt", "new");

        Assert.Equal("new", fileSystem.ReadAllText("/root/a.txt"));
        Assert.Empty(fileSystem.GetFiles("/root", "*.bak"));
    }

    [Fact]
    public void WriteAllText_KeepsExistingFile_WhenInteractiveNo()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/a.txt", "old");
        var console = new MockGeneratorConsole();
        console.ReadLineAnswers.Enqueue("n");
        var writer = new GeneratedFileWriter(fileSystem, console);

        writer.WriteAllText("/root/a.txt", "new");

        Assert.Equal("old", fileSystem.ReadAllText("/root/a.txt"));
    }

    [Fact]
    public void WriteAllText_SkipAll_SkipsSubsequentFiles()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/a.txt", "old-a");
        AddFile(fileSystem, "/root/b.txt", "old-b");
        var console = new MockGeneratorConsole();
        console.ReadLineAnswers.Enqueue("s");
        var writer = new GeneratedFileWriter(fileSystem, console);

        writer.WriteAllText("/root/a.txt", "new-a");
        writer.WriteAllText("/root/b.txt", "new-b");

        Assert.Equal("old-a", fileSystem.ReadAllText("/root/a.txt"));
        Assert.Equal("old-b", fileSystem.ReadAllText("/root/b.txt"));
        Assert.Single(console.WriteCalls);
    }

    [Fact]
    public void WriteAllText_OverwriteAll_OverwritesSubsequentFiles()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/a.txt", "old-a");
        AddFile(fileSystem, "/root/b.txt", "old-b");
        var console = new MockGeneratorConsole();
        console.ReadLineAnswers.Enqueue("all");
        var writer = new GeneratedFileWriter(fileSystem, console);

        writer.WriteAllText("/root/a.txt", "new-a");
        writer.WriteAllText("/root/b.txt", "new-b");

        Assert.Equal("new-a", fileSystem.ReadAllText("/root/a.txt"));
        Assert.Equal("new-b", fileSystem.ReadAllText("/root/b.txt"));
        Assert.Single(console.WriteCalls);
    }

    [Fact]
    public void WriteAllText_Retries_UntilValidAnswer()
    {
        var fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/root/a.txt", "old");
        var console = new MockGeneratorConsole();
        console.ReadLineAnswers.Enqueue("x");
        console.ReadLineAnswers.Enqueue("maybe");
        console.ReadLineAnswers.Enqueue("yes");
        var writer = new GeneratedFileWriter(fileSystem, console);

        writer.WriteAllText("/root/a.txt", "new");

        Assert.Equal("new", fileSystem.ReadAllText("/root/a.txt"));
        Assert.Equal(3, console.WriteCalls.Count);
    }

    [Fact]
    public void WriteAllText_WritesBom_AndBackups_OnPhysicalFileSystem()
    {
        var tempDir = Path.Combine(Path.GetTempPath(),
            "Serenity_GeneratedFileWriter_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try
        {
            var fileSystem = new PhysicalFileSystem();
            var console = new MockGeneratorConsole();
            var writer = new GeneratedFileWriter(fileSystem, console)
            {
                Interactive = false
            };

            var target = Path.Combine(tempDir, "sub", "a.ts");
            writer.WriteAllText(target, "export const x = 1;");

            Assert.True(File.Exists(target));
            var bytes = File.ReadAllBytes(target);
            var preamble = Encoding.UTF8.GetPreamble();
            Assert.Equal(preamble, bytes.Take(preamble.Length).ToArray());

            writer.WriteAllText(target, "export const x = 2;");

            Assert.Equal("export const x = 2;", File.ReadAllText(target));
            var backups = Directory.GetFiles(Path.Combine(tempDir, "sub"), "*.bak");
            Assert.Single(backups);
            Assert.Equal("export const x = 1;", File.ReadAllText(backups[0]));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }
}
