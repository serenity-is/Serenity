namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void Uses_InternalModifiers_AndAsNamespace()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                internal static partial class Views
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
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews(
            new Options() { AsNamespace = true, Modifiers = "internal" },
            "Modules/Administration/User/UserIndex.cshtml"));
    }
}