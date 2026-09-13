namespace Serenity.CodeGeneration;

public partial class EsmEntryPointsGeneratorTests
{
    [Fact]
    public void Applies_ExcludePatterns()
    {
        var expected = /*lang=c#*/ """
            namespace MyTest
            {
                public static partial class ESM
                {
                    public const string SamplePage = "~/esm/Modules/Sample/SamplePage.js";

                    public static partial class Modules
                    {
                        public static partial class Sample
                        {
                            public const string SamplePage = "~/esm/Modules/Sample/SamplePage.js";
                        }
                    }
                }
            }
            """.ReplaceLineEndings();

        Assert.Equal(expected, Generate(
            new Options() { EntryPoints = ["Modules/**/*.ts", "!Modules/Admin/**"] },
            "Modules/Admin/TestPage.ts",
            "Modules/Sample/SamplePage.ts"));
    }
}