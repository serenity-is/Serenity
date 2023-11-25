using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public partial class CliTests
{
    private readonly IFileSystem fileSystem = new MockFileSystem();
    private readonly IGeneratorConsole console = new MockGeneratorConsole();

    [Fact]
    public void ThrowsArgumentNull_ForAnyNullArgument()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Cli(null, console));

        Assert.Throws<ArgumentNullException>(() =>
            new Cli(fileSystem, null));
    }

}
