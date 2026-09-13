namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void Uses_CustomStripViewPaths()
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
                            public const string Index = "~/Custom/Admin/Index.cshtml";
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews(
            new Options() { StripViewPaths = ["Custom/"] },
            "Custom/Admin/Index.cshtml"));
    }
}