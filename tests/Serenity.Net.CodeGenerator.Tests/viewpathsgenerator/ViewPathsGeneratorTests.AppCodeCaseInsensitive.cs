namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void Skips_AppCodeFolder_CaseInsensitively()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class MVC
                {
                    public static partial class Views
                    {
                        public const string Test = "~/Modules/Test.cshtml";
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews(
            "app_code/Foo/Bar.cshtml",
            "Modules/Test.cshtml"));
    }
}
