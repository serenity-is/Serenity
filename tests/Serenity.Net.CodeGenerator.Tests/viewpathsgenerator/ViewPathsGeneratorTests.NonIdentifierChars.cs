namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    [Fact]
    public void Normalizes_NonIdentifierCharacters()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class MVC
                {
                    public static partial class Views
                    {
                        public static partial class ÆDmÏn
                        {
                            public const string T__a = "~/Modules/æDmÏn/T,;a.cshtml";
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, GenerateViews(
            "Modules/æDmÏn/T,;a.cshtml"));
    }
}