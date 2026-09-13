namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void Skips_ViewStartAndViewImports()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class MVC
                {
                    public static partial class Views
                    {
                        public static partial class Admin
                        {
                            public const string Index = "~/Modules/Admin/Index.cshtml";
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews(
            "Modules/Admin/_ViewStart.cshtml",
            "Modules/Admin/_ViewImports.cshtml",
            "Modules/Admin/Index.cshtml"));
    }
}