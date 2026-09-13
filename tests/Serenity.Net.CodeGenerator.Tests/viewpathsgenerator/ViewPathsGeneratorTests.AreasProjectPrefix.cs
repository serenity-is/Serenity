namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void Strips_AreasProjectPrefix()
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
                            public static partial class User
                            {
                                public const string Index = "~/Areas/MyTest.Web/Admin/User/Index.cshtml";
                            }
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews(
            "Areas/MyTest.Web/Admin/User/Index.cshtml"));
    }
}
