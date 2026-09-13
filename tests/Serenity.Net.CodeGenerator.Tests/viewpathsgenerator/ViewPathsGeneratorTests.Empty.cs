namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void Generates_EmptyViewsClass_ForNoFiles()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class MVC
                {
                    public static partial class Views
                    {
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews());
    }
}