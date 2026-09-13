namespace Serenity.CodeGeneration;

public partial class EsmEntryPointsGeneratorTests
{
    [Fact]
    public void Throws_ArgumentNullException_For_NullFileSystem()
    {
        Assert.Throws<ArgumentNullException>(() => new EsmEntryPointsGenerator(null));
    }
}
