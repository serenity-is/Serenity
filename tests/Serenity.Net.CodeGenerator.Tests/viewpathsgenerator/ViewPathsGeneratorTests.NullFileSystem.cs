namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void Throws_ArgumentNullException_For_NullFileSystem()
    {
        Assert.Throws<ArgumentNullException>(() => new ViewPathsGenerator(null, ["Modules/"]));
    }
}
