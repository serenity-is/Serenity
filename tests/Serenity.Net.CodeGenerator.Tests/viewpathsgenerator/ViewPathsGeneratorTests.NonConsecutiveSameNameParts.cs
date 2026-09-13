namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void KeepsSameNameParts_WhenNotConsecutive()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class MVC
                {
                    public static partial class Views
                    {
                        public static partial class SameName
                        {
                            public static partial class Other
                            {
                                public static partial class SameName
                                {
                                    public const string Index = "~/Modules/SameName/Other/SameName/Index.cshtml";
                                }
                            }
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews(
            "Modules/SameName/Other/SameName/Index.cshtml"));
    }
}
