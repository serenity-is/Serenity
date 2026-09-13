namespace Serenity.CodeGeneration;

public partial class EsmEntryPointsGeneratorTests
{
    [Fact]
    public void Generates_EmptyEsmClass_ForNoFiles()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class ESM
                {
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, Generate());
    }
}