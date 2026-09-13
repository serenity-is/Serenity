namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void KeepsFullPath_WhenNoStripMatches()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class MVC
                {
                    public static partial class Views
                    {
                        public static partial class Other
                        {
                            public static partial class Admin
                            {
                                public const string Index = "~/Other/Admin/Index.cshtml";
                            }
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews(
            "Other/Admin/Index.cshtml"));
    }
}