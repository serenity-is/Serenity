namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void Generates_Constant_DirectlyUnderViews_ForRootLevelFile()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class MVC
                {
                    public static partial class Views
                    {
                        public const string Test1Index = "~/Modules/Test1Index.cshtml";
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews(
            "Modules/Test1Index.cshtml"));
    }
}