namespace Serenity.CodeGeneration;

public partial class EsmEntryPointsGeneratorTests
{
    [Fact]
    public void Normalizes_NonIdentifierCharacters()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class ESM
                {
                    public const string T__aPage = "~/esm/Modules/æDmÏn/T,;aPage.js";

                    public static partial class Modules
                    {
                        public static partial class ÆDmÏn
                        {
                            public const string T__aPage = "~/esm/Modules/æDmÏn/T,;aPage.js";
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, Generate(
            "Modules/æDmÏn/T,;aPage.ts"));
    }
}