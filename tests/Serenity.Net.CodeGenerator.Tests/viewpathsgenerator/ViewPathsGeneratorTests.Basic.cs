namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void Generates_Basic()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class MVC
                {
                    public static partial class Views
                    {
                        public static partial class Administration
                        {
                            public static partial class User
                            {
                                public const string UserIndex = "~/Modules/Administration/User/UserIndex.cshtml";
                            }
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews(
            "Modules/Administration/User/UserIndex.cshtml"));
    }
}