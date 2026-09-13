namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void AppendsUnderscore_ForConsecutiveSameNameParts()
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
                            public static partial class SameName_
                            {
                                public static partial class SameName
                                {
                                    public const string Test15Index = "~/Modules/SameName/SameName/SameName/Test15Index.cshtml";
                                    public const string Test16Index = "~/Modules/SameName/SameName/SameName/Test16Index.cshtml";
                                }
                            }
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews(
            "Modules/SameName/SameName/SameName/Test15Index.cshtml",
            "Modules/SameName/SameName/SameName/Test16Index.cshtml"));
    }
}